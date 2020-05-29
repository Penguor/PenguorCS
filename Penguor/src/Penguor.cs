/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
# 
*/

using System;
using System.Collections.Generic;
using Penguor.Debugging;
using Penguor.Build;
using Penguor.Tools;

namespace Penguor
{
    /// <summary>
    /// The main class and input handler for Penguor
    /// </summary>
    public class Penguor
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // print the Penguor info
                Console.WriteLine("Penguor\n\n (c) 2020 Carl Schierig");
                Debug.Log("Penguor info displayed", LogLevel.Info);
            }
            else
            {
                switch (args[0])
                {
                    // TODO: finish help screen
                    case "--help": // display help
                    case "-h":
                        {
                            Debug.Log("Printing help", LogLevel.Info);

                            Console.WriteLine("\nPenguor help");
                            Console.WriteLine("------------\n");
                            Console.WriteLine("    --help, -h: This help");
                            Console.WriteLine("    --build");
                            Console.WriteLine("        <file>: Build a program from a single file");

                            Debug.Log("Printed help", LogLevel.Info);
                            break;
                        }
                    case "-b":
                    case "--build": // build a program from source or Penguor project
                        {
                            Debugging.Debug.Log("Penguor main: build started", LogLevel.Info);

                            if (args.Length == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Usage: Penguor --build [script]");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (args.Length == 2)
                            {
                                // check for grammar argument without folder arg
                                BuildSource(args[1]);
                            }
                            break;
                        }
                    // case "--lex": // build a program from source or Penguor project
                    //     {
                    //         Debug.Log("Penguor main: build started", LogLevel.Info);

                    //         if (args.Length == 1)
                    //         {
                    //             Console.ForegroundColor = ConsoleColor.Yellow;
                    //             Console.WriteLine("Usage: Penguor --build [script]");
                    //             Console.ForegroundColor = ConsoleColor.White;
                    //         }
                    //         else if (args.Length == 2)
                    //         {
                    //             Lexing.Lexer l = new Lexing.Lexer();
                    //             l.Tokenize(args[1]);
                    //         }
                    //         break;
                    //     }
#if (DEBUG)
                    case "--tools":
                        {
                            if (args.Length >= 2)
                            {
                                ASTPartGenerator generator = new ASTPartGenerator();
                                generator.Generate(args[1]);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Usage: Penguor --tools <inputFile>");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            break;
                        }
#endif
                    default:
                        {
                            Debug.CastPGRCS(6, args[0]);
                            goto case "-h";
                        }
                }
            }
        }

        // build program from  source
        static void BuildSource(string file)
        {
            Debugging.Debug.Log("Penguor main: Build process started", LogLevel.Info);
            Builder builder = new Builder();
            builder.BuildFromSource(file);
        }
    }
}
