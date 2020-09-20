/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
# code in method ToUpperCase(this string input)
# has originally been written here:
# https://stackoverflow.com/questions/4135317/make-first-letter-of-a-string-upper-case-with-maximum-performance/27073919#27073919
#
*/

namespace Penguor.Compiler.Tools
{
    internal static class StringExtensions
    {
        public static string ToUppercase(this string input)
        {
            char[] output = input.ToCharArray();
            output[0] = char.ToUpper(output[0]);
            return new string(output);
        }
    }
}
