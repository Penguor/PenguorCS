using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Penguor.Compiler.Debugging;
using Stopwatch = System.Diagnostics.Stopwatch;

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

        private static bool run;

        static BuildManager()
        {
            tableManager = new SymbolTableManager();
        }

        /// <summary>
        /// automatically chooses whether to build a project or a file.
        /// </summary>
        /// <param name="path">the file/project to build</param>
        /// <param name="stdLib">the path of the standard library</param>
        /// <param name="run">whether the program should be executed after building</param>
        public static void SmartBuild(string path, string? stdLib, bool run = true)
        {
            BuildManager.run = run;
            if (string.IsNullOrEmpty(stdLib)) stdLib = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "stdlib/stdlib.pgrp");

            if ((File.GetAttributes(path) & FileAttributes.Directory) != 0)
            {
                if (path.Length == 0 || path == null) throw new PenguorCSException();
                string[] files = Directory.GetFiles(path, "*.pgrp", SearchOption.AllDirectories);
                if (files.Length > 1) throw new PenguorCSException();
                else if (files.Length < 1) throw new PenguorCSException();
                else BuildProject(files[0], stdLib);
                return;
            }
            if (!File.Exists(path))
            {
                Logger.Log(new Notification(path, 0, 10, MsgType.PGR, path));
                Environment.Exit(1);
            }
            if (!File.Exists(stdLib))
            {
                Logger.Log(new Notification(stdLib, 0, 6, MsgType.PGR, stdLib));
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
            string asm;

            List<string> files = singleFile ? new(new string[] { project }) : new(Directory.GetFiles(Path.GetDirectoryName(project)!, "*.pgr", SearchOption.AllDirectories));
            if (stdLib != null) files.AddRange(Directory.GetFiles(Path.GetDirectoryName(stdLib)!, "*.pgr", SearchOption.AllDirectories));

            Builder[] builders = new Builder[files.Count];
            for (int i = 0; i < files.Count; i++)
                builders[i] = new Builder(ref tableManager, files[i]);

            foreach (var b in builders)
                b.Parse();
            foreach (var b in builders)
                b.Analyse(1);
            foreach (var b in builders)
                b.Analyse(2);
            foreach (var b in builders)
                b.GenerateIR();
            foreach (var b in builders)
                b.GenerateAsm();

            asm = "";

            string buildPath = Path.Combine(Path.GetDirectoryName(project)!, "build");
            Directory.CreateDirectory(buildPath);

            foreach (var b in builders)
                b.Emit(Path.GetDirectoryName(project)!);

            if (OperatingSystem.IsWindows())
            {
                Process.Start("nasm", $" -fwin64 -g {Path.Combine(buildPath, "out.asm")}").WaitForExit();
                Process.Start("gcc", $"{Path.Combine(buildPath, "out.obj")} -o {Path.Combine(buildPath, "out.exe")}").WaitForExit();
                if (run)
                {
                    using Process process = new Process();
                    process.StartInfo.FileName = ".\\build\\out.exe";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    StreamReader reader = process.StandardOutput;

                    string output = reader.ReadToEnd();

                    Console.WriteLine(output);

                    process.WaitForExit();
                }
            }
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