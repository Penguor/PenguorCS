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
using System.IO;
using System.Threading;
using Stopwatch = System.Diagnostics.Stopwatch;
using Penguor.Compiler.Debugging;
using Penguor.Compiler.Transpiling;

namespace Penguor.Compiler.Build
{
    /// <summary>
    /// the BuildManager class provides method for building Penguor files and projects
    /// </summary>
    public static class BuildManager
    {
        /// <summary>
        /// automatically chooses whether to build a project or a file.
        /// </summary>
        /// <param name="path">the file/project to build</param>
        /// <param name="output">where to put output files</param>
        /// <param name="transpile">should the program be transpiled?</param>
        public static void SmartBuild(string path, string output, bool transpile = false)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                if (path == "" || path == null) throw new PenguorCSException(1);
                string[] files = Directory.GetFiles(path, "*.pgrp", SearchOption.AllDirectories);
                if (files.Length > 1) throw new PenguorCSException(1);
                else if (files.Length < 1) throw new PenguorCSException(1);
                else BuildProject(files[0]);
                return;
            }
            if (!File.Exists(path)) throw new PenguorException(5, 0, path, path);

            if (!transpile)
            {
                if (Path.GetExtension(path) == ".pgr") BuildFile(path);
                else if (Path.GetExtension(path) == ".pgrp") BuildProject(path);
            }
            else
            {
                if (Path.GetExtension(path) == ".pgr") TranspileFile(path, output);
                else if (Path.GetExtension(path) == ".pgrp") TranspileProject(path, output);

            }
        }

        /// <summary>
        /// build a Penguor project
        /// </summary>
        /// <param name="project">the project file in the project root directory</param>
        public static void BuildProject(string project)
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(project)!, "*.pgr", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Thread buildThread = new Thread(() => BuildFile(file));
                buildThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public static void BuildFile(string file)
        {
            Builder builder = new Builder(file);
            builder.Build();
        }

        /// <summary>
        /// build a Penguor project
        /// </summary>
        /// <param name="project">the project file in the project root directory</param>
        /// <param name="output">where to write the output file to</param>
        public static void TranspileProject(string project, string output)
        {
            Uri? basePath = new Uri(Path.GetDirectoryName(project)!);

            string[] files = Directory.GetFiles(Path.GetDirectoryName(project)!, "*.pgr", SearchOption.AllDirectories); //!  no null argument check
            foreach (string file in files)
            {
                Uri relativeOut = basePath.MakeRelativeUri(new Uri(file));
                string outputFile = Uri.UnescapeDataString(relativeOut.OriginalString);
                Thread buildThread = new Thread(() => TranspileFile(file, outputFile));
                buildThread.Start();
            }
        }

        /// <summary>
        ///  Transpile a Penguor file to C#
        /// </summary>
        /// <param name="file"></param>
        /// <param name="output"></param>
        public static void TranspileFile(string file, string output)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);

            Builder builder = new Builder(file);
            builder.Build();
            builder.Transpile(TranspileLanguage.CSHARP, output);
        }

        /// <summary>
        /// builds a Penguor file and logs the time the individual steps take
        /// </summary>
        /// <param name="file">the file to build</param>
        public static void Benchmark(string file)
        {
            Stopwatch totalWatch = Stopwatch.StartNew();
            Builder builder = new Builder(file);

            Stopwatch watch = Stopwatch.StartNew();
            builder.Lex();
            watch.Stop();
            var lexTime = watch.Elapsed;

            watch.Restart();
            builder.Parse();
            watch.Stop();
            totalWatch.Stop();
            var parseTime = watch.Elapsed;
            var totalTime = totalWatch.Elapsed;

            Debug.Log("Results", LogLevel.Info);
            Debug.Log("-------", LogLevel.Info);
            Debug.Log($"file: {file}", LogLevel.Info);
            Debug.Log($"lexing time: {lexTime.Minutes}m  {lexTime.Seconds}s {lexTime.Milliseconds}ms", LogLevel.Info);
            Debug.Log($"parsing time: {parseTime.Minutes}m  {parseTime.Seconds}s {parseTime.Milliseconds}ms", LogLevel.Info);
            Debug.Log($"total time: {totalTime.Minutes}m  {totalTime.Seconds}s {totalTime.Milliseconds}ms", LogLevel.Info);
        }
    }
}