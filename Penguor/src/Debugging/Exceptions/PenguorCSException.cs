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
        /// initialize a new instance of the PenguorCS exception
        /// </summary>
        /// <param name="msg">the number of the PGRCS error message</param>
        public PenguorCSException(uint msg)
        {
        }
    }
}