using System;
using Penguor.Compiler.Build;

namespace Penguor.Debugging
{
    /// <summary>
    /// cast a new exception to do with a source code error
    /// </summary>
    public class PenguorException : Exception
    {
        /// <summary></summary>
        /// <param name="msg">the number of the PGR error message</param>
        /// <param name="offset">the offset where the error occured in the source file</param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public PenguorException(int msg, int offset, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            Debug.CastPGR(msg, offset, arg0, arg1, arg2, arg3);
        }
    }
}