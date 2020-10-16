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

using Stopwatch = System.Diagnostics.Stopwatch;

using Penguor.Compiler.Debugging;

namespace Penguor.Compiler.Build
{
    /// <summary>
    /// the BuildManager class provides method for building Penguor files and projects
    /// </summary>
    public static class BuildManager
    {
        private static SymbolTableManager tableManager;

        public static SymbolTableManager TableManager
        {
            get => tableManager;
            set => tableManager = value;
        }

        static BuildManager()
        {
            tableManager = new SymbolTableManager();
        }

        /// <summary>
        /// automatically chooses whether to build a project or a file.
        /// </summary>
        /// <param name="path">the file/project to build</param>
        public static void SmartBuild(string path)
        {
            SymbolTableManager manager = new SymbolTableManager();
            if ((File.GetAttributes(path) & FileAttributes.Directory) != 0)
            {
                if (path.Length == 0 || path == null) throw new PenguorCSException(1);
                string[] files = Directory.GetFiles(path, "*.pgrp", SearchOption.AllDirectories);
                if (files.Length > 1) throw new PenguorCSException(1);
                else if (files.Length < 1) throw new PenguorCSException(1);
                else BuildProject(files[0]);
                return;
            }
            if (!File.Exists(path)) throw new PenguorException(5, 0, path, path);

            if (Path.GetExtension(path) == ".pgr") BuildFile(ref manager, path);
            else if (Path.GetExtension(path) == ".pgrp") BuildProject(path);
        }

        public static void BuildProject(string project)
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(project)!, "*.pgr", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                var builder = new Builder(ref tableManager, files[i]);
                builder.Build();
            }
        }

        /// <summary>
        /// build a single file from source
        /// </summary>
        /// <param name="file"></param>
        public static void BuildFile(ref SymbolTableManager manager, string file)
        {
            Builder builder = new Builder(ref manager, file);
            builder.Build();
        }

        /// <summary>
        /// builds a Penguor file and logs the time the individual steps take
        /// </summary>
        /// <param name="file">the file to build</param>
        public static void Benchmark(string file)
        {
            Stopwatch totalWatch = Stopwatch.StartNew();
            // Builder builder = new Builder(file);

            Stopwatch watch = Stopwatch.StartNew();
            // builder.Lex();
            watch.Stop();
            var lexTime = watch.Elapsed;

            watch.Restart();
            // builder.Parse();
            watch.Stop();
            var parseTime = watch.Elapsed;

            watch.Restart();
            //builder.Analyse();
            watch.Stop();
            var analyseTime = watch.Elapsed;
            totalWatch.Stop();
            var totalTime = totalWatch.Elapsed;

            Debug.Log("Results", LogLevel.Info);
            Debug.Log("-------", LogLevel.Info);
            Debug.Log($"file: {file}", LogLevel.Info);
            Debug.Log($"lexing time: {lexTime.Minutes}m  {lexTime.Seconds}s {lexTime.Milliseconds}ms", LogLevel.Info);
            Debug.Log($"parsing time: {parseTime.Minutes}m  {parseTime.Seconds}s {parseTime.Milliseconds}ms", LogLevel.Info);
            Debug.Log($"analysing time: {analyseTime.Minutes}m  {analyseTime.Seconds}s {analyseTime.Milliseconds}ms", LogLevel.Info);
            Debug.Log($"total time: {totalTime.Minutes}m  {totalTime.Seconds}s {totalTime.Milliseconds}ms", LogLevel.Info);
        }
    }
}