
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
        public PenguorCSException()
        {
        }

        /// <inheritdoc/>
        public PenguorCSException(string? message) : base(message)
        {
        }

        /// <inheritdoc/>
        public PenguorCSException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}