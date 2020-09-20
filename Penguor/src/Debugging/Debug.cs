/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/

using System;
using System.Collections.Generic;

namespace Penguor.Compiler.Debugging
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
        Error,
        /// <summary>
        /// Used for logging without prefix
        /// </summary>
        None
    }

    /// <summary>
    /// Log something
    /// </summary>
    internal static class Debug
    {
        private static FLogger? fLogger;

        private static readonly Dictionary<uint, (LogLevel, string)> pgrcsMessages = new Dictionary<uint, (LogLevel, string)>
        {
            {1, (LogLevel.Error, "An unexpected error occurred")},
            {2, (LogLevel.Warn, "Warning")},
            {3, (LogLevel.Debug, "Debug")},
            {4, (LogLevel.Info, "Info")},
            {5, (LogLevel.Error, "The compiler is out of memory")},
            {6, (LogLevel.Error, "Source file '{0}' not found")},
            {7, (LogLevel.Error, "Parser.tokens is null")},
            {8, (LogLevel.Error, "trying to access active file, but there is none")},
        };

        private static readonly Dictionary<uint, (LogLevel, string)> pgrMessages = new Dictionary<uint, (LogLevel, string)>
        {
            {1, (LogLevel.Error, "An unexpected error occurred")},
            {2, (LogLevel.Warn, "Warning")},
            {3, (LogLevel.Debug, "Debug")},
            {4, (LogLevel.Info, "Info")},
            //! 5 is missing
            {6, (LogLevel.Error, "Expecting '{0}'")},
            {7, (LogLevel.Error, "Unexpected character '{0}'")},
            {8, (LogLevel.Error, "Unknown compiler statement '{0}'")},
            {9, (LogLevel.Error, "Unexpected end of file")}
        };

        /// <summary>
        /// Log a single line of text with the given Log level
        /// </summary>
        /// <param name="logText">The text to log</param>
        /// <param name="logLevel">The loglevel</param>
        public static void Log(string logText, LogLevel logLevel)
        {
            CLogger.Log(logText, logLevel);
            fLogger?.Log(logText, logLevel);
        }

        /// <summary>
        /// log a PenguorCS compiler debug message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void CastPGRCS(uint message, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            (LogLevel level, string debugMessage) = pgrcsMessages.GetValueOrDefault(
                message,
                (LogLevel.Error, "this error is not supposed to exist. Please create an issue at https://github.com/Penguor/PenguorCS"));
            string completeMessage = $"[PGRCS-{string.Format("{0:D4}", message)}] {string.Format(debugMessage, arg0, arg1, arg2, arg3)}";
            Log(completeMessage, level);
        }

        /// <summary>
        /// log a Penguor language debug message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="offset">the offset where the error occurred</param>
        /// <param name="file">the file where the error occurred</param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void CastPGR(uint message, int offset, string? file, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            (LogLevel level, string debugMessage) = pgrMessages.GetValueOrDefault(
                message,
                (LogLevel.Error, "this error is not supposed to exist. Please create an issue at https://github.com/Penguor/PenguorCS"));

            // Format the message to contain error arguments
            string completeMessage =
                $"[PGR-{string.Format("{0:D4}", message)}] {string.Format(debugMessage, arg0, arg1, arg2, arg3)} {(file == null ? "" : GetSourcePosition(offset, file))}";
            Log(completeMessage, level);
        }

        private static string GetSourcePosition(int offset, string file)
        {
            string source;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(file))
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
                if (!gotPos) column++;
            }
            return $"({file}:{line}:{column})";
        }

        public static void EnableFileLogger(string file)
        {
            fLogger = new FLogger(file);
        }
    }
}