﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace DME.Base.Cache.MemCached
{
    /// <summary>
    /// The SocketPool encapsulates the list of PooledSockets against one specific host, and contains methods for 
    /// acquiring or returning PooledSockets.
    /// </summary>
    [DebuggerDisplay("[ Host: {Host} ]")]
    internal class DME_SocketPool
    {
        private static DME_LogAdapter logger = DME_LogAdapter.GetLogger(typeof(DME_SocketPool));

		/// <summary>
		/// If the host stops responding, we mark it as dead for this amount of seconds, 
		/// and we double this for each consecutive failed retry. If the host comes alive
		/// again, we reset this to 1 again.
		/// </summary>
		private int deadEndPointSecondsUntilRetry = 1;
		private const int maxDeadEndPointSecondsUntilRetry = 60*10; //10 minutes
		private DME_ServerPool owner;
		private IPEndPoint endPoint;
		private Queue<DME_PooledSocket> queue;

		//Debug variables and properties
		private int newsockets = 0;
		private int failednewsockets = 0;
		private int reusedsockets = 0;
		private int deadsocketsinpool = 0;
		private int deadsocketsonreturn = 0;
		private int dirtysocketsonreturn = 0;
		private int acquired = 0;
		public int NewSockets { get { return newsockets; } }
		public int FailedNewSockets { get { return failednewsockets; } }
		public int ReusedSockets { get { return reusedsockets; } }
		public int DeadSocketsInPool { get { return deadsocketsinpool; } }
		public int DeadSocketsOnReturn { get { return deadsocketsonreturn; } }
		public int DirtySocketsOnReturn { get { return dirtysocketsonreturn; } }
		public int Acquired { get { return acquired; } }
		public int Poolsize { get { return queue.Count; } }

		//Public variables and properties
		public readonly string Host;

		private bool isEndPointDead = false;
		public bool IsEndPointDead { get { return isEndPointDead; } }

		private DateTime deadEndPointRetryTime;
		public DateTime DeadEndPointRetryTime { get { return deadEndPointRetryTime; } }

        internal DME_SocketPool(DME_ServerPool owner, string host)
        {
			Host = host;
			this.owner = owner;
			endPoint = getEndPoint(host);
            queue = new Queue<DME_PooledSocket>();
		}

		/// <summary>
		/// This method parses the given string into an IPEndPoint.
		/// If the string is malformed in some way, or if the host cannot be resolved, this method will throw an exception.
		/// </summary>
		private static IPEndPoint getEndPoint(string host) {
			//Parse port, default to 11211.
			int port = 11211;
			if(host.Contains(":")) {
				string[] split = host.Split(new char[] { ':' });
				if(!Int32.TryParse(split[1], out port)) {
					throw new ArgumentException("Unable to parse host: " + host);
				}
				host = split[0];
			}

			//Parse host string.
			IPAddress address;
			if(IPAddress.TryParse(host, out address)) {
				//host string successfully resolved as an IP address.
			} else {
				//See if we can resolve it as a hostname
				try {
					address = Dns.GetHostEntry(host).AddressList[0];
				} catch(Exception e) {
					throw new ArgumentException("Unable to resolve host: " + host, e);
				}
			}

			return new IPEndPoint(address, port);
		}

		/// <summary>
		/// Gets a socket from the pool.
		/// If there are no free sockets, a new one will be created. If something goes
		/// wrong while creating the new socket, this pool's endpoint will be marked as dead
		/// and all subsequent calls to this method will return null until the retry interval
		/// has passed.
		/// </summary>
        internal DME_PooledSocket Acquire()
        {
			//Do we have free sockets in the pool?
			//if so - return the first working one.
			//if not - create a new one.
			Interlocked.Increment(ref acquired);
			lock(queue) {
				while(queue.Count > 0) {
                    DME_PooledSocket socket = queue.Dequeue();
					if(socket != null && socket.IsAlive) {
						Interlocked.Increment(ref reusedsockets);
						return socket;
					}
					Interlocked.Increment(ref deadsocketsinpool);
				}
			}

			Interlocked.Increment(ref newsockets);
			//If we know the endpoint is dead, check if it is time for a retry, otherwise return null.
			if (isEndPointDead) {
				if (DateTime.Now > deadEndPointRetryTime) {
					//Retry
					isEndPointDead = false;
				} else {
					//Still dead
					return null;
				}
			} 

			//Try to create a new socket. On failure, mark endpoint as dead and return null.
			try {
                DME_PooledSocket socket = new DME_PooledSocket(this, endPoint, owner.SendReceiveTimeout);
				//Reset retry timer on success.
				deadEndPointSecondsUntilRetry = 1;
				return socket;
			}
			catch (Exception e) {
				Interlocked.Increment(ref failednewsockets);
				logger.Error("Error connecting to: " + endPoint.Address, e);
				//Mark endpoint as dead
				isEndPointDead = true;
				//Retry in 2 minutes
				deadEndPointRetryTime = DateTime.Now.AddSeconds(deadEndPointSecondsUntilRetry);
				if (deadEndPointSecondsUntilRetry < maxDeadEndPointSecondsUntilRetry) {
					deadEndPointSecondsUntilRetry = deadEndPointSecondsUntilRetry * 2; //Double retry interval until next time
				}
				return null;
			}
		}

		/// <summary>
		/// Returns a socket to the pool.
		/// If the socket is dead, it will be destroyed.
		/// If there are more than MaxPoolSize sockets in the pool, it will be destroyed.
		/// If there are less than MinPoolSize sockets in the pool, it will always be put back.
		/// If there are something inbetween those values, the age of the socket is checked. 
		/// If it is older than the SocketRrecycleAge, it is destroyed, otherwise it will be 
		/// put back in the pool.
		/// </summary>
        internal void Return(DME_PooledSocket socket)
        {
			//If the socket is dead, destroy it.
			if (!socket.IsAlive) {
				Interlocked.Increment(ref deadsocketsonreturn);
				socket.Close();
			} else {
				//Clean up socket
				if (socket.Reset()) {
					Interlocked.Increment(ref dirtysocketsonreturn);
				}

				//Check pool size.
				if (queue.Count >= owner.MaxPoolSize) {
					//If the pool is full, destroy the socket.
					socket.Close();
				} else if (queue.Count > owner.MinPoolSize && DateTime.Now - socket.Created > owner.SocketRecycleAge) {
					//If we have more than the minimum amount of sockets, but less than the max, and the socket is older than the recycle age, we destroy it.
					socket.Close();
				} else {
					//Put the socket back in the pool.
					lock (queue) {
						queue.Enqueue(socket);
					}
				}
			}
		}
    }
}
