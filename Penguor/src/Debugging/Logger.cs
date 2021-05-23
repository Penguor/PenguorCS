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
    /// Specifies the type of message of a notification
    /// </summary>
    public enum MsgType
    {
        /// <summary>a message indicating a problem in the source code</summary>
        PGR,
        /// <summary>a message indicating a problem with the compiler</summary>
        PGRCS
    }

    /// <summary>
    /// Log something
    /// </summary>
    internal static class Logger
    {
        private static FLogger? fLogger;

        public static bool Blocked { get; set; }

        private static readonly Dictionary<uint, (LogLevel, string)> pgrcsMessages = new Dictionary<uint, (LogLevel, string)>
        {
            {1, (LogLevel.Error, "An unexpected error occurred")},
            {2, (LogLevel.Warn, "Warning")},
            {3, (LogLevel.Debug, "Debug")},
            {4, (LogLevel.Info, "Info")},
            {5, (LogLevel.Error, "The compiler is out of memory")},
            {6, (LogLevel.Error, "Parser.tokens is null")},
            {7, (LogLevel.Error, "Could not insert symbol into symbol table")},
            {8, (LogLevel.Error, "Trying to access active file, but there is none")},
            {9, (LogLevel.Error, "Token {0} is not valid for binary expressions")},
        };

        private static readonly Dictionary<uint, (LogLevel, string)> pgrMessages = new Dictionary<uint, (LogLevel, string)>
        {
            {1, (LogLevel.Error, "An unexpected error occurred")},
            {2, (LogLevel.Warn, "Warning")},
            {3, (LogLevel.Debug, "Debug")},
            {4, (LogLevel.Info, "Info")},
            {5, (LogLevel.Error, "Expecting a statement terminator: new line or ';'")},
            {6, (LogLevel.Error, "Standard library not found at path '{0}'")},
            {7, (LogLevel.Error, "Unexpected character '{0}'")},
            {8, (LogLevel.Error, "Unknown compiler statement '{0}'")},
            {9, (LogLevel.Error, "Unexpected end of file")},
            {10, (LogLevel.Error, "Source File '{0}' not found")},
            {11, (LogLevel.Error, "Expecting '{0}', but found '{1}'")},
            {12, (LogLevel.Error, "Expecting a code block (with curly braces) or a colon followed by a statement")},
            {13, (LogLevel.Error, "{0}s must not have modifiers")},
            {14, (LogLevel.Error, "Values cannot be assigned to {0}s, expecting a call")},
            {15, (LogLevel.Error, "Only assign expressions and call expressions may be statements")},
            {16, (LogLevel.Error, "Unknown escape sequence '{0}'")},
            {17, (LogLevel.Error, "The contents of {0}s containing more than one statement have to be surrounded by braces")},
            {18, (LogLevel.Warn, "Consider wrapping the contents of this {0} in braces")},
            {19, (LogLevel.Error, "{0} is not accessible from {1}")},
            {20, (LogLevel.Error, "The symbol '{0}' does not exist in the current scope")},
            {21, (LogLevel.Error, "Could not find library '{0}'")},
            {22, (LogLevel.Error, "Postfixed calls may not be chained")},
            {23, (LogLevel.Error, "You may not put statement inside of this block")}
        };

        public static event EventHandler<OnLoggedEventArgs>? OnTryLogged;

        /// <summary>
        /// Log a single line of text with the given Log level
        /// </summary>
        /// <param name="logText">The text to log</param>
        /// <param name="logLevel">The loglevel</param>
        public static void Log(string logText, LogLevel logLevel)
        {
            OnTryLogged?.Invoke(null, new OnLoggedEventArgs(null, logLevel));
            if (Blocked) return;
            CLogger.Log(logText, logLevel);
            fLogger?.Log(logText, logLevel);
        }

        /// <summary>
        /// Log a single line of text with the given Log level
        /// </summary>
        /// <param name="logText">The text to log</param>
        /// <param name="sourceFile">The text to log</param>
        /// <param name="logLevel">The loglevel</param>
        public static void Log(string logText, string sourceFile, LogLevel logLevel)
        {
            OnTryLogged?.Invoke(null, new OnLoggedEventArgs(sourceFile, logLevel));
            if (Blocked) return;
            CLogger.Log(logText, logLevel);
            fLogger?.Log(logText, logLevel);
        }

        public static void Log(Notification notification)
        {
            (LogLevel level, string debugMessage) = notification.Type switch
            {
                MsgType.PGR => pgrMessages.GetValueOrDefault(
                    notification.Message,
                    (LogLevel.Error, "this notification is not supposed to exist. Please create an issue at https://github.com/Penguor/PenguorCS/issues/new/choose")),
                MsgType.PGRCS => pgrcsMessages.GetValueOrDefault(
                    notification.Message,
                    (LogLevel.Error, "this notification is not supposed to exist. Please create an issue at https://github.com/Penguor/PenguorCS/issues/new/choose/issues/new/choose")),
                _ => (LogLevel.Error, "this notification type is not supposed to exist. Please create an issue at https://github.com/Penguor/PenguorCS/issues/new/choose")
            };
            Log(notification.Format(debugMessage), notification.File, level);
        }

        public static void EnableFileLogger(string file)
        {
            fLogger = new FLogger(file);
        }
    }
}