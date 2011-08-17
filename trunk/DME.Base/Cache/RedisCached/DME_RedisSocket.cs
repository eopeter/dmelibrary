using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace DME.Base.Cache.RedisCached
{
    public class DME_RedisSocket : IDisposable
    {
        private Socket socket;
        private BufferedStream bstream;
        private string _host = "localhost";
        private int _port = 6379;
        private string _passWord = string.Empty;
        private int _sendTimeout = -1;
        private static object sendLock = new object();
        private static object receiveLock = new object();

        public DME_RedisSocket(string host, int port, string passWord, int sendTimeout)
        {
            _host = host;
            _port = port;
            _passWord = passWord;
            _sendTimeout = sendTimeout;

            Connect();
        }
        #region 属性
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                _host = value;
            }
        }
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }
        public string PassWord
        {
            get
            {
                return _passWord;
            }
            set
            {
                _passWord = value;
            }
        }
        public int SendTimeout
        {
            get
            {
                return _sendTimeout;
            }
            set
            {
                _sendTimeout = value;
            }
        }
        #endregion

        /// <summary>
        /// 连接
        /// </summary>
        private void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.SendTimeout = SendTimeout;
            socket.Connect(Host, Port);
            if (!socket.Connected)
            {
                socket.Close();
                socket = null;
                return;
            }
            bstream = new BufferedStream(new NetworkStream(socket), 16 * 1024);

            if (!string.IsNullOrEmpty(PassWord))
            {
                SendExpectSuccess("AUTH {0}\r\n", PassWord);
            }
        }

        [Conditional("DEBUG")]
        private void Log(string fmt, params object[] args)
        {
            //Console.WriteLine("{0}", String.Format(fmt, args).Trim());
        }


        byte[] end_data = new byte[] { (byte)'\r', (byte)'\n' };
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SendDataCommand(byte[] data, string cmd, params object[] args)
        {
            if (socket == null)
                Connect();
            if (socket == null)
                return false;

            var s = args.Length > 0 ? String.Format(cmd, args) : cmd;
            byte[] r = Encoding.UTF8.GetBytes(s);
            try
            {
                Log("S: " + String.Format(cmd, args));

                lock (sendLock)//发送锁
                {
                    socket.Send(r);
                    if (data != null)
                    {
                        socket.Send(data);
                        socket.Send(end_data);
                    }
                }
            }
            catch (SocketException)
            {
                // timeout;
                socket.Close();
                socket = null;

                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送数据并返回结果
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public byte[] SendExpectData(byte[] data, string cmd, params object[] args)
        {
            byte[] result = null;

            if (!SendDataCommand(data, cmd, args))
                throw new Exception("Unable to connect");

            lock (receiveLock)//接收锁
            {
                result = ReadData();
            }

            return result;
        }

        /// <summary>
        /// 发送数据并返回结果（Int）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SendDataExpectInt(byte[] data, string cmd, params object[] args)
        {
            if (!SendDataCommand(data, cmd, args))
                throw new Exception("Unable to connect");

            int c = 0;
            var s = "";

            lock (receiveLock)//接收锁
            {
                c = bstream.ReadByte();
                if (c == -1)
                    throw new DME_ResponseException("No more data");

                s = ReadLine();
            }

            Log("R: " + s);
            if (c == '-')
                throw new DME_ResponseException(s.StartsWith("ERR") ? s.Substring(4) : s);
            if (c == ':')
            {
                int i;
                if (int.TryParse(s, out i))
                    return i;
            }
            throw new DME_ResponseException("Unknown reply on integer request: " + c + s);
        }

        /// <summary>
        /// 读取返回数据
        /// </summary>
        /// <returns></returns>
        public byte[] ReadData()
        {
            string r = ReadLine();
            Log("R: {0}", r);
            if (r.Length == 0)
                throw new DME_ResponseException("Zero length respose");

            char c = r[0];
            if (c == '-')
                throw new DME_ResponseException(r.StartsWith("-ERR") ? r.Substring(5) : r.Substring(1));
            if (c == '$')
            {
                if (r == "$-1")
                    return null;
                int n;

                if (Int32.TryParse(r.Substring(1), out n))
                {
                    byte[] retbuf = new byte[n];
                    bstream.Read(retbuf, 0, n);
                    if (bstream.ReadByte() != '\r' || bstream.ReadByte() != '\n')
                        throw new DME_ResponseException("Invalid termination");
                    return retbuf;
                }
                throw new DME_ResponseException("Invalid length");
            }

            if (c == '*') //returns the number of matches
            {
                int n;
                if (Int32.TryParse(r.Substring(1), out n))
                {
                    return n <= 0 ? new byte[0] : ReadData();
                }

                throw new DME_ResponseException("Unexpected length parameter" + r);
            }


            throw new DME_ResponseException("Unexpected reply: " + r);
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="bstream"></param>
        /// <returns></returns>
        private string ReadLine()
        {
            var sb = new StringBuilder();
            int c;

            while ((c = bstream.ReadByte()) != -1)
            {
                if (c == '\r')
                    continue;
                if (c == '\n')
                    break;
                sb.Append((char)c);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 发送命令并返回Int
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int SendExpectInt(string cmd, params object[] args)
        {
            if (!SendCommand(cmd, args))
                throw new Exception("Unable to connect");

            int c = 0;
            var s = "";

            lock (receiveLock)//接收锁
            {
                c = bstream.ReadByte();
                if (c == -1)
                    throw new DME_ResponseException("No more data");

                s = ReadLine();
            }
            Log("R: " + s);
            if (c == '-')
                throw new DME_ResponseException(s.StartsWith("ERR") ? s.Substring(4) : s);
            if (c == ':')
            {
                int i;
                if (int.TryParse(s, out i))
                    return i;
            }
            throw new DME_ResponseException("Unknown reply on integer request: " + c + s);
        }

        /// <summary>
        /// 发送命令并返回字符串
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string SendExpectString(string cmd, params object[] args)
        {
            if (!SendCommand(cmd, args))
                throw new Exception("Unable to connect");

            int c = 0;
            var s = "";

            lock (receiveLock)//接收锁
            {
                c = bstream.ReadByte();
                if (c == -1)
                    throw new DME_ResponseException("No more data");

                s = ReadLine();
            }

            Log("R: " + s);
            if (c == '-')
                throw new DME_ResponseException(s.StartsWith("ERR") ? s.Substring(4) : s);
            if (c == '+')
                return s;

            throw new DME_ResponseException("Unknown reply on integer request: " + c + s);
        }

        /// <summary>
        /// 发送命令并返回字符串
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string SendGetString(string cmd, params object[] args)
        {
            if (!SendCommand(cmd, args))
                throw new Exception("Unable to connect");

            string result = null;

            lock (receiveLock)//接收锁
            {
                result = ReadLine();
            }
            return result;
        }

        /// <summary>
        /// 发送数据并返回结果（列表或集合）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public byte[][] SendDataCommandExpectMultiBulkReply(byte[] data, string command, params object[] args)
        {
            if (!SendDataCommand(data, command, args))
                throw new Exception("Unable to connect");
            int c = 0;
            var s = "";

            lock (receiveLock)//接收锁
            {
                c = bstream.ReadByte();
                if (c == -1)
                    throw new DME_ResponseException("No more data");

                s = ReadLine();

                Log("R: " + s);
                if (c == '-')
                    throw new DME_ResponseException(s.StartsWith("ERR") ? s.Substring(4) : s);
                if (c == '*')
                {
                    int count;
                    if (int.TryParse(s, out count))
                    {
                        byte[][] result = new byte[count][];

                        for (int i = 0; i < count; i++)
                            result[i] = ReadData();

                        return result;
                    }
                }
            }
            throw new DME_ResponseException("Unknown reply on multi-request: " + c + s);
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="destKey"></param>
        /// <param name="keys"></param>
        public void StoreSetCommands(string cmd, string destKey, params string[] keys)
        {
            if (String.IsNullOrEmpty(cmd))
                throw new ArgumentNullException("cmd");

            if (String.IsNullOrEmpty(destKey))
                throw new ArgumentNullException("destKey");

            if (keys == null)
                throw new ArgumentNullException("keys");

            SendExpectSuccess("{0} {1} {2}\r\n",
                              cmd,
                              destKey,
                              String.Join(" ", keys)
                             );
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SendCommand(string cmd, params object[] args)
        {
            if (socket == null)
                Connect();
            if (socket == null)
                return false;

            var s = args != null && args.Length > 0 ? String.Format(cmd, args) : cmd;
            byte[] r = Encoding.UTF8.GetBytes(s);
            try
            {
                Log("S: " + String.Format(cmd, args));

                lock (sendLock)//发送锁
                {
                    socket.Send(r);
                }
            }
            catch (SocketException)
            {
                // timeout;
                socket.Close();
                socket = null;

                return false;
            }
            return true;
        }

        /// <summary>
        /// 命令是否正确执行
        /// </summary>
        /// <returns></returns>
        public bool ExpectSuccess()
        {
            int c = 0;
            var s = "";

            lock (receiveLock)//接收锁
            {
                c = bstream.ReadByte();
                if (c == -1)
                    throw new DME_ResponseException("No more data");

                s = ReadLine();
            }

            Log((char)c + s);
            if (c == '-')
                throw new DME_ResponseException(s.StartsWith("ERR") ? s.Substring(4) : s);

            return true;
        }
       
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        public void SendExpectSuccess(string cmd, params object[] args)
        {
            if (!SendCommand(cmd, args))
                throw new Exception("Unable to connect");

            ExpectSuccess();
        }

        #region 释放资源
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~DME_RedisSocket()
        {
            Dispose(false);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SendCommand("QUIT\r\n");
                SafeConnectionClose();
            }
        }
        /// <summary>
        /// 安全关闭连接
        /// </summary>
        private void SafeConnectionClose()
        {
            try
            {
                // workaround for a .net bug: http://support.microsoft.com/kb/821625
                if (bstream != null)
                    bstream.Close();
            }
            catch { }
            try
            {
                if (socket != null)
                    socket.Close();
            }
            catch { }
            bstream = null;
            socket = null;
        }
        #endregion 

    }
}
