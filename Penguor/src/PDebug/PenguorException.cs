using System;

namespace Penguor.Debugging
{
    public class PenguorException : Exception
    {
        public PenguorException(int msg, int offset, char expected = ' ')
        {
            Debug.CastPGR(msg, offset, expected.ToString());
        }
    }
}