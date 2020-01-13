/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/

namespace Penguor.Debugging
{
    /// <summary>
    /// The different logging levels
    /// </summary>
    /// <remarks>
    /// Logging levels are: Debug, Warn, Error
    /// </remarks>
    public enum LogLevel : byte
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
    /// Log something to a file in AppData
    /// </summary>
    public static class Debug
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
            fLogger.Log(logText, logLevel);
        }

        /// <summary>
        /// Log multiple line of text with the given Log level
        /// </summary>
        /// <param name="logText">The lines to log</param>
        /// <param name="logLevel">The loglevel</param>
        public static void Log(string[] logText, LogLevel logLevel)
        {
            fLogger.Log(logText, logLevel);
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
                    Log("[PGRCS-0001] An unexpected error ocurred.", LogLevel.Error);
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
            }
        }

        /// <summary>
        /// log a Penguor language debug message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void CastPGR(int message, int line, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            string currentMessage = "";
            LogLevel level = LogLevel.Debug;
            switch (message)
            {
                case 1:
                    currentMessage = "[PGR-0001] An unexpected error ocurred.";
                    level = LogLevel.Error;
                    break;
                case 2:
                    currentMessage = "[PGR-0002] warning.";
                    level = LogLevel.Warn;
                    break;
                case 3:
                    currentMessage = "[PGR-0003] debug.";
                    level = LogLevel.Debug;
                    break;
                case 4:
                    currentMessage = "[PGR-0004] info.";
                    level = LogLevel.Info;
                    break;
                case 5:
                    currentMessage = "[PGR-0005] Missing header.";
                    level = LogLevel.Error;
                    break;
                case 6:
                    currentMessage = "[PGR-0006] Expecting \"system\", \"component\" or \"datatype\".";
                    level = LogLevel.Error;
                    break;
            }
            currentMessage += $" [line {line}]";

            Log(currentMessage, level);
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
