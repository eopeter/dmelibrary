using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace DME.Base.Cache.MemCached
{
    internal class DME_PooledSocket : IDisposable
    {
        private static DME_LogAdapter logger = DME_LogAdapter.GetLogger(typeof(DME_PooledSocket));
		
		private DME_SocketPool socketPool;
		private Socket socket;
		private Stream stream;
		public readonly DateTime Created;

        public DME_PooledSocket(DME_SocketPool socketPool, IPEndPoint endPoint, int sendReceiveTimeout)
        {
			this.socketPool = socketPool;
			Created = DateTime.Now;

			//Set up the socket.
			socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendReceiveTimeout);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, sendReceiveTimeout);
			socket.ReceiveTimeout = sendReceiveTimeout;
			socket.SendTimeout = sendReceiveTimeout;

			//Do not use Nagle's Algorithm
			socket.NoDelay = true;

			//Establish connection
			socket.Connect(endPoint);

			//Wraps two layers of streams around the socket for communication.
			stream = new BufferedStream(new NetworkStream(socket, false));
		}

		/// <summary>
		/// Disposing of a PooledSocket object in any way causes it to be returned to its SocketPool.
		/// </summary>
		public void Dispose() {
			socketPool.Return(this);
		}

		/// <summary>
		/// This method closes the underlying stream and socket.
		/// </summary>
		public void Close() {
			if (stream != null) {
				try { stream.Close(); } catch (Exception e) { logger.Error("Error closing stream: " + socketPool.Host, e); }
				stream = null;
			}
			if (socket != null ) {
				try { socket.Shutdown(SocketShutdown.Both); } catch (Exception e) { logger.Error("Error shutting down socket: " + socketPool.Host, e);}
				try { socket.Close(); } catch (Exception e) { logger.Error("Error closing socket: " + socketPool.Host, e);}
				socket = null;
			}
		}

		/// <summary>
		/// Checks if the underlying socket and stream is connected and available.
		/// </summary>
		public bool IsAlive {
			get { return socket != null && socket.Connected && stream.CanRead; }
		}

		/// <summary>
		/// Writes a string to the socket encoded in UTF8 format.
		/// </summary>
		public void Write(string str) {
			Write(Encoding.UTF8.GetBytes(str));
		}

		/// <summary>
		/// Writes an array of bytes to the socket and flushes the stream.
		/// </summary>
		public void Write(byte[] bytes) {
			stream.Write(bytes, 0, bytes.Length);
			stream.Flush();
		}

		/// <summary>
		/// Reads from the socket until the sequence '\r\n' is encountered, 
		/// and returns everything up to but not including that sequence as a UTF8-encoded string
		/// </summary>
		public string ReadLine() {
			MemoryStream buffer = new MemoryStream();
			int b;
			bool gotReturn = false;
			while((b = stream.ReadByte()) != -1) {
				if(gotReturn) {
					if(b == 10) {
						break;
					} else {
						buffer.WriteByte(13);
						gotReturn = false;
					}
				}
				if(b == 13) {
					gotReturn = true;
				} else {
					buffer.WriteByte((byte)b);
				}
			}
			return Encoding.UTF8.GetString(buffer.GetBuffer());
		}

		/// <summary>
		/// Reads a response line from the socket, checks for general memcached errors, and returns the line.
		/// If an error is encountered, this method will throw an exception.
		/// </summary>
		public string ReadResponse() {
			string response = ReadLine();

			if(String.IsNullOrEmpty(response)) {
                throw new DME_MemcachedClientException("Received empty response.");
			}

			if(response.StartsWith("ERROR")
				|| response.StartsWith("CLIENT_ERROR")
				|| response.StartsWith("SERVER_ERROR")) {
                    throw new DME_MemcachedClientException("Server returned " + response);
			}

			return response;
		}

		/// <summary>
		/// Fills the given byte array with data from the socket.
		/// </summary>
		public void Read(byte[] bytes) {
			if(bytes == null) {
				return;
			}

			int readBytes = 0;
			while(readBytes < bytes.Length) {
				readBytes += stream.Read(bytes, readBytes, (bytes.Length - readBytes));
			}
		}

		/// <summary>
		/// Reads from the socket until the sequence '\r\n' is encountered.
		/// </summary>
		public void SkipUntilEndOfLine() {
			int b;
			bool gotReturn = false;
			while((b = stream.ReadByte()) != -1) {
				if(gotReturn) {
					if(b == 10) {
						break;
					} else {
						gotReturn = false;
					}
				}
				if(b == 13) {
					gotReturn = true;
				}
			}
		}

		/// <summary>
		/// Resets this PooledSocket by making sure the incoming buffer of the socket is empty.
		/// If there was any leftover data, this method return true.
		/// </summary>
		public bool Reset() {
			if (socket.Available > 0) {
				byte[] b = new byte[socket.Available];
				Read(b);
				return true;
			}
			return false;
		}
    }
}
