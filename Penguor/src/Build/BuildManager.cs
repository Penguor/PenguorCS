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
using Penguor.Debugging;
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
        /// <param name="file">the file/project to build</param>
        public static void SmartBuild(string file, string output, bool transpile = false)
        {
            if (!File.Exists(file)) throw new PenguorException(5, 0);

            if (!transpile)
            {
                if (Path.GetExtension(file) == ".pgr") BuildFile(file);
                else if (Path.GetExtension(file) == ".pgrp") BuildProject(file);
            }
            else
            {
                if (Path.GetExtension(file) == ".pgr") TranspileFile(file, output);
                else if (Path.GetExtension(file) == ".pgrp") TranspileProject(file, output);

            }
        }

        /// <summary>
        /// build a Penguor project
        /// </summary>
        /// <param name="project">the project file in the project root directory</param>
        public static void BuildProject(string project)
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(project), "*.pgr", SearchOption.AllDirectories);
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
        public static void TranspileProject(string project, string output)
        {
            Uri? basePath = new Uri(Path.GetDirectoryName(project)!);
            string[] files = Directory.GetFiles(Path.GetDirectoryName(project), "*.pgr", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                Uri relativeOut = basePath.MakeRelativeUri(new Uri(file));
                string outputFile = Uri.UnescapeDataString(relativeOut.OriginalString);
                Thread buildThread = new Thread(() => TranspileFile(file, outputFile));
                buildThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public static void TranspileFile(string file, string output)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);

            Builder builder = new Builder(file);
            builder.Build();
            builder.Transpile(TranspileLanguage.CSHARP, output);
        }
    }
}