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
using System.CommandLine;
using System.CommandLine.Invocation;

using Penguor.Compiler.Build;
using Penguor.Tools;

namespace Penguor
{
    /// <summary>
    /// The main class and input handler for Penguor
    /// </summary>
    public class Penguor
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("The Penguor Compiler written in C#");

            var buildCommand = new Command("build", "compile a Penguor file or project");
            buildCommand.AddOption(new Option<string>(new string[] { "--input", "-i" }, "the file or project to build"));
            buildCommand.AddOption(new Option("--benchmark", "when this option is set, the build time will be benchmarked"));
            buildCommand.AddOption(new Option<string>("--transpile", "transpile the input to the specified location"));
            buildCommand.Handler = CommandHandler.Create<string, bool, string>(Build);
            rootCommand.AddCommand(buildCommand);

#if(DEBUG)
            var toolsCommand = new Command("tools", "tools for Penguor Compiler development");

            var ASTGenTool = new Command("generateAST", "generate ast files")
            {
                new Argument<string>("file", "the file to generate the AST files from")
            };
            ASTGenTool.Handler = CommandHandler.Create<string>((string file) => new ASTPartGenerator().Generate(file));

            toolsCommand.AddCommand(ASTGenTool);

            rootCommand.AddCommand(toolsCommand);
#endif

            return rootCommand.InvokeAsync(args).Result;

            static void Build(string input, bool benchmark, string transpile)
            {
                if (input == null) input = Environment.CurrentDirectory;
                Console.WriteLine(transpile);
                if (benchmark) BuildManager.Benchmark(input);
                else BuildManager.SmartBuild(input, transpile == null ? "" : transpile, transpile != null);
            }
        }
    }
}
