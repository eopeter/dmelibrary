using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Base.Cache.MemCached
{
    internal class DME_LogAdapter
    {
        public static DME_LogAdapter GetLogger(Type type) {
			return new DME_LogAdapter(type);
		}

		public static DME_LogAdapter GetLogger(string name) {
			return new DME_LogAdapter(name);
		}

		/*
		 * The problem with logging on the .Net platform is that there is no common logging framework, and 
		 * everyone seems to have their own favorite. We wanted this project to compile straight away
		 * without external dependencies, and we want you to be able to embed it directly into your code,
		 * without having to add references to some other logging framework.
		 * 
		 * Therefore, the MemcachedClient code uses this LogAdapter to add flexible logging.
		 * By default, it is implemented as simple console logging.
		 * 
		 * If you are using log4net, simply comment out the console logging code, uncomment the log4net code,
		 * add the using statement, and make sure your project references log4net.
		 * 
		 * If you are using some other logging framework, feel free to implement your own version of this LogAdapter.
		 */

		//Console Implementation
		private string loggerName;
		private DME_LogAdapter(string name) { loggerName = name; }
        private DME_LogAdapter(Type type) { loggerName = type.FullName; }
		public void Debug(string message) { Console.Out.WriteLine(DateTime.Now + " DEBUG " + loggerName + " - " + message); }
		public void Info(string message) { Console.Out.WriteLine(DateTime.Now + " INFO " + loggerName + " - " + message); }
		public void Warn(string message) { Console.Out.WriteLine(DateTime.Now + " WARN " + loggerName + " - " + message); }
		public void Error(string message) { Console.Out.WriteLine(DateTime.Now + " ERROR " + loggerName + " - " + message); }
		public void Fatal(string message) { Console.Out.WriteLine(DateTime.Now + " FATAL " + loggerName + " - " + message); }
		public void Debug(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " DEBUG " + loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Info(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " INFO " + loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Warn(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " WARN " + loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Error(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " ERROR " + loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Fatal(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " FATAL " + loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }

		//Empty logging Implementation
		/*
		public void Debug(string message) {}
		public void Info(string message) { }
		public void Warn(string message) { }
		public void Error(string message) { }
		public void Fatal(string message) { }
		public void Debug(string message, Exception e) { }
		public void Info(string message, Exception e) { }
		public void Warn(string message, Exception e) { }
		public void Error(string message, Exception e) { }
		public void Fatal(string message, Exception e) { }
		*/

		//Log4net Implementation
		/*
		private log4net.ILog logger;
		private LogAdapter(string name) { logger = log4net.LogManager.GetLogger(name); }
		private LogAdapter(Type type) { logger = log4net.LogManager.GetLogger(type); }
		public void Debug(string message) { logger.Debug(message); }
		public void Info(string message) { logger.Info(message); }
		public void Warn(string message) { logger.Warn(message); }
		public void Error(string message) { logger.Error(message); }
		public void Fatal(string message) { logger.Fatal(message); }
		public void Debug(string message, Exception e) { logger.Debug(message, e); }
		public void Info(string message, Exception e) { logger.Info(message, e); }
		public void Warn(string message, Exception e) { logger.Warn(message, e); }
		public void Error(string message, Exception e) { logger.Error(message, e); }
		public void Fatal(string message, Exception e) { logger.Fatal(message, e); }
		*/
    }
}
