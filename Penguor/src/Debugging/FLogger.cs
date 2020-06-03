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

        /// <value>Gets the filepath of the log</value>
        public string LogPath { get; }

        /// <param name="applicationName">the name of the application, currently used to choose AppData folder</param>
        public FLogger(string applicationName)
        {
            // set the path for the logs folder
            LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName, "logs");

            // create logs directory if non-existent
            if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);

            // Initialize new StreamWriter
            writer = new StreamWriter(Path.Combine(LogPath, DateTime.Now.ToString("dd.MM.yy_HH-mm") + ".log"));

            // first textblock for common information
            writer.WriteLine(applicationName);
            for (int i = 0; i < applicationName.Length; i++) writer.Write("-");
            writer.WriteLine();
            writer.WriteLine(DateTime.Now.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
            writer.WriteLine();
            writer.Flush();
            writer.AutoFlush = true; // makes sure file data gets automatically flushed
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
#if(DEBUG)
                    writer.Write("[Debug] ");
#endif
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
