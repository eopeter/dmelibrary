using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System.Threading
{
    #region LockRecursionPolicy
    public enum LockRecursionPolicy
    {
        NoRecursion,
        SupportsRecursion
    }
    #endregion

    #region RecursiveCounts
    internal class RecursiveCounts
    {
        // Fields
        public int upgradecount;
        public int writercount;
    }
    #endregion

    #region ReaderWriterCount
    internal class ReaderWriterCount
    {
        // Fields
        public ReaderWriterCount next;
        public RecursiveCounts rc;
        public int readercount;
        public int threadid = -1;

        // Methods
        public ReaderWriterCount(bool fIsReentrant)
        {
            if (fIsReentrant)
            {
                rc = new RecursiveCounts();
            }
        }
    }
    #endregion

    #region LockRecursionException
    [Serializable, HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    public class LockRecursionException : Exception
    {
        // Methods
        public LockRecursionException()
        {
        }

        public LockRecursionException(string message)
            : base(message)
        {
        }

        protected LockRecursionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public LockRecursionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    #endregion

    #region ReaderWriterLockSlim
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true),
    HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ReaderWriterLockSlim : IDisposable
    {
        #region private fidlds
        private const int hashTableSize = 0xff;
        private const int LockSleep0Count = 5;
        private const int LockSpinCount = 10;
        private const int LockSpinCycles = 20;
        private const uint MAX_READER = 0xffffffe;
        private const int MaxSpinCount = 20;
        private const uint READER_MASK = 0xfffffff;
        private const uint WAITING_UPGRADER = 0x20000000;
        private const uint WAITING_WRITERS = 0x40000000;
        private const uint WRITER_HELD = 0x80000000;
        private readonly bool fIsReentrant;
        private bool fDisposed;
        private bool fNoWaiters;
        private bool fUpgradeThreadHoldingRead;
        private int myLock;
        private uint numReadWaiters;
        private uint numUpgradeWaiters;
        private uint numWriteUpgradeWaiters;
        private uint numWriteWaiters;
        private uint owners;
        private EventWaitHandle readEvent;
        private ReaderWriterCount[] rwc;
        private EventWaitHandle upgradeEvent;
        private int upgradeLockOwnerId;
        private EventWaitHandle waitUpgradeEvent;
        private EventWaitHandle writeEvent;
        private int writeLockOwnerId;
        
        #endregion

        #region public contructs
        public ReaderWriterLockSlim()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        public ReaderWriterLockSlim(LockRecursionPolicy recursionPolicy)
        {
            if (recursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                fIsReentrant = true;
            }
            InitializeThreadCounts();
        }
        #endregion

        #region public properties
        public int CurrentReadCount
        {
            get
            {
                int numReaders = (int)GetNumReaders();
                if (upgradeLockOwnerId != -1)
                {
                    return (numReaders - 1);
                }
                return numReaders;
            }
        }

        public bool IsReadLockHeld
        {
            get { return (RecursiveReadCount > 0); }
        }

        public bool IsUpgradeableReadLockHeld
        {
            get { return (RecursiveUpgradeCount > 0); }
        }

        public bool IsWriteLockHeld
        {
            get { return (RecursiveWriteCount > 0); }
        }

        public LockRecursionPolicy RecursionPolicy
        {
            get
            {
                if (fIsReentrant)
                {
                    return LockRecursionPolicy.SupportsRecursion;
                }
                return LockRecursionPolicy.NoRecursion;
            }
        }

        public int RecursiveReadCount
        {
            get
            {
                int managedThreadId = Thread.CurrentThread.ManagedThreadId;
                int readercount = 0;
                EnterMyLock();
                ReaderWriterCount threadRWCount = GetThreadRWCount(managedThreadId, true);
                if (threadRWCount != null)
                {
                    readercount = threadRWCount.readercount;
                }
                ExitMyLock();
                return readercount;
            }
        }

        public int RecursiveUpgradeCount
        {
            get
            {
                int managedThreadId = Thread.CurrentThread.ManagedThreadId;
                if (fIsReentrant)
                {
                    int upgradecount = 0;
                    EnterMyLock();
                    ReaderWriterCount threadRWCount = GetThreadRWCount(managedThreadId, true);
                    if (threadRWCount != null)
                    {
                        upgradecount = threadRWCount.rc.upgradecount;
                    }
                    ExitMyLock();
                    return upgradecount;
                }
                if (managedThreadId == upgradeLockOwnerId)
                {
                    return 1;
                }
                return 0;
            }
        }

        public int RecursiveWriteCount
        {
            get
            {
                int managedThreadId = Thread.CurrentThread.ManagedThreadId;
                int writercount = 0;
                if (fIsReentrant)
                {
                    EnterMyLock();
                    ReaderWriterCount threadRWCount = GetThreadRWCount(managedThreadId, true);
                    if (threadRWCount != null)
                    {
                        writercount = threadRWCount.rc.writercount;
                    }
                    ExitMyLock();
                    return writercount;
                }
                if (managedThreadId == writeLockOwnerId)
                {
                    return 1;
                }
                return 0;
            }
        }

        public int WaitingReadCount
        {
            get { return (int)numReadWaiters; }
        }

        public int WaitingUpgradeCount
        {
            get { return (int)numUpgradeWaiters; }
        }

        public int WaitingWriteCount
        {
            get { return (int)numWriteWaiters; }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (fDisposed)
                {
                    throw new ObjectDisposedException(null);
                }
                if (((WaitingReadCount > 0) || (WaitingUpgradeCount > 0)) || (WaitingWriteCount > 0))
                {
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_IncorrectDispose"));
                }
                if ((IsReadLockHeld || IsUpgradeableReadLockHeld) || IsWriteLockHeld)
                {
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_IncorrectDispose"));
                }
                if (writeEvent != null)
                {
                    writeEvent.Close();
                    writeEvent = null;
                }
                if (readEvent != null)
                {
                    readEvent.Close();
                    readEvent = null;
                }
                if (upgradeEvent != null)
                {
                    upgradeEvent.Close();
                    upgradeEvent = null;
                }
                if (waitUpgradeEvent != null)
                {
                    waitUpgradeEvent.Close();
                    waitUpgradeEvent = null;
                }
                fDisposed = true;
            }
        }

        #endregion

        #region private methods
        private string GetStringFromResource(string key)
        {
            return key;
        }

        private void ClearUpgraderWaiting()
        {
            owners &= 0xdfffffff;
        }

        private void ClearWriterAcquired()
        {
            owners &= 0x7fffffff;
        }

        private void ClearWritersWaiting()
        {
            owners &= 0xbfffffff;
        }


        private void EnterMyLock()
        {
            if (Interlocked.CompareExchange(ref myLock, 1, 0) != 0)
            {
                EnterMyLockSpin();
            }
        }

        private void EnterMyLockSpin()
        {
            int processorCount = Environment.ProcessorCount;
            int num2 = 0;
            while (true)
            {
                if ((num2 < 10) && (processorCount > 1))
                {
                    Thread.SpinWait(20 * (num2 + 1));
                }
                else if (num2 < 15)
                {
                    Thread.Sleep(0);
                }
                else
                {
                    Thread.Sleep(1);
                }
                if ((myLock == 0) && (Interlocked.CompareExchange(ref myLock, 1, 0) == 0))
                {
                    return;
                }
                num2++;
            }
        }



        private void ExitAndWakeUpAppropriateWaiters()
        {
            if (fNoWaiters)
            {
                ExitMyLock();
            }
            else
            {
                ExitAndWakeUpAppropriateWaitersPreferringWriters();
            }
        }

        private void ExitAndWakeUpAppropriateWaitersPreferringWriters()
        {
            bool flag = false;
            bool flag2 = false;
            uint numReaders = GetNumReaders();
            if ((fIsReentrant && (numWriteUpgradeWaiters > 0)) && (fUpgradeThreadHoldingRead && (numReaders == 2)))
            {
                ExitMyLock();
                waitUpgradeEvent.Set();
            }
            else if ((numReaders == 1) && (numWriteUpgradeWaiters > 0))
            {
                ExitMyLock();
                waitUpgradeEvent.Set();
            }
            else if ((numReaders == 0) && (numWriteWaiters > 0))
            {
                ExitMyLock();
                writeEvent.Set();
            }
            else if (numReaders >= 0)
            {
                if ((numReadWaiters == 0) && (numUpgradeWaiters == 0))
                {
                    ExitMyLock();
                }
                else
                {
                    if (numReadWaiters != 0)
                    {
                        flag2 = true;
                    }
                    if ((numUpgradeWaiters != 0) && (upgradeLockOwnerId == -1))
                    {
                        flag = true;
                    }
                    ExitMyLock();
                    if (flag2)
                    {
                        readEvent.Set();
                    }
                    if (flag)
                    {
                        upgradeEvent.Set();
                    }
                }
            }
            else
            {
                ExitMyLock();
            }
        }
        private uint GetNumReaders()
        {
            return (owners & 0xfffffff);
        }

        private ReaderWriterCount GetThreadRWCount(int id, bool DontAllocate)
        {
            int index = id & 0xff;
            ReaderWriterCount count = null;
            if (rwc[index].threadid == id)
            {
                return rwc[index];
            }
            if (IsRWEntryEmpty(rwc[index]) && !DontAllocate)
            {
                if (rwc[index].next == null)
                {
                    rwc[index].threadid = id;
                    return rwc[index];
                }
                count = rwc[index];
            }
            ReaderWriterCount next = rwc[index].next;
            while (next != null)
            {
                if (next.threadid == id)
                {
                    return next;
                }
                if ((count == null) && IsRWEntryEmpty(next))
                {
                    count = next;
                }
                next = next.next;
            }
            if (DontAllocate)
            {
                return null;
            }
            if (count == null)
            {
                next = new ReaderWriterCount(fIsReentrant);
                next.threadid = id;
                next.next = rwc[index].next;
                rwc[index].next = next;
                return next;
            }
            count.threadid = id;
            return count;
        }

        private void InitializeThreadCounts()
        {
            rwc = new ReaderWriterCount[0x100];
            for (int i = 0; i < rwc.Length; i++)
            {
                rwc[i] = new ReaderWriterCount(fIsReentrant);
            }
            upgradeLockOwnerId = -1;
            writeLockOwnerId = -1;
        }

        private static bool IsRWEntryEmpty(ReaderWriterCount rwc)
        {
            return ((rwc.threadid == -1) ||
                    (((rwc.readercount == 0) && (rwc.rc == null)) ||
                     (((rwc.readercount == 0) && (rwc.rc.writercount == 0)) && (rwc.rc.upgradecount == 0))));
        }

        private static bool IsRwHashEntryChanged(ReaderWriterCount lrwc, int id)
        {
            return (lrwc.threadid != id);
        }

        private bool IsWriterAcquired()
        {
            return ((owners & 0xbfffffff) == 0);
        }

        private void LazyCreateEvent(ref EventWaitHandle waitEvent, bool makeAutoResetEvent)
        {
            EventWaitHandle handle;
            ExitMyLock();
            if (makeAutoResetEvent)
            {
                handle = new AutoResetEvent(false);
            }
            else
            {
                handle = new ManualResetEvent(false);
            }
            EnterMyLock();
            if (waitEvent == null)
            {
                waitEvent = handle;
            }
            else
            {
                handle.Close();
            }
        }

        private void SetUpgraderWaiting()
        {
            owners |= 0x20000000;
        }

        private void SetWriterAcquired()
        {
            owners |= 0x80000000;
        }

        private void SetWritersWaiting()
        {
            owners |= 0x40000000;
        }

        private static void SpinWait(int SpinCount)
        {
            if ((SpinCount < 5) && (Environment.ProcessorCount > 1))
            {
                Thread.SpinWait(20 * SpinCount);
            }
            else if (SpinCount < 0x11)
            {
                Thread.Sleep(0);
            }
            else
            {
                Thread.Sleep(1);
            }
        }

        private void ExitMyLock()
        {
            myLock = 0;
        }

        private bool WaitOnEvent(EventWaitHandle waitEvent, ref uint numWaiters, int millisecondsTimeout)
        {
            waitEvent.Reset();
            numWaiters++;
            fNoWaiters = false;
            if (numWriteWaiters == 1)
            {
                SetWritersWaiting();
            }
            if (numWriteUpgradeWaiters == 1)
            {
                SetUpgraderWaiting();
            }
            bool flag = false;
            ExitMyLock();
            try
            {
                flag = waitEvent.WaitOne(millisecondsTimeout, false);
            }
            finally
            {
                EnterMyLock();
                numWaiters--;
                if (((numWriteWaiters == 0) && (numWriteUpgradeWaiters == 0)) &&
                    ((numUpgradeWaiters == 0) && (numReadWaiters == 0)))
                {
                    fNoWaiters = true;
                }
                if (numWriteWaiters == 0)
                {
                    ClearWritersWaiting();
                }
                if (numWriteUpgradeWaiters == 0)
                {
                    ClearUpgraderWaiting();
                }
                if (!flag)
                {
                    ExitMyLock();
                }
            }
            return flag;
        }
        #endregion

        #region public methods

        #region Enter and Exit ReadLock
        public void EnterReadLock()
        {
            TryEnterReadLock(-1);
        }
        public void ExitReadLock()
        {
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            ReaderWriterCount threadRWCount = null;
            EnterMyLock();
            threadRWCount = GetThreadRWCount(managedThreadId, true);
            if (!fIsReentrant)
            {
                if (threadRWCount == null)
                {
                    ExitMyLock();
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedRead"));
                }
            }
            else
            {
                if ((threadRWCount == null) || (threadRWCount.readercount < 1))
                {
                    ExitMyLock();
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedRead"));
                }
                if (threadRWCount.readercount > 1)
                {
                    threadRWCount.readercount--;
                    ExitMyLock();
                    return;
                }
                if (managedThreadId == upgradeLockOwnerId)
                {
                    fUpgradeThreadHoldingRead = false;
                }
            }
            owners--;
            threadRWCount.readercount--;
            ExitAndWakeUpAppropriateWaiters();
        }
        #endregion

        #region Enter and Exit WriteLock
        public void EnterWriteLock()
        {
            TryEnterWriteLock(-1);
        }
        public void ExitWriteLock()
        {
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            if (!fIsReentrant)
            {
                if (managedThreadId != writeLockOwnerId)
                {
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedWrite"));
                }
                EnterMyLock();
            }
            else
            {
                EnterMyLock();
                ReaderWriterCount threadRWCount = GetThreadRWCount(managedThreadId, false);
                if (threadRWCount == null)
                {
                    ExitMyLock();
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedWrite"));
                }
                RecursiveCounts rc = threadRWCount.rc;
                if (rc.writercount < 1)
                {
                    ExitMyLock();
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedWrite"));
                }
                rc.writercount--;
                if (rc.writercount > 0)
                {
                    ExitMyLock();
                    return;
                }
            }
            ClearWriterAcquired();
            writeLockOwnerId = -1;
            ExitAndWakeUpAppropriateWaiters();
        }
        #endregion

        #region Enter and Exit UpgradeableReadLock
        public void EnterUpgradeableReadLock()
        {
            TryEnterUpgradeableReadLock(-1);
        }
        public void ExitUpgradeableReadLock()
        {
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            if (!fIsReentrant)
            {
                if (managedThreadId != upgradeLockOwnerId)
                {
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedUpgrade"));
                }
                EnterMyLock();
            }
            else
            {
                EnterMyLock();
                ReaderWriterCount threadRWCount = GetThreadRWCount(managedThreadId, true);
                if (threadRWCount == null)
                {
                    ExitMyLock();
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedUpgrade"));
                }
                RecursiveCounts rc = threadRWCount.rc;
                if (rc.upgradecount < 1)
                {
                    ExitMyLock();
                    throw new SynchronizationLockException(
                        GetStringFromResource("SynchronizationLockException_MisMatchedUpgrade"));
                }
                rc.upgradecount--;
                if (rc.upgradecount > 0)
                {
                    ExitMyLock();
                    return;
                }
                fUpgradeThreadHoldingRead = false;
            }
            owners--;
            upgradeLockOwnerId = -1;
            ExitAndWakeUpAppropriateWaiters();
        }
        #endregion

        #region TryEnterReadLock
        public bool TryEnterReadLock(int millisecondsTimeout)
        {
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (fDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            ReaderWriterCount lrwc = null;
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            if (!fIsReentrant)
            {
                if (managedThreadId == writeLockOwnerId)
                {
                    throw new LockRecursionException(GetStringFromResource("LockRecursionException_ReadAfterWriteNotAllowed"));
                }
                EnterMyLock();
                lrwc = GetThreadRWCount(managedThreadId, false);
                if (lrwc.readercount > 0)
                {
                    ExitMyLock();
                    throw new LockRecursionException(GetStringFromResource("LockRecursionException_RecursiveReadNotAllowed"));
                }
                if (managedThreadId == upgradeLockOwnerId)
                {
                    lrwc.readercount++;
                    owners++;
                    ExitMyLock();
                    return true;
                }
            }
            else
            {
                EnterMyLock();
                lrwc = GetThreadRWCount(managedThreadId, false);
                if (lrwc.readercount > 0)
                {
                    lrwc.readercount++;
                    ExitMyLock();
                    return true;
                }
                if (managedThreadId == upgradeLockOwnerId)
                {
                    lrwc.readercount++;
                    owners++;
                    ExitMyLock();
                    fUpgradeThreadHoldingRead = true;
                    return true;
                }
                if (managedThreadId == writeLockOwnerId)
                {
                    lrwc.readercount++;
                    owners++;
                    ExitMyLock();
                    return true;
                }
            }
            bool flag = true;
            int spinCount = 0;
        Label_013D:
            if (owners < 0xffffffe)
            {
                owners++;
                lrwc.readercount++;
            }
            else
            {
                if (spinCount < 20)
                {
                    ExitMyLock();
                    if (millisecondsTimeout == 0)
                    {
                        return false;
                    }
                    spinCount++;
                    SpinWait(spinCount);
                    EnterMyLock();
                    if (IsRwHashEntryChanged(lrwc, managedThreadId))
                    {
                        lrwc = GetThreadRWCount(managedThreadId, false);
                    }
                }
                else if (readEvent == null)
                {
                    LazyCreateEvent(ref readEvent, false);
                    if (IsRwHashEntryChanged(lrwc, managedThreadId))
                    {
                        lrwc = GetThreadRWCount(managedThreadId, false);
                    }
                }
                else
                {
                    flag = WaitOnEvent(readEvent, ref numReadWaiters, millisecondsTimeout);
                    if (!flag)
                    {
                        return false;
                    }
                    if (IsRwHashEntryChanged(lrwc, managedThreadId))
                    {
                        lrwc = GetThreadRWCount(managedThreadId, false);
                    }
                }
                goto Label_013D;
            }
            ExitMyLock();
            return flag;
        }
        public bool TryEnterReadLock(TimeSpan timeout)
        {
            long totalMilliseconds = (long)timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            int millisecondsTimeout = (int)timeout.TotalMilliseconds;
            return TryEnterReadLock(millisecondsTimeout);
        }
        #endregion

        #region TryEnterWriteLock
        public bool TryEnterWriteLock(int millisecondsTimeout)
        {
            ReaderWriterCount threadRWCount;
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (fDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            bool flag = false;
            if (!fIsReentrant)
            {
                if (managedThreadId == writeLockOwnerId)
                {
                    throw new LockRecursionException(GetStringFromResource("LockRecursionException_RecursiveWriteNotAllowed"));
                }
                if (managedThreadId == upgradeLockOwnerId)
                {
                    flag = true;
                }
                EnterMyLock();
                threadRWCount = GetThreadRWCount(managedThreadId, true);
                if ((threadRWCount != null) && (threadRWCount.readercount > 0))
                {
                    ExitMyLock();
                    throw new LockRecursionException(GetStringFromResource("LockRecursionException_WriteAfterReadNotAllowed"));
                }
            }
            else
            {
                EnterMyLock();
                threadRWCount = GetThreadRWCount(managedThreadId, false);
                if (managedThreadId == writeLockOwnerId)
                {
                    threadRWCount.rc.writercount++;
                    ExitMyLock();
                    return true;
                }
                if (managedThreadId == upgradeLockOwnerId)
                {
                    flag = true;
                }
                else if (threadRWCount.readercount > 0)
                {
                    ExitMyLock();
                    throw new LockRecursionException(GetStringFromResource("LockRecursionException_WriteAfterReadNotAllowed"));
                }
            }
            int spinCount = 0;
        Label_00EC:
            if (IsWriterAcquired())
            {
                SetWriterAcquired();
            }
            else
            {
                if (flag)
                {
                    uint numReaders = GetNumReaders();
                    if (numReaders == 1)
                    {
                        SetWriterAcquired();
                        goto Label_01DD;
                    }
                    if ((numReaders == 2) && (threadRWCount != null))
                    {
                        if (IsRwHashEntryChanged(threadRWCount, managedThreadId))
                        {
                            threadRWCount = GetThreadRWCount(managedThreadId, false);
                        }
                        if (threadRWCount.readercount > 0)
                        {
                            SetWriterAcquired();
                            goto Label_01DD;
                        }
                    }
                }
                if (spinCount < 20)
                {
                    ExitMyLock();
                    if (millisecondsTimeout == 0)
                    {
                        return false;
                    }
                    spinCount++;
                    SpinWait(spinCount);
                    EnterMyLock();
                    goto Label_00EC;
                }
                if (flag)
                {
                    if (waitUpgradeEvent != null)
                    {
                        if (!WaitOnEvent(waitUpgradeEvent, ref numWriteUpgradeWaiters, millisecondsTimeout))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        LazyCreateEvent(ref waitUpgradeEvent, true);
                    }
                    goto Label_00EC;
                }
                if (writeEvent == null)
                {
                    LazyCreateEvent(ref writeEvent, true);
                    goto Label_00EC;
                }
                if (WaitOnEvent(writeEvent, ref numWriteWaiters, millisecondsTimeout))
                {
                    goto Label_00EC;
                }
                return false;
            }
        Label_01DD:
            if (fIsReentrant)
            {
                if (IsRwHashEntryChanged(threadRWCount, managedThreadId))
                {
                    threadRWCount = GetThreadRWCount(managedThreadId, false);
                }
                threadRWCount.rc.writercount++;
            }
            ExitMyLock();
            writeLockOwnerId = managedThreadId;
            return true;
        }
        public bool TryEnterWriteLock(TimeSpan timeout)
        {
            long totalMilliseconds = (long)timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            int millisecondsTimeout = (int)timeout.TotalMilliseconds;
            return TryEnterWriteLock(millisecondsTimeout);
        }
        #endregion

        #region TryEnterUpgradeableReadLock
        public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
        {
            ReaderWriterCount threadRWCount;
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (fDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            if (!fIsReentrant)
            {
                if (managedThreadId == upgradeLockOwnerId)
                {
                    throw new LockRecursionException(
                        GetStringFromResource("LockRecursionException_RecursiveUpgradeNotAllowed"));
                }
                if (managedThreadId == writeLockOwnerId)
                {
                    throw new LockRecursionException(
                        GetStringFromResource("LockRecursionException_UpgradeAfterWriteNotAllowed"));
                }
                EnterMyLock();
                threadRWCount = GetThreadRWCount(managedThreadId, true);
                if ((threadRWCount != null) && (threadRWCount.readercount > 0))
                {
                    ExitMyLock();
                    throw new LockRecursionException(
                        GetStringFromResource("LockRecursionException_UpgradeAfterReadNotAllowed"));
                }
            }
            else
            {
                EnterMyLock();
                threadRWCount = GetThreadRWCount(managedThreadId, false);
                if (managedThreadId == upgradeLockOwnerId)
                {
                    threadRWCount.rc.upgradecount++;
                    ExitMyLock();
                    return true;
                }
                if (managedThreadId == writeLockOwnerId)
                {
                    owners++;
                    upgradeLockOwnerId = managedThreadId;
                    threadRWCount.rc.upgradecount++;
                    if (threadRWCount.readercount > 0)
                    {
                        fUpgradeThreadHoldingRead = true;
                    }
                    ExitMyLock();
                    return true;
                }
                if (threadRWCount.readercount > 0)
                {
                    ExitMyLock();
                    throw new LockRecursionException(
                        GetStringFromResource("LockRecursionException_UpgradeAfterReadNotAllowed"));
                }
            }
            int spinCount = 0;
        Label_0139:
            if ((upgradeLockOwnerId == -1) && (owners < 0xffffffe))
            {
                owners++;
                upgradeLockOwnerId = managedThreadId;
            }
            else
            {
                if (spinCount < 20)
                {
                    ExitMyLock();
                    if (millisecondsTimeout == 0)
                    {
                        return false;
                    }
                    spinCount++;
                    SpinWait(spinCount);
                    EnterMyLock();
                    goto Label_0139;
                }
                if (upgradeEvent == null)
                {
                    LazyCreateEvent(ref upgradeEvent, true);
                    goto Label_0139;
                }
                if (WaitOnEvent(upgradeEvent, ref numUpgradeWaiters, millisecondsTimeout))
                {
                    goto Label_0139;
                }
                return false;
            }
            if (fIsReentrant)
            {
                if (IsRwHashEntryChanged(threadRWCount, managedThreadId))
                {
                    threadRWCount = GetThreadRWCount(managedThreadId, false);
                }
                threadRWCount.rc.upgradecount++;
            }
            ExitMyLock();
            return true;
        }
        public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
        {
            long totalMilliseconds = (long)timeout.TotalMilliseconds;
            if ((totalMilliseconds < -1L) || (totalMilliseconds > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            int millisecondsTimeout = (int)timeout.TotalMilliseconds;
            return TryEnterUpgradeableReadLock(millisecondsTimeout);
        }
        #endregion

        #endregion
    }
    #endregion 
}
/*
	Dispose	                                释放 ReaderWriterLockSlim 类的当前实例所使用的所有资源。
	EnterReadLock	                        尝试进入读取模式锁定状态。
	EnterUpgradeableReadLock	            尝试进入可升级模式锁定状态。
	EnterWriteLock	                        尝试进入写入模式锁定状态。

	ExitReadLock	                        减少读取模式的递归计数，并在生成的计数为 0（零）时退出读取模式。
	ExitUpgradeableReadLock	                减少可升级模式的递归计数，并在生成的计数为 0（零）时退出可升级模式。
	ExitWriteLock	                        减少写入模式的递归计数，并在生成的计数为 0（零）时退出写入模式。

	TryEnterReadLock	                    已重载。 尝试进入读取模式锁定状态，可以选择超时时间。
	TryEnterUpgradeableReadLock	            已重载。 尝试进入可升级模式锁定状态，可以选择超时时间。
	TryEnterWriteLock	                    已重载。 尝试进入写入模式锁定状态，可以选择超时时间。
 

 * 时候，在一个原子操作里面交换读写锁是非常有用的，比如，当某个item不在list中的时候，添加此item进去。最好的情况是，最小化写如锁的时间，例如像下面这样处理:
    1 获得一个读取锁
    2 测试list是否包含item，如果是，则返回
    3 释放读取锁
    4 获得一个写入锁
    5 写入item到list中，释放写入锁。
     但是在步骤3、4之间，当另外一个线程可能偷偷修改List（比如说添加同样一个Item）,ReaderWriterLockSlim通过提供第三种锁来解决这个问题，这就是upgradeable lock。一个可升级锁和read lock 类似，只是它能够通过一个原子操作，被提升为write lock。使用方法如下:
 
调用 EnterUpgradeableReadLock
读操作(e.g. test if item already present in list)
调用 EnterWriteLock (this converts the upgradeable lock to a write lock)
写操作(e.g. add item to list)
调用ExitWriteLock (this converts the write lock back to an upgradeable lock)
其他读取的过程
调用ExitUpgradeableReadLock

*/