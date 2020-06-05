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
        public static void SmartBuild(string file)
        {
            if (!File.Exists(file)) throw new PenguorException(5, 0);
            if (Path.GetExtension(file) == ".pgr") BuildFile(file);
            else if (Path.GetExtension(file) == ".pgrp") BuildProject(file);
        }

        /// <summary>
        /// build a Penguor project
        /// </summary>
        /// <param name="project">the project file in the project root directory</param>
        public static void BuildProject(string project)
        {
            string[] files = Directory.GetFiles(Path.GetDirectoryName(project), "*.*", SearchOption.AllDirectories);
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
    }
}