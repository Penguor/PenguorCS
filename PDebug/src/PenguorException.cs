using System;

namespace Penguor.Debugging
{
    public class PenguorException : Exception
    {
        public PenguorException(int msg, int line, char expected = ' ')
        {
            Debug.CastPGR(msg, line, expected.ToString());
        }
    }
}