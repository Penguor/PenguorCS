/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using Penguor.Build;

namespace Penguor.Debugging
{
    /// <summary>
    /// The different logging levels
    /// </summary>
    /// <remarks>
    /// Logging levels are: Debug, Warn, Error
    /// </remarks>
    internal enum LogLevel : byte
    {
        /// <summary>
        /// Info Level
        /// </summary>
        Info,
        /// <summary>
        /// Debug Level
        /// </summary>
        Debug,
        /// <summary>
        /// Warn Level
        /// </summary>
        Warn,
        /// <summary>
        /// Error Level
        /// </summary>
        Error
    }

    /// <summary>
    /// Log something
    /// </summary>
    internal static class Debug
    {
        private static FLogger fLogger;
        private static CLogger cLogger;

        static Debug()
        {
            fLogger = new FLogger("Penguor");
            cLogger = new CLogger();
        }

        /// <summary>
        /// Log a single line of text with the given Log level
        /// </summary>
        /// <param name="logText">The text to log</param>
        /// <param name="logLevel">The loglevel</param>
        public static void Log(string logText, LogLevel logLevel)
        {
            if (logLevel != LogLevel.Info)
            {
                cLogger.Log(logText, logLevel);
            }
            //fLogger.Log(logText, logLevel);
        }

        /// <summary>
        /// Log multiple line of text with the given Log level
        /// </summary>
        /// <param name="logText">The lines to log</param>
        /// <param name="logLevel">The loglevel</param>
        [System.Obsolete("Currently not working")]
        public static void Log(string[] logText, LogLevel logLevel)
        {
            //fLogger.Log(logText, logLevel);
        }

        /// <summary>
        /// log a PenguorCS compiler debug message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void CastPGRCS(int message, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            switch (message)
            {
                case 1:
                    Log("[PGRCS-0001] An unexpected error occurred.", LogLevel.Error);
                    break;
                case 2:
                    Log("[PGRCS-0002] warning.", LogLevel.Warn);
                    break;
                case 3:
                    Log("[PGRCS-0003] debug.", LogLevel.Error);
                    break;
                case 4:
                    Log("[PGRCS-0004] info.", LogLevel.Error);
                    break;
                case 5:
                    Log("[PGRCS-0005] Out of memory.", LogLevel.Error);
                    break;
                case 6:
                    Log($"[PGRCS-0006] Invalid argument \"{arg0}\"", LogLevel.Error);
                    break;
                case 7:
                    Log($"[PGRCS-0007] \"{arg0}\" expected.", LogLevel.Error);
                    break;
                case 8:
                    Log($"[PGRCS-0008] Parser.tokens equals null", LogLevel.Error);
                    break;
                case 9:
                    Log($"[PGRCS-0009] trying to access active file, but there is none", LogLevel.Error);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("message", message, "This PGRCS message doesn't exist");
            }
        }

        /// <summary>
        /// log a Penguor language debug message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="offset">the offset where the error occurred</param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void CastPGR(int message, int offset, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            string currentMessage = "";
            LogLevel level = LogLevel.Debug;
            switch (message)
            {
                case 1:
                    currentMessage = "[PGR-0001] An unexpected error occurred";
                    level = LogLevel.Error;
                    break;
                case 2:
                    currentMessage = "[PGR-0002] warning";
                    level = LogLevel.Warn;
                    break;
                case 3:
                    currentMessage = "[PGR-0003] debug";
                    level = LogLevel.Debug;
                    break;
                case 4:
                    currentMessage = "[PGR-0004] info";
                    level = LogLevel.Info;
                    break;
                case 5:
                    currentMessage = $"[PGR-0005] Source file '{arg0}' not found";
                    level = LogLevel.Error;
                    break;
                case 6:
                    currentMessage = $"[PGR-0006] Expecting \"{arg0}\"";
                    level = LogLevel.Error;
                    break;
                case 7:
                    currentMessage = $"[PGR-0007] Unexpected char '{arg0}'";
                    level = LogLevel.Error;
                    break;
                case 8:
                    currentMessage = $"[PGR-0008] Unknown preprocessor statement '{arg0}'";
                    level = LogLevel.Error;
                    break;
            }
            currentMessage += " " + GetSourcePosition(offset);
            Builder.ExitCode = message;
            Log(currentMessage, level);
        }

        private static string GetSourcePosition(int offset)
        {
            string source;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(Builder.ActiveFile))
            {
                source = reader.ReadToEnd();
            }
            uint line = 1;
            int column = 1;
            bool gotPos = false;
            for (int i = 0; i < source.Length; i++)
            {
                if (offset == i) gotPos = true;
                if (source[i] == '\n')
                {
                    if (gotPos) break;
                    line++;
                    column = 0;
                    continue;
                }
                else if (source[i] == '\r')
                {
                    if (gotPos) break;
                    i++;
                    line++;
                    column = 0;
                    continue;
                }
                if (!gotPos) column++;
            }
            return $"({Builder.ActiveFile}:{line}:{column})";
        }

        /// <summary>
        /// Stop the logger with a reason
        /// </summary>
        /// <param name="CloseReason">Why should the logger stop</param>
        public static void EndLog(string CloseReason)
        {
            fLogger.EndLog(CloseReason);
        }
    }
}