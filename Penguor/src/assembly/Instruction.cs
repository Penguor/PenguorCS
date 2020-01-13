/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/

namespace Penguor.ASM
{
    /// <summary>
    /// an assembly instruction
    /// </summary>
    public struct Instruction
    {
        private string[] operands;

        /// <summary>
        /// creates a new instruction with the given values
        /// </summary>
        /// <param name="mnemonic"></param>
        public Instruction(string mnemonic)
        {
            Mnemonic = mnemonic;
            operands = new string[3];
        }
        /// <summary>
        /// creates a new instruction with the given values
        /// </summary>
        /// <param name="operands"></param>
        /// <param name="mnemonic"></param>
        public Instruction(string[] operands, string mnemonic)
        {
            Mnemonic = mnemonic;
            this.operands = null;
            Operands = operands;
        }

        /// <summary>
        /// the Mnemonic of the instruction
        /// </summary>
        /// <value></value>
        public string Mnemonic { get; set; }
        /// <summary>
        /// the 3 operands
        /// </summary>
        /// <value></value>
        public string[] Operands
        {
            get => operands;
            set
            {
                if (value.Length > 3) throw new System.Exception();
                else operands = value;
            }
        }
    }
}