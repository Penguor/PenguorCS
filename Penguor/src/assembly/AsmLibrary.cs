/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/
#pragma warning disable 1591
// todo: add xml comments

using System.Collections.Generic;

namespace Penguor.ASM
{
    public class Library
    {
        public Library()
        {
            Instructions = new List<Instruction>();
        }

        public string Name { get; set; }
        public List<Instruction> Instructions { get; set; }

        public void Add(Instruction item) => Instructions.Add(item);
    }
}