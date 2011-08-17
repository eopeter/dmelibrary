using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemCached
{
    internal delegate T UseSocket<T>(DME_PooledSocket socket);
    internal delegate void UseSocket(DME_PooledSocket socket);

    /// <summary>
    /// The ServerPool encapsulates a collection of memcached servers and the associated SocketPool objects.
    /// This class contains the server-selection logic, and contains methods for executing a block of code on 
    /// a socket from the server corresponding to a given key.
    /// </summary>
    internal class DME_ServerPool
    {
        private static DME_LogAdapter logger = DME_LogAdapter.GetLogger(typeof(DME_ServerPool));

		//Expose the socket pools.
		private DME_SocketPool[] hostList;
		internal DME_SocketPool[] HostList { get { return hostList; } }

		private Dictionary<uint, DME_SocketPool> hostDictionary;
		private uint[] hostKeys;

		//Internal configuration properties
		private int sendReceiveTimeout = 2000;
		private uint maxPoolSize = 10;
		private uint minPoolSize = 5;
		private TimeSpan socketRecycleAge = TimeSpan.FromMinutes(30);
		internal int SendReceiveTimeout { get { return sendReceiveTimeout; } set { sendReceiveTimeout = value; } }
		internal uint MaxPoolSize { get { return maxPoolSize; } set { maxPoolSize = value; } }
		internal uint MinPoolSize { get { return minPoolSize; } set { minPoolSize = value; } }
		internal TimeSpan SocketRecycleAge { get { return socketRecycleAge; } set { socketRecycleAge = value; } }

		/// <summary>
		/// Internal constructor. This method takes the array of hosts and sets up an internal list of socketpools.
		/// </summary>
        internal DME_ServerPool(string[] hosts)
        {
			hostDictionary = new Dictionary<uint, DME_SocketPool>();
            List<DME_SocketPool> pools = new List<DME_SocketPool>();
			List<uint> keys = new List<uint>();
			foreach(string host in hosts) {
				//Create pool
                DME_SocketPool pool = new DME_SocketPool(this, host.Trim());

				//Create 250 keys for this pool, store each key in the hostDictionary, as well as in the list of keys.
				for (int i = 0; i < 250; i++) {
					uint key = BitConverter.ToUInt32(new ModifiedFNV1_32().ComputeHash(Encoding.UTF8.GetBytes(host + "-" + i)), 0);
					if (!hostDictionary.ContainsKey(key)) {
						hostDictionary[key] = pool;
						keys.Add(key);
					}
				}

				pools.Add(pool);
			}

			//Hostlist should contain the list of all pools that has been created.
			hostList = pools.ToArray();

			//Hostkeys should contain the list of all key for all pools that have been created.
			//This array forms the server key continuum that we use to lookup which server a
			//given item key hash should be assigned to.
			keys.Sort();
			hostKeys = keys.ToArray();
		}

		/// <summary>
		/// Given an item key hash, this method returns the serverpool which is closest on the server key continuum.
		/// </summary>
        internal DME_SocketPool GetSocketPool(uint hash)
        {
			//Quick return if we only have one host.
			if (hostList.Length == 1) {
				return hostList[0];
			}

			//New "ketama" host selection.
			int i = Array.BinarySearch(hostKeys, hash);

			//If not exact match...
			if(i < 0) {
				//Get the index of the first item bigger than the one searched for.
				i = ~i;

				//If i is bigger than the last index, it was bigger than the last item = use the first item.
				if (i >= hostKeys.Length) {
					i = 0;
				}
			}
			return hostDictionary[hostKeys[i]];
		}

        internal DME_SocketPool GetSocketPool(string host)
        {
            return Array.Find(HostList, delegate(DME_SocketPool socketPool) { return socketPool.Host == host; });
		}

		/// <summary>
		/// This method executes the given delegate on a socket from the server that corresponds to the given hash.
		/// If anything causes an error, the given defaultValue will be returned instead.
		/// This method takes care of disposing the socket properly once the delegate has executed.
		/// </summary>
		internal T Execute<T>(uint hash, T defaultValue, UseSocket<T> use) {
			return Execute(GetSocketPool(hash), defaultValue, use);
		}

        internal T Execute<T>(DME_SocketPool pool, T defaultValue, UseSocket<T> use)
        {
            DME_PooledSocket sock = null;
			try {
				//Acquire a socket
				sock = pool.Acquire();

				//Use the socket as a parameter to the delegate and return its result.
				if (sock != null) {
					return use(sock);
				}
			} catch(Exception e) {
				logger.Error("Error in Execute<T>: " + pool.Host, e);

				//Socket is probably broken
				if (sock != null) {
					sock.Close();
				}
			} finally {
				if (sock != null) {
					sock.Dispose();
				}
			}
			return defaultValue;
		}

        internal void Execute(DME_SocketPool pool, UseSocket use)
        {
            DME_PooledSocket sock = null;
			try {
				//Acquire a socket
				sock = pool.Acquire();

				//Use the socket as a parameter to the delegate and return its result.
				if (sock != null) {
					use(sock);
				}
			} catch(Exception e) {
				logger.Error("Error in Execute: " + pool.Host, e);

				//Socket is probably broken
				if (sock != null) {
					sock.Close();
				}
			}
			finally {
				if(sock != null) {
					sock.Dispose();
				}
			}
		}

		/// <summary>
		/// This method executes the given delegate on all servers.
		/// </summary>
		internal void ExecuteAll(UseSocket use) {
            foreach (DME_SocketPool socketPool in hostList)
            {
				Execute(socketPool, use);
			}
		}
    }
}
