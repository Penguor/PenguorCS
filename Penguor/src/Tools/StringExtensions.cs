/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# 
# 
# code in method ToUpperCase(this string input)
# has originally been written here:
# https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance/27073919#27073919
#
*/

#if (DEBUG)

namespace Penguor.Tools
{
    public static class StringExtensions
    {
        public static string ToUppercase(this string input)
        {
            if (string.IsNullOrEmpty(input)) throw new System.ArgumentNullException();
            char[] output = input.ToCharArray();
            output[0] = char.ToUpper(output[0]);
            return new string(output);
        }
    }
}
#endif