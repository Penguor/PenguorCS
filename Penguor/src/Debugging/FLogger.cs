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
using System.Globalization;
using System.IO;

namespace Penguor.Debugging
{
    /// <summary>
    /// The <c>Logger</c> class writes logs
    /// to the AppData/ApplicationName/logs folder
    /// </summary>
    internal class FLogger : IDisposable
    {
        // the writer for the log
        private StreamWriter writer;

        public string LogFile { get; }

        /// <param name="file">the name of the application, currently used to choose AppData folder</param>
        public FLogger(string file)
        {
            LogFile = file;

            // Initialize new StreamWriter
            writer = new StreamWriter(file);

            // first textblock for common information
            writer.WriteLine(file);
            for (int i = 0; i < file.Length; i++) writer.Write("-");
            writer.WriteLine();
            writer.WriteLine(DateTime.Now.ToString("F", CultureInfo.InvariantCulture));
            writer.WriteLine();
            writer.Flush();
        }

        /// <summary>
        /// Logs on line of text at four different LogLevels
        /// </summary>
        /// <param name="logText">The string that gets logged</param>
        /// <param name="logLevel">Debug, Warn, Error</param>
        public void Log(string logText, LogLevel logLevel)
        {
            // writes down the time the logger writes sth
            writer.Write("[" + DateTime.Now.ToLongTimeString() + "] ");

            // writes the logging level in front of the actual text
            switch (logLevel)
            {
                case LogLevel.Debug:
                    writer.Write("[Debug] ");
                    break;
                case LogLevel.Info:
                    writer.Write("[Info] ");
                    break;
                case LogLevel.Warn:
                    writer.Write("[Warn] ");
                    break;
                case LogLevel.Error:
                    writer.Write("[Error] ");
                    break;
            }
            writer.WriteLine(logText); // the text that gets be logged
            writer.Flush();
        }

        /// <summary>
        /// Logs multiple lines of text at three different LogLevels
        /// </summary>
        /// <param name="logText">the strings that get logged</param>
        /// <param name="logLevel">Debug, Warn, Error</param>
        public void Log(string[] logText, LogLevel logLevel)
        {
            // writes down the time the logger writes sth
            writer.Write("[" + DateTime.Now.ToLongTimeString() + "] ");

            foreach (string textPart in logText)
            {
                // writes the logging level in front of the actual text
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        writer.Write("[Debug] ");
                        break;
                    case LogLevel.Warn:
                        writer.Write("[Warn] ");
                        break;
                    case LogLevel.Error:
                        writer.Write("[Error] ");
                        break;
                }
                writer.WriteLine(textPart); // the text that gets be logged
            }
        }

        [Obsolete("use Dispose() instead")]
        public void EndLog(string reason) { }

        public void Dispose()
        {
            writer.WriteLine("\nLogger closing");
            writer.WriteLine(DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US"))); // Date at the end

            writer.Flush();
        }
    }
}
