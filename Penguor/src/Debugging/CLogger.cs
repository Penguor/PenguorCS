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

namespace Penguor.Compiler.Debugging
{
    internal class CLogger
    {
        public static void Log(string logText, LogLevel logLevel)
        {
            var FGColor = Console.ForegroundColor;
            // writes the logging level in front of the actual text
            switch (logLevel)
            {
#if DEBUG
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("[Debug] ");
                    break;
#endif
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("[Info] ");
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[Warn] ");
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.Write("[Error] ");
                    break;
                case LogLevel.None:
                    break;
            }

            if (logLevel == LogLevel.Error) Console.Error.WriteLine(logText);
            else
#if !DEBUG
            if (logLevel != LogLevel.Debug)
#endif
            {
                Console.WriteLine(logText);
            }

            Console.ForegroundColor = FGColor;
        }
    }
}
