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

namespace Penguor.Compiler.Debugging
{
    /// <summary>
    /// cast a new exception to do with the Penguor Compiler
    /// </summary>
    public class PenguorCSException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="msg">the number of the PGRCS error message</param>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public PenguorCSException(uint msg, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            Debug.CastPGRCS(msg, arg0, arg1, arg2, arg3);
        }
    }
}