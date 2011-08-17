using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemoryCached
{
    class DME_CacheItem : IDisposable
    {
        public DateTime timestamp { get; set; }
        public DateTime expiryDate { get; set; }
        public byte[] value { get; set; }

        public DME_CacheItem(DateTime timestamp, byte[] value)
        {
            Initialize(timestamp, value, DateTime.Now);
        }

        public DME_CacheItem(DateTime timestamp, byte[] value, DateTime expiryDate)
        {
            Initialize(timestamp, value, expiryDate);
        }

        internal void Initialize(DateTime timestamp, byte[] value, DateTime expiryDate)
        {
            this.timestamp = timestamp;
            this.value = value;
            this.expiryDate = expiryDate;
        }

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                this.value = null;

                // Note disposing has been done.
                disposed = true;
            }
        }
    }
}
