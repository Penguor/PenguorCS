using System;

namespace Penguor.Debugging
{
    public class PenguorCSException : Exception
    {
        public PenguorCSException(int msg, char expected = ' ')
        {
            Debug.CastPGRCS(msg, expected.ToString());
        }
    }
}