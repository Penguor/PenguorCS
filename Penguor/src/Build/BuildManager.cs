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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Penguor.Compiler.Build
{
    /// <summary>
    /// the BuildManager class provides method for building Penguor files and projects
    /// </summary>
    public static class BuildManager
    {
        private static SymbolTableManager tableManager;
        /// <summary>
        /// A SymbolTableManager for all files being built by this manager
        /// </summary>
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
        /// <param name="stdLib">the path of the standard library</param>
        public static void SmartBuild(string path, string? stdLib)
        {
            if ((File.GetAttributes(path) & FileAttributes.Directory) != 0)
            {
                if (path.Length == 0 || path == null) throw new PenguorCSException(1);
                string[] files = Directory.GetFiles(path, "*.pgrp", SearchOption.AllDirectories);
                if (files.Length > 1) throw new PenguorCSException(1);
                else if (files.Length < 1) throw new PenguorCSException(1);
                else BuildProject(files[0], stdLib);
                return;
            }
            if (!File.Exists(path) || !File.Exists(stdLib))
            {
                Logger.Log(new Notification(path, 0, 10, MsgType.PGR, path));
                Environment.Exit(1);
            }

            if (Path.GetExtension(path) == ".pgr") BuildProject(path, stdLib, true);
            else if (Path.GetExtension(path) == ".pgrp") BuildProject(path, stdLib);
        }

        /// <summary>
        /// build a Penguor project (file ending .pgrp)
        /// </summary>
        /// <param name="project">the path of the project file</param>
        /// <param name="stdLib">the path of the standard library</param>
        /// <param name="singleFile">whether <paramref name="project"/> is a single file</param>
        public static void BuildProject(string project, string? stdLib, bool singleFile = false)
        {
            List<string> files = singleFile ? new(new string[] { project }) : new(Directory.GetFiles(Path.GetDirectoryName(project)!, "*.pgr", SearchOption.AllDirectories));
            if (stdLib != null) files.AddRange(Directory.GetFiles(Path.GetDirectoryName(stdLib)!, "*.pgr", SearchOption.AllDirectories));

            Builder[] builders = new Builder[files.Count];
            for (int i = 0; i < files.Count; i++)
                builders[i] = new Builder(ref tableManager, files[i]);

            foreach (var b in builders)
                b.Parse();
            foreach (var b in builders)
                b.Analyse();
            foreach (var b in builders)
                b.GenerateIR();
            foreach (var b in builders)
                b.GenerateAsm();
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

            Logger.Log("Results", LogLevel.Info);
            Logger.Log("-------", LogLevel.Info);
            Logger.Log($"file: {file}", LogLevel.Info);
            Logger.Log($"lexing time: {lexTime.Minutes}m  {lexTime.Seconds}s {lexTime.Milliseconds}ms", LogLevel.Info);
            Logger.Log($"parsing time: {parseTime.Minutes}m  {parseTime.Seconds}s {parseTime.Milliseconds}ms", LogLevel.Info);
            Logger.Log($"analysing time: {analyseTime.Minutes}m  {analyseTime.Seconds}s {analyseTime.Milliseconds}ms", LogLevel.Info);
            Logger.Log($"total time: {totalTime.Minutes}m  {totalTime.Seconds}s {totalTime.Milliseconds}ms", LogLevel.Info);
        }
    }
}