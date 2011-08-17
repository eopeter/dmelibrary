using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemoryCached
{
    /// <summary>
    /// This is a Helper/Wrapper class that calls GC.Collect with allowed conditions/settings.
    /// </summary>
    public static class DME_GCWrapper
    {
        const int GCCOLLECTTIME = 30;

        internal static DateTime LastRunnedTime;
        internal static int RunDifferenceInMinutes;
        private static object _lastRunHandle;

        static DME_GCWrapper()
        {
            RunDifferenceInMinutes = GCCOLLECTTIME;
            LastRunnedTime = DateTime.Now;
            _lastRunHandle = new object();
        }

        /// <summary>
        /// Calls GC.Collect when 30 minutes has passed since last collection.
        /// 
        /// Note: Does Gen 0,1,2 collections.
        /// </summary>
        public static void Collect()
        {
            Collect(2);
        }

        /// <summary>
        /// Calls GC.Collect when 30 minutes has passed since last collection.
        /// </summary>
        /// <param name="generation">The gc generation.</param>
        public static void Collect(int generation)
        {
            TimeSpan difference = DateTime.Now - LastRunnedTime;
            if (difference.Minutes >= RunDifferenceInMinutes)
            {
                lock (_lastRunHandle)
                {
                    //Its possible, that someone checks the difference at the same time as 
                    //someone else has entered the lock, but hasnt yet updated the LastRunnedTime
                    //So to avoid multiple GC collections, a lock is used. However, chances are slim.
                    difference = DateTime.Now - LastRunnedTime;
                    if (difference.Minutes >= RunDifferenceInMinutes)
                    {
                        LastRunnedTime = DateTime.Now;
                        GC.Collect(generation, GCCollectionMode.Optimized);
                    }
                }
            }
            return;
        }
    }
}
