using System;

namespace Penguor.Debugging
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
        public PenguorCSException(int msg)
        {
            Debug.CastPGRCS(msg);
        }
    }
}