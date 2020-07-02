using System;
using System.IO;
using System.Threading.Tasks;

using Penguor.Compiler.Build;

namespace Penguor.Compiler.Debugging
{
    /// <summary>
    /// cast a new exception to do with a source code error
    /// </summary>
    public class PenguorException : Exception
    {
        private string? sourceFile;

        public uint Msg { get; }
        public int Offset { get; }

        protected string arg0, arg1, arg2, arg3;

        /// <exception cref="FileNotFoundException"></exception>
        public string? SourceFile
        {
            get { return sourceFile; }
            set
            {
                if (File.Exists(value)) sourceFile = value;
                else throw new FileNotFoundException(null, value);
            }
        }

        public PenguorException(uint msg, int offset, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            Msg = msg;
            Offset = offset;

            this.arg0 = arg0;
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
        }

        /// <summary>
        /// Log the exception
        /// </summary>
        public virtual void Log()
        {
            Debug.CastPGR(Msg, Offset, sourceFile ?? throw new ArgumentNullException(), arg0, arg1, arg2, arg3);
        }

        /// <summary>
        /// Log the exception
        /// </summary>
        public virtual void Log(string file)
        {
            SourceFile = file;
            Debug.CastPGR(Msg, Offset, sourceFile ?? throw new ArgumentNullException(), arg0, arg1, arg2, arg3);
        }
    }
}