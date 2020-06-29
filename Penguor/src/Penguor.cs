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
using Penguor.Debugging;
using Penguor.Compiler.Build;
using Penguor.Compiler.Parsing;
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
                Console.WriteLine("Penguor\n\n (c) 2019-2020 Carl Schierig");
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

                            if (args.Length == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Usage: Penguor --build [script]");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                            else if (args.Length == 2)
                            {
                                // check for grammar argument without folder arg
                                BuildManager.SmartBuild(args[1], "");
                            }
                            else if (args.Length == 4)
                            {
                                switch (args[2])
                                {
                                    case "--transpile":
                                        BuildManager.SmartBuild(args[1], args[3], true);
                                        break;
                                }
                            }
                            break;
                        }
                    case "--benchmark":
                        if (args.Length == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Usage: Penguor --benchmark [script]");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (args.Length == 2)
                        {
                            BuildManager.Benchmark(args[1]);

                        }
                        break;
#if (DEBUG)
                    case "--lex":
                        if (args.Length == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Usage: Penguor --lex [script]");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (args.Length == 2)
                        {
                            Builder builder = new Builder(args[1]);
                            List<Token> tokens = builder.Lex();
                            foreach (Token token in tokens) Debug.Log(token.ToString(), LogLevel.Debug);
                        }
                        break;
                    case "--tools":
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
#endif
                    default:
                        Debug.CastPGRCS(6, args[0]);
                        goto case "-h";
                }
            }
        }
    }
}
