using System;
using System.Diagnostics;
using System.IO;
using Microsoft.SPOT;
using NetMf.CommonExtensions;

namespace Utils
{
    public class Logger
    {
        private static string LOGMESSAGE = "{0} {1} - {2}";
        private static StreamWriter logWriter = null;

        public static LogLevel Threshold { get; set; }

        public static void Log(LogLevel level, string message, params object[] args)
        {
            Log(level, StringUtility.Format(message, args));
        }
        
        public static void Log(LogLevel level, string message)
        {
            if (logWriter == null)
            {
                initLogWriter();
            }
            string logMessage = StringUtility.Format(LOGMESSAGE, DateTime.Now.ToString(), level.ToString(), message);

            if (Threshold <= level)
            {
                Log(logMessage);
            }
            if (level == LogLevel.DEBUG)
            {
                ConsoleDebug(message);
            }
        }

        private static void initLogWriter()
        {
            try
            {
                logWriter = new StreamWriter(new FileStream("//SD/log.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 512));
            }
            catch
            {
                //ignore
            }
        }

        private static void Log(string message) {
            if (logWriter != null)
            {
                try
                {
                    logWriter.WriteLine(message);
                }
                catch
                {
                    //ignore
                }
            }
        }

        
        [Conditional("DEBUG")]
        private static void ConsoleDebug(string message)
        {
            Microsoft.SPOT.Debug.Print(message);
        }

        
        public static void Debug(string message) {
            Log(LogLevel.DEBUG, message);
        }

        public static void Debug(string message, params object[] args)
        {
            Log(LogLevel.DEBUG, message, args);
        }

        public static void Info(string message)
        {
            Log(LogLevel.INFO, message);
        }
        public static void Info(string message, params object[] args)
        {
            Log(LogLevel.INFO, message, args);
        }
        public static void Error(string message)
        {
            Log(LogLevel.ERROR, message);
        }
        public static void Error(string message, params object[] args)
        {
            Log(LogLevel.ERROR, message, args);
        }

    }

    public enum LogLevel : byte
    {
        DEBUG = 0,
        INFO = 1,
        ERROR = 2
    }
}

