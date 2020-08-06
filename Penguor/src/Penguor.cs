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
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;

using Penguor.Compiler.Build;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Tools;

namespace Penguor.Compiler
{
    /// <summary>
    /// The main class and input handler for Penguor
    /// </summary>
    public static class Penguor
    {
        private static async Task<int> Main(params string[] args)
        {
            var rootCommand = new RootCommand("The Penguor Compiler written in C#");

            rootCommand.AddGlobalOption(new Option<string>("--log", "write the log output to a file"));

            // the command used to build files
            var buildCommand = new Command("build", "compile a Penguor file or project");
            buildCommand.AddOption(new Option<string>(new string[] { "--input", "-i" }, "the file or project to build"));
            buildCommand.AddOption(new Option("--benchmark", "when this option is set, the build time will be benchmarked"));
            buildCommand.AddOption(new Option<string>("--transpile", "transpile the input to the specified location"));
            buildCommand.Handler = CommandHandler.Create<string, bool, string, string>(Build);
            rootCommand.AddCommand(buildCommand);

            // with debug builds, the tools command provides access to several developer tools
#if (DEBUG)
            var toolsCommand = new Command("tools", "tools for Penguor Compiler development");

            // generate the AST node files from a txt file
            var ASTGenTool = new Command("generateAST", "generate ast files")
            {
                new Argument<string>("file", "the file to generate the AST files from")
            };
            ASTGenTool.Handler = CommandHandler.Create<string, string>((string file, string log) =>
            {
                if (log != null) Debug.EnableFileLogger(log);
                new ASTPartGenerator().Generate(file);
            });
            toolsCommand.AddCommand(ASTGenTool);

            rootCommand.AddCommand(toolsCommand);
#endif

            return await rootCommand.InvokeAsync(args).ConfigureAwait(false);

            static void Build(string input, bool benchmark, string transpile, string log)
            {
                if (log != null) Debug.EnableFileLogger(log);
                if (input == null) input = Environment.CurrentDirectory;
                Console.WriteLine(transpile);
                if (benchmark) BuildManager.Benchmark(input);
                else BuildManager.SmartBuild(input, transpile ?? "", transpile != null);
            }
        }
    }
}
