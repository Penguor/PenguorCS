namespace Penguor.Compiler.Assembly
{
    /// <summary>
    /// a mnemonic for an assembly instruction
    /// </summary>
    public enum AsmMnemonicAmd64
    {
        /// <summary>a label</summary>
        LABEL, // labels unfortunately do need an opcode at the moment
        /// <summary>ASCII Adjust After Addition</summary>
        AAA,
        /// <summary>ASCII Adjust AX Before Division</summary>
        AAD,
        /// <summary>ASCII Adjust AX After Multiply</summary>
        AAM,
        /// <summary>ASCII Adjust AL After Subtraction</summary>
        AAS,
        ///<summary>Add with Carry</summary>
        ADC,
        ///<summary>Unsigned Integer Addition of Two Operands with Carry Flag</summary>
        ADCX,
        ///<summary>Add</summary>
        ADD,
        ///<summary>Add Packed Double-Precision Floating-Point Values</summary>
        ADDPD,
        ///<summary>Add Packed Single-Precision Floating-Point Values</summary>
        ADDPS,
        ///<summary>Add Scalar Double-Precision Floating-Point Values</summary>
        ADDSD,
        ///<summary>Add Scalar Single-Precision Floating-Point Values</summary>
        ADDSS,
        ///<summary>Packed Double-FP Add/Subtract</summary>
        ADDSUBPD,
        ///<summary>Packed Single-FP Add/Subtract</summary>
        ADDSUBPS,
        ///<summary>Unsigned Integer Addition of Two Operands with Overflow Flag</summary>
        ADOX,
        ///<summary>Perform One Round of an AES Decryption Flow</summary>
        AESDEC,
        ///<summary>Perform Last Round of an AES Decryption Flow</summary>
        AESDECLAST,
        ///<summary>Perform One Round of an AES Encryption Flow</summary>
        AESENC,
        ///<summary>Perform Last Round of an AES Encryption Flow</summary>
        AESENCLAST,
        ///<summary>Perform the AES InvMixColumn Transformation</summary>
        AESIMC,
        ///<summary>AES Round Key Generation Assist</summary>
        AESKEYGENASSIST,
        ///<summary>Logical AND</summary>
        AND,
        ///<summary>Logical AND NOT</summary>
        ANDN,
        ///<summary>Bitwise Logical AND of Packed Double Precision Floating-Point Values</summary>
        ANDPD,
        ///<summary>Bitwise Logical AND of Packed Single Precision Floating-Point Values</summary>
        ANDPS,
        ///<summary>Bitwise Logical AND NOT of Packed Double Precision Floating-Point Values</summary>
        ANDNPD,
        ///<summary>Bitwise Logical AND NOT of Packed Single Precision Floating-Point Values</summary>
        ANDNPS,
        ///<summary>Adjust RPL Field of Segment Selector</summary>
        ARPL,
        ///<summary>Bit Field Extract</summary>
        BEXTR,
        ///<summary>Blend Packed Double Precision Floating-Point Values</summary>
        BLENDPD,
        ///<summary>Blend Packed Single Precision Floating-Point Values</summary>
        BLENDPS,
        ///<summary>Variable Blend Packed Double Precision Floating-Point Values</summary>
        BLENDVPD,
        ///<summary>Variable Blend Packed Single Precision Floating-Point Values</summary>
        BLENDVPS,
        ///<summary>Extract Lowest Set Isolated Bit</summary>
        BLSI,
        ///<summary>Get Mask Up to Lowest Set Bit</summary>
        BLSMSK,
        ///<summary>Reset Lowest Set Bit</summary>
        BLSR,
        ///<summary>Check Lower Bound</summary>
        BNDCL,
        ///<summary>Check Upper Bound</summary>
        BNDCU,
        ///<summary>Check Upper Bound</summary>
        BNDCN,
        ///<summary>Load Extended Bounds Using Address Translation</summary>
        BNDLDX,
        ///<summary>Make Bounds</summary>
        BNDMK,
        ///<summary>Move Bounds</summary>
        BNDMOV,
        ///<summary>Store Extended Bounds Using Address Translation</summary>
        BNDSTX,
        ///<summary>Check Array Index Against Bounds</summary>
        BOUND,
        ///<summary>Bit Scan Forward</summary>
        BSF,
        ///<summary>Bit Scan Reverse</summary>
        BSR,
        ///<summary>Byte Swap</summary>
        BSWAP,
        ///<summary>Bit Test</summary>
        BT,
        ///<summary>Bit Test and Complement</summary>
        BTC,
        ///<summary>Bit Test and Reset</summary>
        BTR,
        ///<summary>Bit Test and Set</summary>
        BTS,
        ///<summary>Zero High Bits Starting with Specified Bit Position</summary>
        BZHI,
        ///<summary>Call Procedure</summary>
        CALL,
        ///<summary>Convert Byte to Word</summary>
        CBW,
        ///<summary>Convert Word to Doubleword</summary>
        CWDE,
        ///<summary>Convert Doubleword to Quadword</summary>
        CDQE,
        ///<summary>Clear AC Flag in EFLAGS Register</summary>
        CLAC,
        ///<summary>Clear Carry Flag</summary>
        CLC,
        ///<summary>Clear Direction Flag</summary>
        CLD,
        ///<summary>Cache Line Demote</summary>
        CLDEMOTE,
        ///<summary>Flush Cache Line</summary>
        CLFLUSH,
        ///<summary>Flush Cache Line Optimized</summary>
        CLFLUSHOPT,
        ///<summary>Clear Interrupt Flag</summary>
        CLI,
        ///<summary>Clear Busy Flag in a Supervisor Shadow Stack Token</summary>
        CLRSSBSY,
        ///<summary>Clear Task-Switched Flag in CR0</summary>
        CLTS,
        ///<summary>Cache Line Write Back</summary>
        CLWB,
        ///<summary>Complement Carry Flag</summary>
        CMC,
        ///<summary>Conditional Move</summary>
        CMOVA,
        ///<summary>Conditional Move</summary>
        CMOVAE,
        ///<summary>Conditional Move</summary>
        CMOVB,
        ///<summary>Conditional Move</summary>
        CMOVBE,
        ///<summary>Conditional Move</summary>
        CMOVC,
        ///<summary>Conditional Move</summary>
        CMOVE,
        ///<summary>Conditional Move</summary>
        CMOVGE,
        ///<summary>Conditional Move</summary>
        CMOVL,
        ///<summary>Conditional Move</summary>
        CMOVLE,
        ///<summary>Conditional Move</summary>
        CMOVNA,
        ///<summary>Conditional Move</summary>
        CMOVNAE,
        ///<summary>Conditional Move</summary>
        CMOVNB,
        ///<summary>Conditional Move</summary>
        CMOVNBE,
        ///<summary>Conditional Move</summary>
        CMOVNC,
        ///<summary>Conditional Move</summary>
        CMOVNE,
        ///<summary>Conditional Move</summary>
        CMOVNG,
        ///<summary>Conditional Move</summary>
        CMOVNGE,
        ///<summary>Conditional Move</summary>
        CMOVNL,
        ///<summary>Conditional Move</summary>
        CMOVNLE,
        ///<summary>Conditional Move</summary>
        CMOVNO,
        ///<summary>Conditional Move</summary>
        CMOVNP,
        ///<summary>Conditional Move</summary>
        CMOVNS,
        ///<summary>Conditional Move</summary>
        CMOVNZ,
        ///<summary>Conditional Move</summary>
        CMOVO,
        ///<summary>Conditional Move</summary>
        CMOVP,
        ///<summary>Conditional Move</summary>
        CMOVPE,
        ///<summary>Conditional Move</summary>
        CMOVPO,
        ///<summary>Conditional Move</summary>
        CMOVS,
        ///<summary>Conditional Move</summary>
        CMOVZ,
        ///<summary>Compare Two Operands</summary>
        CMP,
        ///<summary>Compare Packed Double-Precision Floating-Point Values</summary>
        CMPPD,
        ///<summary>Compare Packed Single-Precision Floating-Point Values</summary>
        CMPPS,
        ///<summary>Compare String Operands</summary>
        CMPS,
        ///<summary>Compare String Operands</summary>
        CMPSB,
        ///<summary>Compare String Operands</summary>
        CMPSW,
        //todo: look this up, defined twice
        ///<summary>Compare String Operands/Compare Scalar Double-Precision Floating-Point Value</summary>
        CMPSD,
        ///<summary>Compare String Operands</summary>
        CMPSQ,
        ///<summary>Compare Scalar Single-Precision Floating-Point Value</summary>
        CMPSS,
        ///<summary>Compare and Exchange</summary>
        CMPXCHG,
        ///<summary>Compare and Exchange Bytes</summary>
        CMPXCHG8B,
        ///<summary>Compare and Exchange Bytes</summary>
        CMPXCHG16B,
        ///<summary>Compare Scalar Ordered Double-Precision Floating-Point Values and Set EFLAGS</summary>
        COMISD,
        ///<summary>Compare Scalar Ordered Single-Precision Floating-Point Values and Set EFLAGS</summary>
        COMISS,
        ///<summary>CPU Identification</summary>
        CPUID,
        ///<summary>Accumulate CRC32 Value</summary>
        CRC32,
        ///<summary>Convert Packed Doubleword Integers to Packed Double-Precision Floating-Point Values</summary>
        CVTDQ2PD,
        ///<summary>Convert Packed Doubleword Integers to Packed Single-Precision Floating-Point Values</summary>
        CVTDQ2PS,
        ///<summary>Convert Packed Double-Precision Floating-Point Values to Packed Doubleword Integers</summary>
        CVTPD2DQ,
        ///<summary>Convert Packed Double-Precision FP Values to Packed Dword Integers</summary>
        CVTPD2PI,
        ///<summary>Convert Packed Double-Precision Floating-Point Values to Packed Single-Precision Floating-Point Values</summary>
        CVTPD2PS,
        ///<summary>Convert Packed Dword Integers to Packed Double-Precision FP Values</summary>
        CVTPI2PD,
        ///<summary>Convert Packed Dword Integers to Packed Single-Precision FP Values</summary>
        CVTPI2PS,
        ///<summary>Convert Packed Single-Precision Floating-Point Values to Packed Signed Doubleword Integer Values</summary>
        CVTPS2DQ,
        ///<summary>Convert Packed Single-Precision Floating-Point Values to Packed Double-Precision Floating-Point Values</summary>
        CVTPS2PD,
        ///<summary>Convert Packed Single-Precision FP Values to Packed Dword Integers</summary>
        CVTPS2PI,
        ///<summary>Convert Scalar Double-Precision Floating-Point Value to Doubleword Integer</summary>
        CVTSD2SI,
        ///<summary>Convert Scalar Double-Precision Floating-Point Value to Scalar Single-Precision Floating-Point Value</summary>
        CVTSD2SS,
        ///<summary>Convert Doubleword Integer to Scalar Double-Precision Floating-Point Value</summary>
        CVTSI2SD,
        ///<summary>Convert Doubleword Integer to Scalar Single-Precision Floating-Point Value</summary>
        CVTSI2SS,
        ///<summary>Convert Scalar Single-Precision Floating-Point Value to Scalar Double-Precision Floating-Point Value</summary>
        CVTSS2SD,
        ///<summary>Convert Scalar Single-Precision Floating-Point Value to Doubleword Integer</summary>
        CVTSS2SI,
        ///<summary>Convert with Truncation Packed Double-Precision Floating-Point Values to Packed Doubleword Integers</summary>
        CVTTPD2DQ,
        ///<summary>Convert with Truncation Packed Double-Precision FP Values to Packed Dword Integers</summary>
        CVTTPD2PI,
        ///<summary>Convert with Truncation Packed Single-Precision Floating-Point Values to Packed Signed Doubleword Integer Values</summary>
        CVTTPS2DQ,
        ///<summary>Convert with Truncation Packed Single-Precision FP Values to Packed Dword Integers</summary>
        CVTTPS2PI,
        ///<summary>Convert with Truncation Scalar Double-Precision Floating-Point Value to Signed Integer</summary>
        CVTTSD2SI,
        ///<summary>Convert with Truncation Scalar Single-Precision Floating-Point Value to Integer</summary>
        CVTTSS2SI,
        ///<summary>Convert Word to Doubleword</summary>
        CWD,
        ///<summary>Convert Doubleword to Quadword</summary>
        CDQ,
        ///<summary>Convert Doubleword to Quadword</summary>
        CQO,
        ///<summary>Decimal Adjust AL after Addition</summary>
        DAA,
        ///<summary>Decimal Adjust AL after Subtraction</summary>
        DAS,
        ///<summary>Decrement by 1</summary>
        DEC,
        ///<summary>Unsigned Divide</summary>
        DIV,
        ///<summary>Divide Packed Double-Precision Floating-Point Values</summary>
        DIVPD,
        ///<summary>Divide Packed Single-Precision Floating-Point Values</summary>
        DIVPS,
        ///<summary>Divide Scalar Double-Precision Floating-Point Value</summary>
        DIVSD,
        ///<summary>Divide Scalar Single-Precision Floating-Point Values</summary>
        DIVSS,
        ///<summary>Dot Product of Packed Double Precision Floating-Point Values</summary>
        DPPD,
        ///<summary>Dot Product of Packed Single Precision Floating-Point Values</summary>
        DPPS,
        ///<summary>Empty MMX Technology State</summary>
        EMMS,
        ///<summary>Terminate an Indirect Branch in 32-bit and Compatibility Mode</summary>
        ENDBR32,
        ///<summary>Terminate an Indirect Branch in 64-bit Mode</summary>
        ENDBR64,
        ///<summary>Make Stack Frame for Procedure Parameters</summary>
        ENTER,
        ///<summary>Extract Packed Floating-Point Values</summary>
        EXTRACTPS,
        ///<summary>Compute 2^x–1</summary>
        F2XM1,
        ///<summary>Absolute Value</summary>
        FABS,
        ///<summary>Add</summary>
        FADD,
        ///<summary>Add</summary>
        FADDP,
        ///<summary>Add</summary>
        FIADD,
        ///<summary>Load Binary Coded Decimal</summary>
        FBLD,
        ///<summary>Store BCD Integer and Pop</summary>
        FBSTP,
        ///<summary>Change Sign</summary>
        FCHS,
        ///<summary>Clear Exceptions</summary>
        FCLEX,
        ///<summary>Clear Exceptions</summary>
        FNCLEX,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVB,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVE,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVBE,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVU,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVNB,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVNE,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVNBE,
        ///<summary>Floating-Point Conditional Move</summary>
        FCMOVNU,
        ///<summary>Compare Floating Point Values</summary>
        FCOM,
        ///<summary>Compare Floating Point Values</summary>
        FOMP,
        ///<summary>Compare Floating Point Values</summary>
        FCOMPP,
        ///<summary>Compare Floating Point Values and Set EFLAGS</summary>
        FCOMI,
        ///<summary>Compare Floating Point Values and Set EFLAGS</summary>
        FCOMIP,
        ///<summary>Compare Floating Point Values and Set EFLAGS</summary>
        FUCOMI,
        ///<summary>Compare Floating Point Values and Set EFLAGS</summary>
        FUCOMIP,
        ///<summary>Cosine</summary>
        FCOS,
        ///<summary>Decrement Stack-Top Pointer</summary>
        FDECSTP,
        ///<summary>Divide</summary>
        FDIV,
        ///<summary>Divide</summary>
        FDIVP,
        ///<summary>Divide</summary>
        FIDIV,
        ///<summary>Reverse Divide</summary>
        FDIVR,
        ///<summary>Reverse Divide</summary>
        FDIVRP,
        ///<summary>Reverse Divide</summary>
        FIDIVR,
        ///<summary>Free Floating-Point Register</summary>
        FFREE,
        ///<summary>Compare Integer</summary>
        FICOM,
        ///<summary>Compare Integer</summary>
        FICOMP,
        ///<summary>Load Integer</summary>
        FILD,
        ///<summary>Increment Stack-Top Pointer</summary>
        FINCSTP,
        ///<summary>Initialize Floating-Point Unit</summary>
        FINIT,
        ///<summary>Initialize Floating-Point Unit</summary>
        FNINIT,
        ///<summary>Store Integer</summary>
        FIST,
        ///<summary>Store Integer</summary>
        FISTP,
        ///<summary>Store Integer with Truncation</summary>
        FISTTP,
        ///<summary>Load Floating Point Value</summary>
        FLD,
        ///<summary>Load Constant</summary>
        FLD1,
        ///<summary>Load Constant</summary>
        FLD2T,
        ///<summary>Load Constant</summary>
        FLD2E,
        ///<summary>Load Constant</summary>
        FLDPI,
        ///<summary>Load Constant</summary>
        FLDLG2,
        ///<summary>Load Constant</summary>
        FLDN2,
        ///<summary>Load Constant</summary>
        FLDZ,
        ///<summary>Load x87 FPU Control Word</summary>
        FLDCW,
        ///<summary>Load x87 FPU Environment</summary>
        FLDENV,
        ///<summary>Multiply</summary>
        FMUL,
        ///<summary>Multiply</summary>
        FMULP,
        ///<summary>Multiply</summary>
        FIMUL,
        ///<summary>No Operation</summary>
        FNOP,
        ///<summary>Partial Arctangent</summary>
        FPATAN,
        ///<summary>Partial Remainder</summary>
        FPREM,
        ///<summary>Partial Remainder</summary>
        FPREM1,
        ///<summary>Partial Tangent</summary>
        FPTAN,
        ///<summary>Round to Integer</summary>
        FRNDINT,
        ///<summary>Restore x87 FPU State</summary>
        FRSTOR,
        ///<summary>Store x87 FPU State</summary>
        FSAVE,
        ///<summary>Store x87 FPU State</summary>
        FNSAVE,
        ///<summary>Scale</summary>
        FSCALE,
        ///<summary>Sine</summary>
        FSIN,
        ///<summary>Sine and Cosine</summary>
        FSINCOS,
        ///<summary>Square Root</summary>
        FSQRT,
        ///<summary>Store Floating Point Value</summary>
        FST,
        ///<summary>Store Floating Point Value</summary>
        FSTP,
        ///<summary>Store x87 FPU Control Word</summary>
        FSTCW,
        ///<summary>Store x87 FPU Control Word</summary>
        FNSTCW,
        ///<summary>Store x87 FPU Environment</summary>
        FSTENV,
        ///<summary>Store x87 FPU Environment</summary>
        FNSTENV,
        ///<summary>Store x87 FPU Status Word</summary>
        FSTSW,
        ///<summary>Store x87 FPU Status Word</summary>
        FNSTSW,
        ///<summary>Subtract</summary>
        FSUB,
        ///<summary>Subtract</summary>
        FSUBP,
        ///<summary>Subtract</summary>
        FISUB,
        ///<summary>Reverse Subtract</summary>
        FSUBR,
        ///<summary>Reverse Subtract</summary>
        FSUBRP,
        ///<summary>Reverse Subtract</summary>
        FISUBR,
        ///<summary>TEST</summary>
        FTST,
        ///<summary>Unordered Compare Floating Point Values</summary>
        FUCOM,
        ///<summary>Unordered Compare Floating Point Values</summary>
        FUCOMP,
        ///<summary>Unordered Compare Floating Point Values</summary>
        FUCOMPP,
        ///<summary>Examine Floating-Point</summary>
        FXAM,
        ///<summary>Exchange Register Contents</summary>
        FXCH,
        ///<summary>Restore x87 FPU, MMX, XMM, and MXCSR State</summary>
        FXRSTOR,
        ///<summary>Save x87 FPU, MMX Technology, and SSE State</summary>
        FXSAVE,
        ///<summary>Extract Exponent and Significand</summary>
        FXTRACT,
        ///<summary>Compute y ∗ log2(x)</summary>
        FYL2X,
        ///<summary>Compute y ∗ log2(x +1)</summary>
        FYL2XP1,
        ///<summary>Galois Field Affine Transformation Inverse</summary>
        GF2P8AFFINEINVQB,
        ///<summary>Galois Field Affine Transformation</summary>
        GF2P8AFFINEQB,
        ///<summary>Galois Field Multiply Bytes</summary>
        GF2P8MULB,
        ///<summary>Packed Double-FP Horizontal Add</summary>
        HADDPD,
        ///<summary>Packed Single-FP Horizontal Add</summary>
        HADDPS,
        ///<summary>Packed Double-FP Horizontal Subtract</summary>
        HSUBPD,
        ///<summary>Packed Single-FP Horizontal Subtract</summary>
        HSUBPS,
        ///<summary>Signed Divide</summary>
        IDIV,
        ///<summary>Signed Multiply</summary>
        IMUL,
        ///<summary>Input from Port</summary>
        IN,
        ///<summary>Increment by 1</summary>
        INC,
        ///<summary>Increment Shadow Stack Pointer</summary>
        INCSSPD,
        ///<summary>Increment Shadow Stack Pointer</summary>
        INCSSPQ,
        ///<summary>Input from Port to String</summary>
        INS,
        ///<summary>Input from Port to String</summary>
        INSB,
        ///<summary>Input from Port to String</summary>
        INSW,
        ///<summary>Input from Port to String</summary>
        INSD,
        ///<summary>Insert Scalar Single-Precision Floating-Point Value</summary>
        INSERTPS,
        ///<summary>Call to Interrupt Procedure</summary>
        INT,
        ///<summary>Call to Interrupt Procedure</summary>
        INTO,
        ///<summary>Call to Interrupt Procedure</summary>
        INT1,
        ///<summary>Call to Interrupt Procedure</summary>
        INT3,
        ///<summary>Invalidate Internal Caches</summary>
        INVD,
        ///<summary>Invalidate TLB Entries</summary>
        INVLPG,
        ///<summary>Invalidate Process-Context Identifier</summary>
        INVPCID,
        ///<summary>Interrupt Return</summary>
        IRET,
        ///<summary>Interrupt Return</summary>
        IRETD,
        ///<summary>Interrupt Return</summary>
        IRETQ,
        ///<summary>Jump if Condition Is Met</summary>
        JA,
        ///<summary>Jump if Condition Is Met</summary>
        JAE,
        ///<summary>Jump if Condition Is Met</summary>
        JB,
        ///<summary>Jump if Condition Is Met</summary>
        JBE,
        ///<summary>Jump if Condition Is Met</summary>
        JC,
        ///<summary>Jump if Condition Is Met</summary>
        JCXZ,
        ///<summary>Jump if Condition Is Met</summary>
        JECXZ,
        ///<summary>Jump if Condition Is Met</summary>
        JRCXZ,
        ///<summary>Jump if Condition Is Met</summary>
        JE,
        ///<summary>Jump if Condition Is Met</summary>
        JG,
        ///<summary>Jump if Condition Is Met</summary>
        JGE,
        ///<summary>Jump if Condition Is Met</summary>
        JL,
        ///<summary>Jump if Condition Is Met</summary>
        JLE,
        ///<summary>Jump if Condition Is Met</summary>
        JNA,
        ///<summary>Jump if Condition Is Met</summary>
        JNAE,
        ///<summary>Jump if Condition Is Met</summary>
        JNB,
        ///<summary>Jump if Condition Is Met</summary>
        JNBE,
        ///<summary>Jump if Condition Is Met</summary>
        JNC,
        ///<summary>Jump if Condition Is Met</summary>
        JNE,
        ///<summary>Jump if Condition Is Met</summary>
        JNG,
        ///<summary>Jump if Condition Is Met</summary>
        JNGE,
        ///<summary>Jump if Condition Is Met</summary>
        JNL,
        ///<summary>Jump if Condition Is Met</summary>
        JNLE,
        ///<summary>Jump if Condition Is Met</summary>
        JNO,
        ///<summary>Jump if Condition Is Met</summary>
        JNP,
        ///<summary>Jump if Condition Is Met</summary>
        JNS,
        ///<summary>Jump if Condition Is Met</summary>
        JNZ,
        ///<summary>Jump if Condition Is Met</summary>
        JO,
        ///<summary>Jump if Condition Is Met</summary>
        JP,
        ///<summary>Jump if Condition Is Met</summary>
        JPE,
        ///<summary>Jump if Condition Is Met</summary>
        JPO,
        ///<summary>Jump if Condition Is Met</summary>
        JS,
        ///<summary>Jump if Condition Is Met</summary>
        JZ,
        ///<summary>Jump</summary>
        JMP,
        ///<summary>ADD Two Masks</summary>
        KADDW,
        ///<summary>ADD Two Masks</summary>
        KADDB,
        ///<summary>ADD Two Masks</summary>
        KADDQ,
        ///<summary>Bitwise Logical AND Masks</summary>
        KADDD,
        ///<summary>Bitwise Logical AND Masks</summary>
        KANDW,
        ///<summary>Bitwise Logical AND Masks</summary>
        KANDB,
        ///<summary>Bitwise Logical AND Masks</summary>
        KANDQ,
        ///<summary>Bitwise Logical AND Masks</summary>
        KANDD,
        ///<summary>Bitwise Logical AND NOT Masks</summary>
        KANDNW,
        ///<summary>Bitwise Logical AND NOT Masks</summary>
        KANDNB,
        ///<summary>Bitwise Logical AND NOT Masks</summary>
        KANDNQ,
        ///<summary>Bitwise Logical AND NOT Masks</summary>
        KANDND,
        ///<summary>Move from and to Mask Registers</summary>
        KMOVW,
        ///<summary>Move from and to Mask Registers</summary>
        KMOVB,
        ///<summary>Move from and to Mask Registers</summary>
        KMOVQ,
        ///<summary>Move from and to Mask Registers</summary>
        KMOVD,
        ///<summary>NOT Mask Register</summary>
        KNOTW,
        ///<summary>NOT Mask Register</summary>
        KNOTB,
        ///<summary>NOT Mask Register</summary>
        KNOTQ,
        ///<summary>NOT Mask Register</summary>
        KNOTD,
        ///<summary>Bitwise Logical OR Masks</summary>
        KORW,
        ///<summary>Bitwise Logical OR Masks</summary>
        KORB,
        ///<summary>Bitwise Logical OR Masks</summary>
        KORQ,
        ///<summary>Bitwise Logical OR Masks</summary>
        KORD,
        ///<summary>OR Masks And Set Flags</summary>
        KORTESTW,
        ///<summary>OR Masks And Set Flags</summary>
        KORTESTB,
        ///<summary>OR Masks And Set Flags</summary>
        KORTESTQ,
        ///<summary>OR Masks And Set Flags</summary>
        KORTESTD,
        ///<summary>Shift Left Mask Registers</summary>
        KSHIFTLW,
        ///<summary>Shift Left Mask Registers</summary>
        KSHIFTLB,
        ///<summary>Shift Left Mask Registers</summary>
        KSHIFTLQ,
        ///<summary>Shift Left Mask Registers</summary>
        KSHIFTLD,
        ///<summary>Shift Right Mask Registers</summary>
        KSHIFTRW,
        ///<summary>Shift Right Mask Registers</summary>
        KSHIFTRB,
        ///<summary>Shift Right Mask Registers</summary>
        KSHIFTRQ,
        ///<summary>Shift Right Mask Registers</summary>
        KSHIFTRD,
        ///<summary>Packed Bit Test Masks and Set Flags</summary>
        KTESTW,
        ///<summary>Packed Bit Test Masks and Set Flags</summary>
        KTESTB,
        ///<summary>Packed Bit Test Masks and Set Flags</summary>
        KTESTQ,
        ///<summary>Packed Bit Test Masks and Set Flags</summary>
        KTESTD,
        ///<summary>Unpack for Mask Registers</summary>
        KUNPCKBW,
        ///<summary>Unpack for Mask Registers</summary>
        KUNPCKBD,
        ///<summary>Unpack for Mask Registers</summary>
        KUNPCKBQ,
        ///<summary>Bitwise Logical XNOR Masks</summary>
        KXNORW,
        ///<summary>Bitwise Logical XNOR Masks</summary>
        KXNORB,
        ///<summary>Bitwise Logical XNOR Masks</summary>
        KXNORQ,
        ///<summary>Bitwise Logical XNOR Masks</summary>
        KXNORD,
        ///<summary>Bitwise Logical XOR Masks</summary>
        KXORW,
        ///<summary>Bitwise Logical XOR Masks</summary>
        KXORB,
        ///<summary>Bitwise Logical XOR Masks</summary>
        KXORQ,
        ///<summary>Bitwise Logical XOR Masks</summary>
        KXORD,
        ///<summary>Load Status Flags into AH Register</summary>
        LAHF,
        ///<summary>Load Access Rights Byte</summary>
        LAR,
        ///<summary>Load Unaligned Integer 128 Bits</summary>
        LDDQU,
        ///<summary>Load MXCSR Register</summary>
        LDMXCSR,
        ///<summary>Load Far Pointer</summary>
        LDS,
        ///<summary>Load Far Pointer</summary>
        LES,
        ///<summary>Load Far Pointer</summary>
        LFS,
        ///<summary>Load Far Pointer</summary>
        LGS,
        ///<summary>Load Far Pointer</summary>
        LSS,
        ///<summary>Load Effective Address</summary>
        LEA,
        ///<summary>High Level Procedure Exit</summary>
        LEAVE,
        ///<summary>Load Fence</summary>
        LFENCE,
        ///<summary>Load Global/Interrupt Descriptor Table Register</summary>
        LGDT,
        ///<summary>Load Global/Interrupt Descriptor Table Register</summary>
        LIDT,
        ///<summary>Load Local Descriptor Table Register</summary>
        LLDT,
        ///<summary>Load Machine Status Word</summary>
        LMSW,
        ///<summary>Assert LOCK# Signal Prefix</summary>
        LOCK,
        ///<summary>Load String</summary>
        LODS,
        ///<summary>Load String</summary>
        LODSB,
        ///<summary>Load String</summary>
        LODSW,
        ///<summary>Load String</summary>
        LODSD,
        ///<summary>Load String</summary>
        LODSQ,
        ///<summary>Loop According to ECX Counter</summary>
        LOOP,
        ///<summary>Loop According to ECX Counter</summary>
        LOOPE,
        ///<summary>Loop According to ECX Counter</summary>
        LOOPNE,
        ///<summary>Load Segment Limit</summary>
        LSL,
        ///<summary>Load Task Register</summary>
        LTR,
        ///<summary>Count the Number of Leading Zero Bits</summary>
        LZCNT,
        ///<summary>Store Selected Bytes of Double Quadword</summary>
        MASKMOVDQU,
        ///<summary>Store Selected Bytes of Quadword</summary>
        MASKMOVQ,
        ///<summary>Maximum of Packed Double-Precision Floating-Point Values</summary>
        MAXPD,
        ///<summary>Maximum of Packed Single-Precision Floating-Point Values</summary>
        MAXPS,
        ///<summary>Return Maximum Scalar Double-Precision Floating-Point Value</summary>
        MAXSD,
        ///<summary>Return Maximum Scalar Single-Precision Floating-Point Value</summary>
        MAXSS,
        ///<summary>Memory Fence</summary>
        MFENCE,
        ///<summary>Minimum of Packed Double-Precision Floating-Point Values</summary>
        MINPD,
        ///<summary>Minimum of Packed Single-Precision Floating-Point Values</summary>
        MINPS,
        ///<summary>Return Minimum Scalar Double-Precision Floating-Point Value</summary>
        MINSD,
        ///<summary>Return Minimum Scalar Single-Precision Floating-Point Value</summary>
        MINSS,
        ///<summary>Set Up Monitor Address</summary>
        MONITOR,
        ///<summary>Move</summary>
        MOV,
        ///<summary>Move Aligned Packed Double-Precision Floating-Point Values</summary>
        MOVAPD,
        ///<summary>Move Aligned Packed Single-Precision Floating-Point Values</summary>
        MOVAPS,
        ///<summary>Move Data After Swapping Bytes</summary>
        MOVBE,
        ///<summary>Move Doubleword</summary>
        MOVD,
        ///<summary>Move Quadword</summary>
        MOVQ,
        ///<summary>Replicate Double FP Values</summary>
        MOVDDUP,
        ///<summary>Move Doubleword as Direct Store</summary>
        MOVDIRI,
        ///<summary>Move 64 Bytes as Direct Store</summary>
        MOVDIR64B,
        ///<summary>Move Aligned Packed Integer Values</summary>
        MOVDQA,
        ///<summary>Move Aligned Packed Integer Values</summary>
        VMOVDQA,
        ///<summary>Move Aligned Packed Integer Values</summary>
        VMOVDQA32,
        ///<summary>Move Aligned Packed Integer Values</summary>
        VMOVDQA64,
        ///<summary>Move Unaligned Packed Integer Values</summary>
        MOVDQU,
        ///<summary>Move Unaligned Packed Integer Values</summary>
        VMOVDQU,
        ///<summary>Move Unaligned Packed Integer Values</summary>
        VMOVDQU8,
        ///<summary>Move Unaligned Packed Integer Values</summary>
        VMOVDQU16,
        ///<summary>Move Unaligned Packed Integer Values</summary>
        VMOVDQU32,
        ///<summary>Move Unaligned Packed Integer Values</summary>
        VMOVDQU64,
        ///<summary>Move Quadword from XMM to MMX Technology Register</summary>
        MOVDQ2Q,
        ///<summary>Move Packed Single-Precision Floating-Point Values High to Low</summary>
        MOVHLPS,
        ///<summary>Move High Packed Double-Precision Floating-Point Value</summary>
        MOVHPD,
        ///<summary>Move High Packed Single-Precision Floating-Point Values</summary>
        MOVHPS,
        ///<summary>Move Packed Single-Precision Floating-Point Values Low to High</summary>
        MOVLHPS,
        ///<summary>Move Low Packed Double-Precision Floating-Point Value</summary>
        MOVLPD,
        ///<summary>Move Low Packed Single-Precision Floating-Point Values</summary>
        MOVLPS,
        ///<summary>Extract Packed Double-Precision Floating-Point Sign Mask</summary>
        MOVMSKPD,
        ///<summary>Extract Packed Single-Precision Floating-Point Sign Mask</summary>
        MOVMSKPS,
        ///<summary>Load Double Quadword Non-Temporal Aligned Hint</summary>
        MOVNTDQA,
        ///<summary>Store Packed Integers Using Non-Temporal Hint</summary>
        MOVNTDQ,
        ///<summary>Store Doubleword Using Non-Temporal Hint</summary>
        MOVNTI,
        ///<summary>Store Packed Double-Precision Floating-Point Values Using Non-Temporal Hint</summary>
        MOVNTPD,
        ///<summary>Store Packed Single-Precision Floating-Point Values Using Non-Temporal Hint</summary>
        MOVNTPS,
        ///<summary>Store of Quadword Using Non-Temporal Hint</summary>
        MOVNTQ,
        ///<summary>Move Quadword from MMX Technology to XMM Register</summary>
        MOVQ2DQ,
        ///<summary>Move Data from String to String</summary>
        MOVS,
        ///<summary>Move Data from String to String</summary>
        MOVSB,
        ///<summary>Move Data from String to String</summary>
        MOVSW,
        ///<summary>Move Data from String to String/Move or Merge Scalar Double-Precision Floating-Point Value</summary>
        MOVSD,
        ///<summary>Move Data from String to String</summary>
        MOVSQ,
        ///<summary>Replicate Single FP Values</summary>
        MOVSHDUP,
        ///<summary>Replicate Single FP Values</summary>
        MOVSLDUP,
        ///<summary>Move or Merge Scalar Single-Precision Floating-Point Value</summary>
        MOVSS,
        ///<summary>Move with Sign-Extension</summary>
        MOVSX,
        ///<summary>Move with Sign-Extension</summary>
        MOVSXD,
        ///<summary>Move Unaligned Packed Double-Precision Floating-Point Values</summary>
        MOVUPD,
        ///<summary>Move Unaligned Packed Single-Precision Floating-Point Values</summary>
        MOVUPS,
        ///<summary>Move with Zero-Extend</summary>
        MOVZX,
        ///<summary>Compute Multiple Packed Sums of Absolute Difference</summary>
        MPSADBW,
        ///<summary>Unsigned Multiply</summary>
        MUL,
        ///<summary>Multiply Packed Double-Precision Floating-Point Values</summary>
        MULPD,
        ///<summary>Multiply Packed Single-Precision Floating-Point Values</summary>
        MULPS,
        ///<summary>Multiply Scalar Double-Precision Floating-Point Value</summary>
        MULSD,
        ///<summary>Multiply Scalar Single-Precision Floating-Point Values</summary>
        MULSS,
        ///<summary>Unsigned Multiply Without Affecting Flags</summary>
        MULX,
        ///<summary>Monitor Wait</summary>
        MWAIT,
        ///<summary>Two's Complement Negation</summary>
        NEG,
        ///<summary>No Operation</summary>
        NOP,
        ///<summary>One's Complement Negation</summary>
        NOT,
        ///<summary>Logical Inclusive OR</summary>
        OR,
        ///<summary>Bitwise Logical OR of Packed Double Precision Floating-Point Values</summary>
        ORPD,
        ///<summary>Bitwise Logical OR of Packed Single Precision Floating-Point Values</summary>
        ORPS,
        ///<summary>Output to Port</summary>
        OUT,
        ///<summary>Output String to Port</summary>
        OUTS,
        ///<summary>Output String to Port</summary>
        OUTSB,
        ///<summary>Output String to Port</summary>
        OUTSW,
        ///<summary>Output String to Port</summary>
        OUTSD,
        ///<summary>Packed Absolute Value</summary>
        PABSB,
        ///<summary>Packed Absolute Value</summary>
        PABSW,
        ///<summary>Packed Absolute Value</summary>
        PABSD,
        ///<summary>Packed Absolute Value</summary>
        PABSQ,
        ///<summary>Pack with Signed Saturation</summary>
        PACKSSWB,
        ///<summary>Pack with Signed Saturation</summary>
        PACKSSDW,
        ///<summary>Pack with Unsigned Saturation</summary>
        PACKUSDW,
        ///<summary>Pack with Unsigned Saturation</summary>
        PACKUSWB,
        ///<summary>Add Packed Integers</summary>
        PADDB,
        ///<summary>Add Packed Integers</summary>
        PADDW,
        ///<summary>Add Packed Integers</summary>
        PADDD,
        ///<summary>Add Packed Integers</summary>
        PADDQ,
        ///<summary>Add Packed Signed Integers with Signed Saturation</summary>
        PADDSB,
        ///<summary>Add Packed Signed Integers with Signed Saturation</summary>
        PADDSW,
        ///<summary>Add Packed Unsigned Integers with Unsigned Saturation</summary>
        PADDUSB,
        ///<summary>Add Packed Unsigned Integers with Unsigned Saturation </summary>
        PADDUSW,
        ///<summary>Packed Align Right</summary>
        PALIGNR,
        ///<summary>Logical AND</summary>
        PAND,
        ///<summary>Logical AND NOT</summary>
        PANDN,
        ///<summary>Spin Loop Hint</summary>
        PAUSE,
        ///<summary>Average Packed Integers</summary>
        PAVGB,
        ///<summary>Average Packed Integers</summary>
        PAVGW,
        ///<summary>Variable Blend Packed Bytes</summary>
        PBLENDVB,
        ///<summary>Blend Packed Words</summary>
        PBLENDW,
        ///<summary>Carry-Less Multiplication Quadword</summary>
        PCLMULQDQ,
        ///<summary>Compare Packed Data for Equal</summary>
        PCMPEQB,
        ///<summary>Compare Packed Data for Equal</summary>
        PCMPEQW,
        ///<summary>Compare Packed Data for Equal</summary>
        PCMPEQD,
        ///<summary>Compare Packed Qword Data for Equal</summary>
        PCMPEQQ,
        ///<summary>Packed Compare Explicit Length Strings, Return Index</summary>
        PCMPESTRI,
        ///<summary>Packed Compare Explicit Length Strings, Return Mask</summary>
        PCMPESTRM,
        ///<summary>Compare Packed Signed Integers for Greater Than</summary>
        PCMPGTB,
        ///<summary>Compare Packed Signed Integers for Greater Than</summary>
        PCMPGTW,
        ///<summary>Compare Packed Signed Integers for Greater Than</summary>
        PCMPGTD,
        ///<summary>Compare Packed Data for Greater Than</summary>
        PCMPGTQ,
        ///<summary>Packed Compare Implicit Length Strings, Return Index</summary>
        PCMPISTRI,
        ///<summary>Packed Compare Implicit Length Strings, Return Mask</summary>
        PCMPISTRM,
        ///<summary>Parallel Bits Deposit</summary>
        PDEP,
        ///<summary>Parallel Bits Extract</summary>
        PEXT,
        ///<summary>Extract Byte</summary>
        PEXTRB,
        ///<summary>Extract Dword</summary>
        PEXTRD,
        ///<summary>Extract Qword</summary>
        PEXTRQ,
        ///<summary>Extract Word</summary>
        PEXTRW,
        ///<summary>Packed Horizontal Add</summary>
        PHADDW,
        ///<summary>Packed Horizontal Add</summary>
        PHADDD,
        ///<summary>Packed Horizontal Add and Saturate</summary>
        PHADDSW,
        ///<summary>Packed Horizontal Word Minimum</summary>
        PHMINPOSUW,
        ///<summary>Packed Horizontal Subtract</summary>
        PHSUBW,
        ///<summary>Packed Horizontal Subtract</summary>
        PHSUBD,
        ///<summary>Packed Horizontal Subtract and Saturate</summary>
        PHSUBSW,
        ///<summary>Insert Byte</summary>
        PINSRB,
        ///<summary>Insert Dword</summary>
        PINSRD,
        ///<summary>Insert Qword</summary>
        PINSRQ,
        ///<summary>Insert Word</summary>
        PINSRW,
        ///<summary>Multiply and Add Packed Signed and Unsigned Bytes</summary>
        PMADDUBSW,
        ///<summary>Multiply and Add Packed Integers</summary>
        PMADDWD,
        ///<summary>Maximum of Packed Signed Integers</summary>
        PMAXSB,
        ///<summary>Maximum of Packed Signed Integers</summary>
        PMAXSW,
        ///<summary>Maximum of Packed Signed Integers</summary>
        PMAXSD,
        ///<summary>Maximum of Packed Signed Integers</summary>
        PMAXSQ,
        ///<summary>Maximum of Packed Unsigned Integers</summary>
        PMAXUB,
        ///<summary>Maximum of Packed Unsigned Integers</summary>
        PMAXUW,
        ///<summary>Maximum of Packed Unsigned Integers</summary>
        PMAXUD,
        ///<summary>Maximum of Packed Unsigned Integers</summary>
        PMAXUQ,
        ///<summary>Minimum of Packed Signed Integers</summary>
        PMINSB,
        ///<summary>Minimum of Packed Signed Integers</summary>
        PMINSW,
        ///<summary>Minimum of Packed Signed Integers</summary>
        PMINSD,
        ///<summary>Minimum of Packed Signed Integers</summary>
        PMINSQ,
        ///<summary>Minimum of Packed Unsigned Integers</summary>
        PMINUB,
        ///<summary>Minimum of Packed Unsigned Integers</summary>
        PMINUW,
        ///<summary>Minimum of Packed Unsigned Integers</summary>
        PMINUD,
        ///<summary>Minimum of Packed Unsigned Integers</summary>
        PMINUQ,
        ///<summary>Move Byte Mask</summary>
        PMOVMSKB,
        ///<summary>Packed Move with Sign Extend</summary>
        PMOVSX,
        ///<summary>Packed Move with Zero Extend</summary>
        PMOVZX,
        ///<summary>Multiply Packed Doubleword Integers</summary>
        PMULDQ,
        ///<summary>Packed Multiply High with Round and Scale</summary>
        PMULHRSW,
        ///<summary>Multiply Packed Unsigned Integers and Store High Result</summary>
        PMULHUW,
        ///<summary>Multiply Packed Signed Integers and Store High Result</summary>
        PMULHW,
        ///<summary>Multiply Packed Integers and Store Low Result</summary>
        PMULLD,
        ///<summary>Multiply Packed Integers and Store Low Result</summary>
        PMULLQ,
        ///<summary>Multiply Packed Signed Integers and Store Low Result</summary>
        PMULLW,
        ///<summary>Multiply Packed Unsigned Doubleword Integers</summary>
        PMULUDQ,
        ///<summary>Pop a Value from the Stack</summary>
        POP,
        ///<summary>Pop All General-Purpose Registers</summary>
        POPA,
        ///<summary>Pop All General-Purpose Registers</summary>
        POPAD,
        ///<summary>Return the Count of Number of Bits Set to 1</summary>
        POPCNT,
        ///<summary>Pop Stack into EFLAGS Register</summary>
        POPF,
        ///<summary>Pop Stack into EFLAGS Register</summary>
        POPFD,
        ///<summary>Pop Stack into EFLAGS Register</summary>
        POPFQ,
        ///<summary>Bitwise Logical OR</summary>
        POR,
        ///<summary>Prefetch Data Into Caches</summary>
        PREFETCHT0,
        ///<summary>Prefetch Data Into Caches</summary>
        PREFETCHT1,
        ///<summary>Prefetch Data Into Caches</summary>
        PREFETCHT2,
        ///<summary>Prefetch Data Into Caches</summary>
        PREFETCHNTA,
        ///<summary>Prefetch Data into Caches in Anticipation of a Write</summary>
        PREFETCHW,
        ///<summary>Compute Sum of Absolute Differences</summary>
        PSADBW,
        ///<summary>Packed Shuffle Bytes</summary>
        PSHUFB,
        ///<summary>Shuffle Packed Doublewords</summary>
        PSHUFD,
        ///<summary>Shuffle Packed High Words</summary>
        PSHUFHW,
        ///<summary>Shuffle Packed Low Words</summary>
        PSHUFLW,
        ///<summary>Shuffle Packed Words</summary>
        PSHUFW,
        ///<summary>Packed SIGN</summary>
        PSIGNB,
        ///<summary>Packed SIGN</summary>
        PSIGNW,
        ///<summary>Packed SIGN</summary>
        PSIGND,
        ///<summary>Shift Double Quadword Left Logical</summary>
        PSLLDQ,
        ///<summary>Shift Packed Data Left Logical</summary>
        PSLLW,
        ///<summary>Shift Packed Data Left Logical</summary>
        PSLLD,
        ///<summary>Shift Packed Data Left LogicalAdd</summary>
        PSLLQ,
        ///<summary>Shift Packed Data Right Arithmetic</summary>
        PSRAW,
        ///<summary>Shift Packed Data Right Arithmetic</summary>
        PSRAD,
        ///<summary>Shift Packed Data Right Arithmetic</summary>
        PSRAQ,
        ///<summary>Shift Double Quadword Right Logical</summary>
        PSRLDQ,
        ///<summary>Shift Packed Data Right Logical</summary>
        PSRLW,
        ///<summary>Shift Packed Data Right Logical</summary>
        PSRLD,
        ///<summary>Shift Packed Data Right Logical</summary>
        PSRLQ,
        ///<summary>Subtract Packed Integers</summary>
        PSUBB,
        ///<summary>Subtract Packed Integers</summary>
        PSUBW,
        ///<summary>Subtract Packed Integers</summary>
        PSUBD,
        ///<summary>Subtract Packed Quadword Integers</summary>
        PSUBQ,
        ///<summary>Subtract Packed Signed Integers with Signed Saturation</summary>
        PSUBSB,
        ///<summary>Subtract Packed Signed Integers with Signed Saturation</summary>
        PSUBSW,
        ///<summary>Subtract Packed Unsigned Integers with Unsigned Saturation</summary>
        PSUBUSB,
        ///<summary>Subtract Packed Unsigned Integers with Unsigned Saturation</summary>
        PSUBUSW,
        ///<summary>Logical Compare</summary>
        PTEST,
        ///<summary>Write Data to a Processor Trace Packet</summary>
        PTWRITE,
        ///<summary>Unpack High Data</summary>
        PUNPCKHBW,
        ///<summary>Unpack High Data</summary>
        PUNPCKHWD,
        ///<summary>Unpack High Data</summary>
        PUNPCKHDQ,
        ///<summary>Unpack High Data</summary>
        PUNPCKHQDQ,
        ///<summary>Unpack Low Data</summary>
        PUNPCKLBW,
        ///<summary>Unpack Low Data</summary>
        PUNPCKLWD,
        ///<summary>Unpack Low Data</summary>
        PUNPCKLDQ,
        ///<summary>Unpack Low Data</summary>
        PUNPCKLQDQ,
        ///<summary>Push Word, Doubleword or Quadword Onto the Stack</summary>
        PUSH,
        ///<summary>Push All General-Purpose Registers</summary>
        PUSHA,
        ///<summary>Push All General-Purpose Registers</summary>
        PUSHAD,
        ///<summary>Push EFLAGS Register onto the Stack</summary>
        PUSHF,
        ///<summary>Push EFLAGS Register onto the Stack</summary>
        PUSHFD,
        ///<summary>Push EFLAGS Register onto the Stack</summary>
        PUSHFQ,
        ///<summary>Logical Exclusive OR</summary>
        PXOR,
        ///<summary>Rotate</summary>
        RCL,
        ///<summary>Rotate</summary>
        RCR,
        ///<summary>Rotate</summary>
        ROL,
        ///<summary>Rotate</summary>
        ROR,
        ///<summary>Compute Reciprocals of Packed Single-Precision Floating-Point Values</summary>
        RCPPS,
        ///<summary>Compute Reciprocal of Scalar Single-Precision Floating-Point Values</summary>
        RCPSS,
        ///<summary>Read FS Segment Base</summary>
        RDFSBASE,
        ///<summary>Read GS Segment Base</summary>
        RDGSBASE,
        ///<summary>Read from Model Specific Register</summary>
        RDMSR,
        ///<summary>Read Processor ID</summary>
        RDPID,
        ///<summary>Read Protection Key Rights for User Pages</summary>
        RDPKRU,
        ///<summary>Read Performance-Monitoring Counters</summary>
        RDPMC,
        ///<summary>Read Random Number</summary>
        RDRAND,
        ///<summary>Read Random SEED</summary>
        RDSEED,
        ///<summary>Read Shadow Stack Pointer</summary>
        RDSSPD,
        ///<summary>Read Shadow Stack Pointer</summary>
        RDSSPQ,
        ///<summary>Read Time-Stamp Counter</summary>
        RDTSC,
        ///<summary>Read Time-Stamp Counter and Processor ID</summary>
        RDTSCP,
        ///<summary>Repeat String Operation Prefix</summary>
        REP,
        ///<summary>Repeat String Operation Prefix</summary>
        REPE,
        ///<summary>Repeat String Operation Prefix</summary>
        REPZ,
        ///<summary>Repeat String Operation Prefix</summary>
        REPNE,
        ///<summary>Repeat String Operation Prefix</summary>
        REPNZ,
        ///<summary>Return from Procedure</summary>
        RET,
        ///<summary>Rotate Right Logical Without Affecting Flags</summary>
        RORX,
        ///<summary>Round Packed Double Precision Floating-Point Values</summary>
        ROUNDPD,
        ///<summary>Round Packed Single Precision Floating-Point Values</summary>
        ROUNDPS,
        ///<summary>Round Scalar Double Precision Floating-Point Values</summary>
        ROUNDSD,
        ///<summary>Round Scalar Single Precision Floating-Point Values</summary>
        ROUNDSS,
        ///<summary>Resume from System Management Mode</summary>
        RSM,
        ///<summary>Compute Reciprocals of Square Roots of Packed Single-Precision Floating-Point Values</summary>
        RSQRTPS,
        ///<summary>Compute Reciprocal of Square Root of Scalar Single-Precision Floating-Point Value</summary>
        RSQRTSS,
        ///<summary>Restore Saved Shadow Stack Pointe</summary>
        RSTORSSP,
        ///<summary>Store AH into Flags</summary>
        SAHF,
        ///<summary>Shift</summary>
        SAL,
        ///<summary>Shift</summary>
        SAR,
        ///<summary>Shift</summary>
        SHL,
        ///<summary>Shift</summary>
        SHR,
        ///<summary>Shift Without Affecting Flags</summary>
        SARX,
        ///<summary>Shift Without Affecting Flags</summary>
        SHLX,
        ///<summary>Shift Without Affecting Flags</summary>
        SHRX,
        ///<summary>Save Previous Shadow Stack Pointer</summary>
        SAVEPREVSSP,
        ///<summary>Integer Subtraction with Borrow</summary>
        SBB,
        ///<summary>Scan String</summary>
        SCAS,
        ///<summary>Scan String</summary>
        SCASB,
        ///<summary>Scan String</summary>
        SCASW,
        ///<summary>Scan String</summary>
        SCASD,
        ///<summary>Set Byte on Condition</summary>
        SETA,
        ///<summary>Set Byte on Condition</summary>
        SETAE,
        ///<summary>Set Byte on Condition</summary>
        SETB,
        ///<summary>Set Byte on Condition</summary>
        SETBE,
        ///<summary>Set Byte on Condition</summary>
        SETC,
        ///<summary>Set Byte on Condition</summary>
        SETE,
        ///<summary>Set Byte on Condition</summary>
        SETG,
        ///<summary>Set Byte on Condition</summary>
        SETGE,
        ///<summary>Set Byte on Condition</summary>
        SETL,
        ///<summary>Set Byte on Condition</summary>
        SETLE,
        ///<summary>Set Byte on Condition</summary>
        SETNA,
        ///<summary>Set Byte on Condition</summary>
        SETNAE,
        ///<summary>Set Byte on Condition</summary>
        SETNB,
        ///<summary>Set Byte on Condition</summary>
        SETNBE,
        ///<summary>Set Byte on Condition</summary>
        SETNC,
        ///<summary>Set Byte on Condition</summary>
        SETNE,
        ///<summary>Set Byte on Condition</summary>
        SETNG,
        ///<summary>Set Byte on Condition</summary>
        SETNGE,
        ///<summary>Set Byte on Condition</summary>
        SETNL,
        ///<summary>Set Byte on Condition</summary>
        SETNLE,
        ///<summary>Set Byte on Condition</summary>
        SETNO,
        ///<summary>Set Byte on Condition</summary>
        SETNP,
        ///<summary>Set Byte on Condition</summary>
        SETNS,
        ///<summary>Set Byte on Condition</summary>
        SETNZ,
        ///<summary>Set Byte on Condition</summary>
        SETO,
        ///<summary>Set Byte on Condition</summary>
        SETP,
        ///<summary>Set Byte on Condition</summary>
        SETPE,
        ///<summary>Set Byte on Condition</summary>
        SETPO,
        ///<summary>Set Byte on Condition</summary>
        SETS,
        ///<summary>Set Byte on Condition</summary>
        SETZ,
        ///<summary>Mark Shadow Stack Busy</summary>
        SETSSBSY,
        ///<summary>Store Fence</summary>
        SFENCE,
        ///<summary>Store Global Descriptor Table Register</summary>
        SGDT,
        ///<summary>Perform Four Rounds of SHA1 Operation</summary>
        SHA1RNDS4,
        ///<summary>Calculate SHA1 State Variable E after Four Rounds</summary>
        SHA1NEXTE,
        ///<summary>Perform an Intermediate Calculation for the Next Four SHA1 Message Dwords</summary>
        SHA1MSG1,
        ///<summary>Perform a Final Calculation for the Next Four SHA1 Message Dwords</summary>
        SHA1MSG2,
        ///<summary>Perform Two Rounds of SHA256 Operation</summary>
        SHA256RNDS2,
        ///<summary>Perform an Intermediate Calculation for the Next Four SHA256 Message Dwords</summary>
        SHA256MSG1,
        ///<summary>Perform a Final Calculation for the Next Four SHA256 Message Dwords</summary>
        SHA256MSG2,
        ///<summary>Double Precision Shift Left</summary>
        SHLD,
        ///<summary>Double Precision Shift Right</summary>
        SHRD,
        ///<summary>Packed Interleave Shuffle of Pairs of Double-Precision Floating-Point Values</summary>
        SHUFPD,
        ///<summary>Packed Interleave Shuffle of Quadruplets of Single-Precision Floating-Point Values</summary>
        SHUFPS,
        ///<summary>Store Interrupt Descriptor Table Register</summary>
        SIDT,
        ///<summary>Store Local Descriptor Table Register</summary>
        SLDT,
        ///<summary>Store Machine Status Word</summary>
        SMSW,
        ///<summary>Square Root of Double-Precision Floating-Point Values</summary>
        SQRTPD,
        ///<summary>Square Root of Single-Precision Floating-Point Values</summary>
        SQRTPS,
        ///<summary>Compute Square Root of Scalar Double-Precision Floating-Point Value</summary>
        SQRTSD,
        ///<summary>Compute Square Root of Scalar Single-Precision Value</summary>
        SQRTSS,
        ///<summary>Set AC Flag in EFLAGS Register</summary>
        STAC,
        ///<summary>Set Carry Flag</summary>
        STC,
        ///<summary>Set Direction Flag</summary>
        STD,
        ///<summary>Set Interrupt Flag</summary>
        STI,
        ///<summary>Store MXCSR Register State</summary>
        STMXCSR,
        ///<summary>Store String</summary>
        STOS,
        ///<summary>Store String</summary>
        STOSB,
        ///<summary>Store String</summary>
        STOSW,
        ///<summary>Store String</summary>
        STOSD,
        ///<summary>Store String</summary>
        STOSQ,
        ///<summary>Store Task Register</summary>
        STR,
        ///<summary>Subtract</summary>
        SUB,
        ///<summary>Subtract Packed Double-Precision Floating-Point Values</summary>
        SUBPD,
        ///<summary>Subtract Packed Single-Precision Floating-Point Values</summary>
        SUBPS,
        ///<summary>Subtract Scalar Double-Precision Floating-Point Value</summary>
        SUBSD,
        ///<summary>Subtract Scalar Single-Precision Floating-Point Value</summary>
        SUBSS,
        ///<summary>Swap GS Base Register</summary>
        SWAPGS,
        ///<summary>Fast System Call</summary>
        SYSCALL,
        ///<summary>Fast System Call</summary>
        SYSENTER,
        ///<summary>Fast Return from Fast System Call</summary>
        SYSEXIT,
        ///<summary>Return From Fast System Call</summary>
        SYSRET,
        ///<summary>Logical Compare</summary>
        TEST,
        ///<summary>Timed PAUSE</summary>
        TPAUSE,
        ///<summary>Count the Number of Trailing Zero Bits</summary>
        TZCNT,
        ///<summary>Unordered Compare Scalar Double-Precision Floating-Point Values and Set EFLAGS</summary>
        UCOMISD,
        ///<summary>Unordered Compare Scalar Single-Precision Floating-Point Values and Set EFLAGS</summary>
        UCOMISS,
        ///<summary>Undefined Instruction</summary>
        UD0,
        ///<summary>Undefined Instruction</summary>
        UD1,
        ///<summary>Undefined Instruction</summary>
        UD2,
        ///<summary>User Level Set Up Monitor Address</summary>
        UMONITOR,
        ///<summary>User Level Monitor Wait</summary>
        UMWAIT,
        ///<summary>Unpack and Interleave High Packed Double-Precision Floating-Point Values</summary>
        UNPCKHPD,
        ///<summary>Unpack and Interleave High Packed Single-Precision Floating-Point Values</summary>
        UNPCKHPS,
        ///<summary>Unpack and Interleave Low Packed Double-Precision Floating-Point Values</summary>
        UNPCKLPD,
        ///<summary>Unpack and Interleave Low Packed Single-Precision Floating-Point Values</summary>
        UNPCKLPS,
        ///<summary>Align Doubleword Vectors</summary>
        VALIGND,
        ///<summary>Align Quadword Vectors</summary>
        VALIGNQ,
        ///<summary>Blend Float64 Vectors Using an OpMask Control</summary>
        VBLENDMPD,
        ///<summary>Blend Float32 Vectors Using an OpMask Control</summary>
        VBLENDMPS,
        ///<summary>Load with Broadcast Floating-Point Data</summary>
        VBROADCAST,
        ///<summary>Store Sparse Packed Double-Precision Floating-Point Values into Dense Memory</summary>
        VCOMPRESSPD,
        ///<summary>Store Sparse Packed Single-Precision Floating-Point Values into Dense Memory</summary>
        VCOMPRESSPS,
        ///<summary>Convert Packed Double-Precision Floating-Point Values to Packed Quadword Integers</summary>
        VCVTPD2QQ,
        ///<summary>Convert Packed Double-Precision Floating-Point Values to Packed Unsigned Doubleword Integers</summary>
        VCVTPD2UDQ,
        ///<summary>Convert Packed Double-Precision Floating-Point Values to Packed Unsigned Quadword Integers</summary>
        VCVTPD2UQQ,
        ///<summary>Convert 16-bit FP values to Single-Precision FP values</summary>
        VCVTPH2PS,
        ///<summary>Convert Single-Precision FP value to 16-bit FP value</summary>
        VCVTPS2PH,
        ///<summary>Convert Packed Single-Precision Floating-Point Values to Packed Unsigned Doubleword Integer Values</summary>
        VCVTPS2UDQ,
        ///<summary>Convert Packed Single Precision Floating-Point Values to Packed Signed Quadword Integer Values</summary>
        VCVTPS2QQ,
        ///<summary>Convert Packed Single Precision Floating-Point Values to Packed Unsigned Quadword Integer Values</summary>
        VCVTPS2UQQ,
        ///<summary>Convert Packed Quadword Integers to Packed Double-Precision Floating-Point Values</summary>
        VCVTQQ2PD,
        ///<summary>Convert Packed Quadword Integers to Packed Single-Precision Floating-Point Values</summary>
        VCVTQQ2PS,
        ///<summary>Convert Scalar Double-Precision Floating-Point Value to Unsigned Doubleword Integer</summary>
        VCVTSD2USI,
        ///<summary>Convert Scalar Single-Precision Floating-Point Value to Unsigned Doubleword Integer</summary>
        VCVTSS2USI,
        ///<summary>Convert with Truncation Packed Double-Precision Floating-Point Values to Packed Quadword Integers</summary>
        VCVTTPD2QQ,
        ///<summary>Convert with Truncation Packed Double-Precision Floating-Point Values to Packed Unsigned Doubleword Integers</summary>
        VCVTTPD2UDQ,
        ///<summary>Convert with Truncation Packed Double-Precision Floating-Point Values to Packed Unsigned Quadword Integers</summary>
        VCVTTPD2UQQ,
        ///<summary>Convert with Truncation Packed Single-Precision Floating-Point Values to Packed Unsigned Doubleword Integer Values</summary>
        VCVTTPS2UDQ,
        ///<summary>Convert with Truncation Packed Single Precision Floating-Point Values to Packed Signed Quadword Integer Values</summary>
        VCVTTPS2QQ,
        ///<summary>Convert with Truncation Packed Single Precision Floating-Point Values to Packed Unsigned Quadword Integer Values</summary>
        VCVTTPS2UQQ,
        ///<summary>Convert with Truncation Scalar Double-Precision Floating-Point Value to Unsigned Integer</summary>
        VCVTTSD2USI,
        ///<summary>Convert with Truncation Scalar Single-Precision Floating-Point Value to Unsigned Integer</summary>
        VCVTTSS2USI,
        ///<summary>Convert Packed Unsigned Doubleword Integers to Packed Double-Precision Floating-Point Values</summary>
        VCVTUDQ2PD,
        ///<summary>Convert Packed Unsigned Doubleword Integers to Packed Single-Precision Floating-Point Values</summary>
        VCVTUDQ2PS,
        ///<summary>Convert Packed Unsigned Quadword Integers to Packed Double-Precision Floating-Point Values</summary>
        VCVTUQQ2PD,
        ///<summary>Convert Packed Unsigned Quadword Integers to Packed Single-Precision Floating-Point Values</summary>
        VCVTUQQ2PS,
        ///<summary>Convert Unsigned Integer to Scalar Double-Precision Floating-Point Value</summary>
        VCVTUSI2SD,
        ///<summary>Convert Unsigned Integer to Scalar Single-Precision Floating-Point Value</summary>
        VCVTUSI2SS,
        ///<summary>Double Block Packed Sum-Absolute-Differences (SAD) on Unsigned Bytes</summary>
        VDBPSADBW,
        ///<summary>Load Sparse Packed Double-Precision Floating-Point Values from Dense Memory</summary>
        VEXPANDPD,
        ///<summary>Load Sparse Packed Single-Precision Floating-Point Values from Dense Memory</summary>
        VEXPANDPS,
        ///<summary>Verify a Segment for Reading or Writing</summary>
        VERR,
        ///<summary>Verify a Segment for Reading or Writing</summary>
        VERW,
        ///<summary>Extra ct Packed Floating-Point Values</summary>
        VEXTRACTF128,
        ///<summary>Extra ct Packed Floating-Point Values</summary>
        VEXTRACTF32x4,
        ///<summary>Extra ct Packed Floating-Point Values</summary>
        VEXTRACTF64x2,
        ///<summary>Extra ct Packed Floating-Point Values</summary>
        VEXTRACTF32x8,
        ///<summary>Extra ct Packed Floating-Point Values</summary>
        VEXTRACTF64x4,
        ///<summary>Extract packed Integer Values</summary>
        VEXTRACTI128,
        ///<summary>Extract packed Integer Values</summary>
        VEXTRACTI32x4,
        ///<summary>Extract packed Integer Values</summary>
        VEXTRACTI64x2,
        ///<summary>Extract packed Integer Values</summary>
        VEXTRACTI32x8,
        ///<summary>Extract packed Integer Values</summary>
        VEXTRACTI64x4,
        ///<summary>Fix Up Special Packed Float64 Values</summary>
        VFIXUPIMMPD,
        ///<summary>Fix Up Special Packed Float32 Values</summary>
        VFIXUPIMMPS,
        ///<summary>Fix Up Special Scalar Float64 Value</summary>
        VFIXUPIMMSD,
        ///<summary>Fix Up Special Scalar Float32 Value</summary>
        VFIXUPIMMSS,
        ///<summary>Fused Multiply-Add of Packed Double-Precision Floating-Point Values</summary>
        VFMADD132PD,
        ///<summary>Fused Multiply-Add of Packed Double-Precision Floating-Point Values</summary>
        VFMADD213PD,
        ///<summary>Fused Multiply-Add of Packed Double-Precision Floating-Point Values</summary>
        VFMADD231PD,
        ///<summary>Fused Multiply-Add of Packed Single-Precision Floating-Point Values</summary>
        VFMADD132PS,
        ///<summary>Fused Multiply-Add of Packed Single-Precision Floating-Point Values</summary>
        VFMADD213PS,
        ///<summary>Fused Multiply-Add of Packed Single-Precision Floating-Point Values</summary>
        VFMADD231PS,
        ///<summary>Fused Multiply-Add of Scalar Double-Precision Floating-Point Values</summary>
        VFMADD132SD,
        ///<summary>Fused Multiply-Add of Scalar Double-Precision Floating-Point Values</summary>
        VFMADD213SD,
        ///<summary>Fused Multiply-Add of Scalar Double-Precision Floating-Point Values</summary>
        VFMADD231SD,
        ///<summary>Fused Multiply-Add of Scalar Single-Precision Floating-Point Values</summary>
        VFMADD132SS,
        ///<summary>Fused Multiply-Add of Scalar Single-Precision Floating-Point Values</summary>
        VFMADD213SS,
        ///<summary>Fused Multiply-Add of Scalar Single-Precision Floating-Point Values</summary>
        VFMADD231SS,
        ///<summary>Fused Multiply-Alternating Add/Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFMADDSUB132PD,
        ///<summary>Fused Multiply-Alternating Add/Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFMADDSUB213PD,
        ///<summary>Fused Multiply-Alternating Add/Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFMADDSUB231PD,
        ///<summary>Fused Multiply-Alternating Add/Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFMADDSUB132PS,
        ///<summary>Fused Multiply-Alternating Add/Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFMADDSUB213PS,
        ///<summary>Fused Multiply-Alternating Add/Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFMADDSUB231PS,
        ///<summary>Fused Multiply-Alternating Subtract/Add of Packed Double-Precision Floating-Point Values</summary>
        VFMSUBADD132PD,
        ///<summary>Fused Multiply-Alternating Subtract/Add of Packed Double-Precision Floating-Point Values</summary>
        VFMSUBADD213PD,
        ///<summary>Fused Multiply-Alternating Subtract/Add of Packed Double-Precision Floating-Point Values</summary>
        VFMSUBADD231PD,
        ///<summary>Fused Multiply-Alternating Subtract/Add of Packed Single-Precision Floating-Point Values</summary>
        VFMSUBADD132PS,
        ///<summary>Fused Multiply-Alternating Subtract/Add of Packed Single-Precision Floating-Point Values</summary>
        VFMSUBADD213PS,
        ///<summary>Fused Multiply-Alternating Subtract/Add of Packed Single-Precision Floating-Point Values</summary>
        VFMSUBADD231PS,
        ///<summary>Fused Multiply-Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFMSUB132PD,
        ///<summary>Fused Multiply-Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFMSUB213PD,
        ///<summary>Fused Multiply-Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFMSUB231PD,
        ///<summary>Fused Multiply-Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFMSUB132PS,
        ///<summary>Fused Multiply-Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFMSUB213PS,
        ///<summary>Fused Multiply-Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFMSUB231PS,
        ///<summary>Fused Multiply-Subtract of Scalar Double-Precision Floating-Point Values</summary>
        VFMSUB132SD,
        ///<summary>Fused Multiply-Subtract of Scalar Double-Precision Floating-Point Values</summary>
        VFMSUB213SD,
        ///<summary>Fused Multiply-Subtract of Scalar Double-Precision Floating-Point Values</summary>
        VFMSUB231SD,
        ///<summary>Fused Multiply-Subtract of Scalar Single-Precision Floating-Point Values</summary>
        VFMSUB132SS,
        ///<summary>Fused Multiply-Subtract of Scalar Single-Precision Floating-Point Values</summary>
        VFMSUB213SS,
        ///<summary>Fused Multiply-Subtract of Scalar Single-Precision Floating-Point Values</summary>
        VFMSUB231SS,
        ///<summary>Fused Negative Multiply-Add of Packed Double-Precision Floating-Point Values</summary>
        VFNMADD132PD,
        ///<summary>Fused Negative Multiply-Add of Packed Double-Precision Floating-Point Values</summary>
        VFNMADD213PD,
        ///<summary>Fused Negative Multiply-Add of Packed Double-Precision Floating-Point Values</summary>
        VFNMADD231PD,
        ///<summary>Fused Negative Multiply-Add of Packed Single-Precision Floating-Point Values</summary>
        VFNMADD132PS,
        ///<summary>Fused Negative Multiply-Add of Packed Single-Precision Floating-Point Values</summary>
        VFNMADD213PS,
        ///<summary>Fused Negative Multiply-Add of Packed Single-Precision Floating-Point Values</summary>
        VFNMADD231PS,
        ///<summary>Fused Negative Multiply-Add of Scalar Double-Precision Floating-Point Values</summary>
        VFNMADD132SD,
        ///<summary>Fused Negative Multiply-Add of Scalar Double-Precision Floating-Point Values</summary>
        VFNMADD213SD,
        ///<summary>Fused Negative Multiply-Add of Scalar Double-Precision Floating-Point Values</summary>
        VFNMADD231SD,
        ///<summary>Fused Negative Multiply-Add of Scalar Single-Precision Floating-Point Values</summary>
        VFNMADD132SS,
        ///<summary>Fused Negative Multiply-Add of Scalar Single-Precision Floating-Point Values</summary>
        VFNMADD213SS,
        ///<summary>Fused Negative Multiply-Add of Scalar Single-Precision Floating-Point Values</summary>
        VFNMADD231SS,
        ///<summary>Fused Negative Multiply-Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFNMSUB132PD,
        ///<summary>Fused Negative Multiply-Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFNMSUB213PD,
        ///<summary>Fused Negative Multiply-Subtract of Packed Double-Precision Floating-Point Values</summary>
        VFNMSUB231PD,
        ///<summary>Fused Negative Multiply-Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFNMSUB132PS,
        ///<summary>Fused Negative Multiply-Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFNMSUB213PS,
        ///<summary>Fused Negative Multiply-Subtract of Packed Single-Precision Floating-Point Values</summary>
        VFNMSUB231PS,
        ///<summary>Fused Negative Multiply-Subtract of Scalar Double-Precision Floating-Point Values</summary>
        VFNMSUB132SD,
        ///<summary>Fused Negative Multiply-Subtract of Scalar Double-Precision Floating-Point Values</summary>
        VFNMSUB213SD,
        ///<summary>Fused Negative Multiply-Subtract of Scalar Double-Precision Floating-Point Values</summary>
        VFNMSUB231SD,
        ///<summary>Fused Negative Multiply-Subtract of Scalar Single-Precision Floating-Point Values</summary>
        VFNMSUB132SS,
        ///<summary>Fused Negative Multiply-Subtract of Scalar Single-Precision Floating-Point Values</summary>
        VFNMSUB213SS,
        ///<summary>Fused Negative Multiply-Subtract of Scalar Single-Precision Floating-Point Values</summary>
        VFNMSUB231SS,
        ///<summary>Tests Types Of a Packed Float64 Values</summary>
        VFPCLASSPD,
        ///<summary>Tests Types Of a Packed Float32 Values</summary>
        VFPCLASSPS,
        ///<summary>Tests Types Of a Scalar Float64 Values</summary>
        VFPCLASSSD,
        ///<summary>Tests Types Of a Scalar Float32 Values</summary>
        VFPCLASSSS,
        ///<summary>Gather Packed DP FP Values Using Signed Dword Indices</summary>
        VGATHERDPD,
        ///<summary>Gather Packed DP FP Values Using Signed Dword Indices</summary>
        VGATHERQPD,
        ///<summary>Gather Packed SP FP values Using Signed Dword Indices</summary>
        VGATHERDPS,
        ///<summary>Gather Packed SP FP values Using Signed Qword Indices</summary>
        VGATHERQPS,
        ///<summary>Convert Exponents of Packed DP FP Values to DP FP Values</summary>
        VGETEXPPD,
        ///<summary>Convert Exponents of Packed SP FP Values to SP FP Values</summary>
        VGETEXPPS,
        ///<summary>Convert Exponents of Scalar DP FP Values to DP FP Value</summary>
        VGETEXPSD,
        ///<summary>Convert Exponents of Scalar SP FP Values to SP FP Value</summary>
        VGETEXPSS,
        ///<summary>Extract Float64 Vector of Normalized Mantissas from Float64 Vector</summary>
        VGETMANTPD,
        ///<summary>Extract Float32 Vector of Normalized Mantissas from Float32 Vector</summary>
        VGETMANTPS,
        ///<summary>Extract Float64 of Normalized Mantissas from Float64 Scalar</summary>
        VGETMANTSD,
        ///<summary>Extract Float32 Vector of Normalized Mantissa from Float32 Vector</summary>
        VGETMANTSS,
        ///<summary>Insert Packed Floating-Point Values</summary>
        VINSERTF128,
        ///<summary>Insert Packed Floating-Point Values</summary>
        VINSERTF32x4,
        ///<summary>Insert Packed Floating-Point Values</summary>
        VINSERTF64x2,
        ///<summary>Insert Packed Floating-Point Values</summary>
        VINSERTF32x8,
        ///<summary>Insert Packed Floating-Point Values</summary>
        VINSERTF64x4,
        ///<summary>Insert Packed Integer Values</summary>
        VINSERTI128,
        ///<summary>Insert Packed Integer Values</summary>
        VINSERTI32x4,
        ///<summary>Insert Packed Integer Values</summary>
        VINSERTI64x2,
        ///<summary>Insert Packed Integer Values</summary>
        VINSERTI32x8,
        ///<summary>Insert Packed Integer Values</summary>
        VINSERTI64x4,
        ///<summary>Conditional SIMD Packed Loads and Stores</summary>
        VMASKMOV,
        ///<summary>Blend Packed Dwords</summary>
        VPBLENDD,
        ///<summary>Blend Byte Vectors Using an Opmask Control</summary>
        VPBLENDMB,
        ///<summary>Blend Word Vectors Using an Opmask Control</summary>
        VPBLENDMW,
        ///<summary>Blend Int32 Vectors Using an OpMask Control</summary>
        VPBLENDMD,
        ///<summary>Blend Int64 Vectors Using an OpMask Control</summary>
        VPBLENDMQ,
        ///<summary>Load with Broadcast Integer Data from General Purpose Register</summary>
        VPBROADCASTB,
        ///<summary>Load with Broadcast Integer Data from General Purpose Register</summary>
        VPBROADCASTW,
        ///<summary>Load with Broadcast Integer Data from General Purpose Register</summary>
        VPBROADCASTD,
        ///<summary>Load with Broadcast Integer Data from General Purpose Register</summary>
        VPBROADCASTQ,
        ///<summary>Load Integer and Broadcast</summary>
        VPBROADCAST,
        ///<summary>Broadcast Mask to Vector Register</summary>
        VPBROADCASTM,
        ///<summary>Compare Packed Byte Values Into Mask</summary>
        VPCMPB,
        ///<summary>Compare Packed Byte Values Into Mask</summary>
        VPCMPUB,
        ///<summary>Compare Packed Integer Values into Mask</summary>
        VPCMPD,
        ///<summary>Compare Packed Integer Values into Mask</summary>
        VPCMPUD,
        ///<summary>Compare Packed Integer Values into Mask</summary>
        VPCMPQ,
        ///<summary>Compare Packed Integer Values into Mask</summary>
        VPCMPUQ,
        ///<summary>Compare Packed Word Values Into Mask</summary>
        VPCMPW,
        ///<summary>Compare Packed Word Values Into Mask</summary>
        VPCMPUW,
        ///<summary>Store Sparse Packed Byte/Word Integer Values into Dense Memory/Register</summary>
        VPCOMPRESSB,
        ///<summary>Store Sparse Packed Byte/Word Integer Values into Dense Memory/Register</summary>
        VCOMPRESSW,
        ///<summary>Store Sparse Packed Doubleword Integer Values into Dense Memory/Register</summary>
        VPCOMPRESSD,
        ///<summary>Store Sparse Packed Quadword Integer Values into Dense Memory/Register</summary>
        VPCOMPRESSQ,
        ///<summary>Detect Conflicts Within a Vector of Packed Dword/Qword Values into Dense Memory/ Register</summary>
        VPCONFLICTD,
        ///<summary>Detect Conflicts Within a Vector of Packed Dword/Qword Values into Dense Memory/ Register</summary>
        VPCONFLICTQ,
        ///<summary>Multiply and Add Unsigned and Signed Bytes</summary>
        VPDPBUSD,
        ///<summary>Multiply and Add Unsigned and Signed Bytes with Saturation</summary>
        VPDPBUSDS,
        ///<summary>Multiply and Add Signed Word Integers</summary>
        VPDPWSSD,
        ///<summary>Multiply and Add Signed Word Integers with Saturation</summary>
        VPDPWSSDS,
        ///<summary>Permute Floating-Point Values</summary>
        VPERM2F128,
        ///<summary>Permute Integer Values</summary>
        VPERM2I128,
        ///<summary>Permute Packed Bytes Elements</summary>
        VPERMB,
        ///<summary>Permute Packed Doublewords/Words Elements</summary>
        VPERMD,
        ///<summary>Permute Packed Doublewords/Words Elements</summary>
        VPERMW,
        ///<summary>Full Permute of Bytes from Two Tables Overwriting the Index</summary>
        VPERMI2B,
        ///<summary>Full Permute From Two Tables Overwriting the Index</summary>
        VPERMI2W,
        ///<summary>Full Permute From Two Tables Overwriting the Index</summary>
        VPERMI2D,
        ///<summary>Full Permute From Two Tables Overwriting the Index</summary>
        VPERMI2Q,
        ///<summary>Full Permute From Two Tables Overwriting the Index</summary>
        VPERMI2PS,
        ///<summary>Full Permute From Two Tables Overwriting the Index</summary>
        VPERMI2PD,
        ///<summary>Permute In-Lane of Pairs of Double-Precision Floating-Point Values</summary>
        VPERMILPD,
        ///<summary>Permute In-Lane of Quadruples of Single-Precision Floating-Point Values</summary>
        VPERMILPS,
        ///<summary>Permute Double-Precision Floating-Point Elements</summary>
        VPERMPD,
        ///<summary>Permute Single-Precision Floating-Point Elements</summary>
        VPERMPS,
        ///<summary>Qwords Element Permutation</summary>
        VPERMQ,
        ///<summary>Full Permute of Bytes from Two Tables Overwriting a Table</summary>
        VPERMT2B,
        ///<summary>Full Permute from Two Tables Overwriting one Table</summary>
        VPERMT2W,
        ///<summary>Full Permute from Two Tables Overwriting one Table</summary>
        VPERMT2D,
        ///<summary>Full Permute from Two Tables Overwriting one Table</summary>
        VPERMT2Q,
        ///<summary>Full Permute from Two Tables Overwriting one Table</summary>
        VPERMT2PS,
        ///<summary>Full Permute from Two Tables Overwriting one Table</summary>
        VPERMT2PD,
        ///<summary>Expand Byte Values</summary>
        VPEXPANDB,
        ///<summary>Expand Word Values</summary>
        VPEXPANDW,
        ///<summary>Load Sparse Packed Doubleword Integer Values from Dense Memory / Register</summary>
        VPEXPANDD,
        ///<summary>Load Sparse Packed Quadword Integer Values from Dense Memory / Register</summary>
        VPEXPANDQ,
        ///<summary>Gather Packed Dword Values Using Signed Dword/Qword Indices</summary>
        VPGATHERDD,
        ///<summary>Gather Packed Dword Values Using Signed Dword/Qword Indices</summary>
        VPGATHERQD,
        ///<summary>Gather Packed Qword Values Using Signed Dword/Qword Indices</summary>
        VPGATHERQQ,
        ///<summary>Gather Packed Qword Values Using Signed Dword/Qword Indices</summary>
        VPGATHERDQ,
        ///<summary>Count the Number of Leading Zero Bits for Packed Dword, Packed Qword Values</summary>
        VPLZCNTD,
        ///<summary>Count the Number of Leading Zero Bits for Packed Dword, Packed Qword Values</summary>
        VPLZCNTQ,
        ///<summary>Packed Multiply of Unsigned 52-bit Unsigned Integers and Add High 52-bit Products to 64-bit Accumulators</summary>
        VPMADD52HUQ,
        ///<summary>Packed Multiply of Unsigned 52-bit Integers and Add the Low 52-bit Products to Qword Accumulators</summary>
        VPMADD52LUQ,
        ///<summary>Conditional SIMD Integer Packed Loads and Stores</summary>
        VPMASKMOV,
        ///<summary>Convert a Vector Register to a Mask</summary>
        VPMOVB2M,
        ///<summary>Convert a Vector Register to a Mask</summary>
        VPMOVW2M,
        ///<summary>Convert a Vector Register to a Mask</summary>
        VPMOVD2M,
        ///<summary>Convert a Vector Register to a Mask</summary>
        VPMOVQ2M,
        ///<summary>Down Convert DWord to Byte</summary>
        VPMOVDB,
        ///<summary>Down Convert DWord to Byte</summary>
        VPMOVSDB,
        ///<summary>Down Convert DWord to Byte</summary>
        VPMOVUSDB,
        ///<summary>Down Convert DWord to Word</summary>
        VPMOVDW,
        ///<summary>Down Convert DWord to Word</summary>
        VPMOVSDW,
        ///<summary>Down Convert DWord to Word</summary>
        VPMOVUSDW,
        ///<summary>Convert a Mask Register to a Vector Register</summary>
        VPMOVM2B,
        ///<summary>Convert a Mask Register to a Vector Register</summary>
        VPMOVM2W,
        ///<summary>Convert a Mask Register to a Vector Register</summary>
        VPMOVM2D,
        ///<summary>Convert a Mask Register to a Vector Register</summary>
        VPMOVM2Q,
        ///<summary>Down Convert QWord to Byte</summary>
        VPMOVQB,
        ///<summary>Down Convert QWord to Byte</summary>
        VPMOVSQB,
        ///<summary>Down Convert QWord to Byte</summary>
        VPMOVUSQB,
        ///<summary>Down Convert QWord to DWord</summary>
        VPMOVQD,
        ///<summary>AdDown Convert QWord to DWordd</summary>
        VPMOVSQD,
        ///<summary>Down Convert QWord to DWord</summary>
        VPMOVUSQD,
        ///<summary>Down Convert QWord to Word</summary>
        VPMOVQW,
        ///<summary>Down Convert QWord to Word</summary>
        VPMOVSQW,
        ///<summary>Down Convert QWord to Word</summary>
        VPMOVUSQW,
        ///<summary>Down Convert Word to Byte</summary>
        VPMOVWB,
        ///<summary>Down Convert Word to Byte</summary>
        VPMOVSWB,
        ///<summary>Down Convert Word to Byte</summary>
        VPMOVUSWB,
        ///<summary>Select Packed Unaligned Bytes from Quadword Sources</summary>
        VPMULTISHIFTQB,
        ///<summary>Return the Count of Number of Bits Set to 1 in BYTE/WORD/DWORD/QWORD</summary>
        VPOPCNTB,
        ///<summary>Return the Count of Number of Bits Set to 1 in BYTE/WORD/DWORD/QWORD</summary>
        VPOPCNTW,
        ///<summary>Return the Count of Number of Bits Set to 1 in BYTE/WORD/DWORD/QWORD</summary>
        VPOPCNTD,
        ///<summary>Return the Count of Number of Bits Set to 1 in BYTE/WORD/DWORD/QWORD</summary>
        VPOPCNTQ,
        ///<summary>Bit Rotate Left</summary>
        VPROLD,
        ///<summary>Bit Rotate Left</summary>
        VPROLVD,
        ///<summary>Bit Rotate Left</summary>
        VPROLQ,
        ///<summary>Bit Rotate Left</summary>
        VPROLVQ,
        ///<summary>Bit Rotate Right</summary>
        VPRORD,
        ///<summary>Bit Rotate Right</summary>
        VPRORVD,
        ///<summary>Bit Rotate Right</summary>
        VPRORQ,
        ///<summary>Bit Rotate Right</summary>
        VPRORVQ,
        ///<summary>Scatter Packed Dword, Packed Qword with Signed Dword, Signed Qword Indices</summary>
        VPSCATTERDD,
        ///<summary>Scatter Packed Dword, Packed Qword with Signed Dword, Signed Qword Indices</summary>
        VPSCATTERDQ,
        ///<summary>Scatter Packed Dword, Packed Qword with Signed Dword, Signed Qword Indices</summary>
        VPSCATTERQD,
        ///<summary>Scatter Packed Dword, Packed Qword with Signed Dword, Signed Qword Indices</summary>
        VPSCATTERQQ,
        ///<summary>Concatenate and Shift Packed Data Left Logical</summary>
        VPSHLD,
        ///<summary>Concatenate and Variable Shift Packed Data Left Logical</summary>
        VPSHLDV,
        ///<summary>Concatenate and Shift Packed Data Right Logical</summary>
        VPSHRD,
        ///<summary>Concatenate and Variable Shift Packed Data Right Logical</summary>
        VPSHRDV,
        ///<summary>Shuffle Bits from Quadword Elements Using Byte Indexes into Mask</summary>
        VPSHUFBITQMB,
        ///<summary>Variable Bit Shift Left Logical</summary>
        VPSLLVW,
        ///<summary>Variable Bit Shift Left Logical</summary>
        VPSLLVD,
        ///<summary>Variable Bit Shift Left Logical</summary>
        VPSLLVQ,
        ///<summary>Variable Bit Shift Right Arithmetic</summary>
        VPSRAVW,
        ///<summary>Variable Bit Shift Right Arithmetic</summary>
        VPSRAVD,
        ///<summary>Variable Bit Shift Right Arithmetic</summary>
        VPSRAVQ,
        ///<summary>Variable Bit Shift Right Logical</summary>
        VPSRLVW,
        ///<summary>Variable Bit Shift Right Logical</summary>
        VPSRLVD,
        ///<summary>Variable Bit Shift Right Logical</summary>
        VPSRLVQ,
        ///<summary>Bitwise Ternary Logic</summary>
        VPTERNLOGD,
        ///<summary>Bitwise Ternary Logic</summary>
        VPTERNLOGQ,
        ///<summary>Logical AND and Set Mask</summary>
        VPTESTMB,
        ///<summary>Logical AND and Set Mask</summary>
        VPTESTMW,
        ///<summary>Logical AND and Set Mask</summary>
        VPTESTMD,
        ///<summary>Logical AND and Set Mask</summary>
        VPTESTMQ,
        ///<summary>Logical NAND and Set</summary>
        VPTESTNMB,
        ///<summary>Logical NAND and Set</summary>
        VPTESTNMW,
        ///<summary>Logical NAND and Set</summary>
        VPTESTNMD,
        ///<summary>Logical NAND and Set</summary>
        VPTESTNMQ,
        ///<summary>Range Restriction Calculation For Packed Pairs of Float64 Values</summary>
        VRANGEPD,
        ///<summary>Range Restriction Calculation For Packed Pairs of Float32 Values</summary>
        VRANGEPS,
        ///<summary>Range Restriction Calculation From a pair of Scalar Float64 Values</summary>
        VRANGESD,
        ///<summary>Range Restriction Calculation From a Pair of Scalar Float32 Values</summary>
        VRANGESS,
        ///<summary>Compute Approximate Reciprocals of Packed Float64 Values</summary>
        VRCP14PD,
        ///<summary>Compute Approximate Reciprocal of Scalar Float64 Value</summary>
        VRCP14SD,
        ///<summary>Compute Approximate Reciprocals of Packed Float32 Values</summary>
        VRCP14PS,
        ///<summary>Perform Reduction Transformation on Packed Float64 Values</summary>
        VREDUCEPD,
        ///<summary>Perform a Reduction Transformation on a Scalar Float64 Value</summary>
        VREDUCESD,
        ///<summary>Perform Reduction Transformation on Packed Float32 Values</summary>
        VREDUCEPS,
        ///<summary>Perform a Reduction Transformation on a Scalar Float32 Value</summary>
        VREDUCESS,
        ///<summary>Round Packed Float64 Values To Include A Given Number Of Fraction Bits</summary>
        VRNDSCALEPD,
        ///<summary>Round Scalar Float64 Value To Include A Given Number Of Fraction Bits</summary>
        VRNDSCALESD,
        ///<summary>Round Packed Float32 Values To Include A Given Number Of Fraction Bits</summary>
        VRNDSCALEPS,
        ///<summary>Round Scalar Float32 Value To Include A Given Number Of Fraction Bits</summary>
        VRNDSCALESS,
        ///<summary>Compute Approximate Reciprocals of Square Roots of Packed Float64 Values</summary>
        VRSQRT14PD,
        ///<summary>Compute Approximate Reciprocal of Square Root of Scalar Float64 Value</summary>
        VRSQRT14SD,
        ///<summary>Compute Approximate Reciprocals of Square Roots of Packed Float32 Values</summary>
        VRSQRT14PS,
        ///<summary>Compute Approximate Reciprocal of Square Root of Scalar Float32 Value</summary>
        VRSQRT14SS,
        ///<summary>Scale Packed Float64 Values With Float64 Values</summary>
        VSCALEFPD,
        ///<summary>Scale Scalar Float64 Values With Float64 Values</summary>
        VSCALEFSD,
        ///<summary>Scale Packed Float32 Values With Float32 Values</summary>
        VSCALEFPS,
        ///<summary>Scale Scalar Float32 Value With Float32 Value</summary>
        VSCALEFSS,
        ///<summary>Scatter Packed Single, Packed Double with Signed Dword and Qword Indices</summary>
        VSCATTERDPS,
        ///<summary>Scatter Packed Single, Packed Double with Signed Dword and Qword Indices</summary>
        VSCATTERDPD,
        ///<summary>Scatter Packed Single, Packed Double with Signed Dword and Qword Indices</summary>
        VSCATTERQPS,
        ///<summary>Scatter Packed Single, Packed Double with Signed Dword and Qword Indices</summary>
        VSCATTERQPD,
        ///<summary>Shuffle Packed Values at 128-bit Granularity</summary>
        VSHUFF32x4,
        ///<summary>Shuffle Packed Values at 128-bit Granularity</summary>
        VSHUFF64x2,
        ///<summary>Shuffle Packed Values at 128-bit Granularity</summary>
        VSHUFI32x4,
        ///<summary>Shuffle Packed Values at 128-bit Granularity</summary>
        VSHUFI64x2,
        ///<summary>Packed Bit Test</summary>
        VTESTPD,
        ///<summary>Packed Bit Test</summary>
        VTESTPS,
        ///<summary>Zero XMM, YMM and ZMM Registers</summary>
        VZEROALL,
        ///<summary>Zero Upper Bits of YMM and ZMM Registers</summary>
        VZEROUPPER,
        ///<summary>Wait</summary>
        WAIT,
        ///<summary>Wait</summary>
        FWAIT,
        ///<summary>Write Back and Invalidate Cache</summary>
        WBINVD,
        ///<summary>Write FS Segment Base</summary>
        WRFSBASE,
        ///<summary>Write GS Segment Base</summary>
        WRGSBASE,
        ///<summary>Write to Model Specific Register</summary>
        WRMSR,
        ///<summary>Write Data to User Page Key Register</summary>
        WRPKRU,
        ///<summary>Write to Shadow Stack</summary>
        WRSSD,
        ///<summary>Write to Shadow Stack</summary>
        WRSSQ,
        ///<summary>Write to User Shadow Stack</summary>
        WRUSSD,
        ///<summary>Write to User Shadow Stack</summary>
        WRUSSQ,
        ///<summary>Hardware Lock Elision Prefix Hints</summary>
        XACQUIRE,
        ///<summary>Hardware Lock Elision Prefix Hints</summary>
        XRELEASE,
        ///<summary>Transactional Abort</summary>
        XABORT,
        ///<summary>Exchange and Add</summary>
        XADD,
        ///<summary>Transactional Begin</summary>
        XBEGIN,
        ///<summary>Exchange Register/Memory with Register</summary>
        XCHG,
        ///<summary>Transactional End</summary>
        XEND,
        ///<summary>Get Value of Extended Control Register</summary>
        XGETBV,
        ///<summary>Table Look-up Translation</summary>
        XLAT,
        ///<summary>Table Look-up Translation</summary>
        XLATB,
        ///<summary>Logical Exclusive OR</summary>
        XOR,
        ///<summary>Bitwise Logical XOR of Packed Double Precision Floating-Point Values</summary>
        XORPD,
        ///<summary>Bitwise Logical XOR of Packed Single Precision Floating-Point Values</summary>
        XORPS,
        ///<summary>Restore Processor Extended States</summary>
        XRSTOR,
        ///<summary>Restore Processor Extended States Supervisor</summary>
        XRSTORS,
        ///<summary>Save Processor Extended States</summary>
        XSAVE,
        ///<summary>Save Processor Extended States with Compaction</summary>
        XSAVEC,
        ///<summary>Save Processor Extended States Optimized</summary>
        XSAVEOPT,
        ///<summary>Save Processor Extended States Supervisor</summary>
        XSAVES,
        ///<summary>Set Extended Control Register</summary>
        XSETBV,
        ///<summary>Test If In Transactional Execution</summary>
        XTEST
    }
}