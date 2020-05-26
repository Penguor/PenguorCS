/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
# 
*/

using System.IO;
using System.Collections.Generic;

namespace Penguor.Tools
{
    //! improve code, currently terrible
    internal class ASTPartGenerator
    {
        StreamWriter writer;
        StreamReader reader;

        char current;

        public void Generate(string file)
        {
            reader = new StreamReader(file);
            string mode = null;
            string folder = null;
            string line = null;
            while (!reader.EndOfStream)
            {
                Advance();
                if (current == ';')
                {
                    mode = "groupHead";
                    Advance();
                }

                string tmp = "";
                string name = "";
                int c = 0;
                List<string> types = new List<string>();
                List<string> names = new List<string>();
                switch (mode)
                {
                    case "groupHead":
                        tmp = "";
                        while (char.IsWhiteSpace(current)) Advance();
                        while (!char.IsWhiteSpace(current))
                        {
                            tmp += current;
                            Advance();
                        }
                        mode = tmp;
                        tmp = "";
                        while (!Match('"')) Advance();
                        while (!Match('"'))
                        {
                            tmp += current;
                            Advance();
                        }
                        folder = tmp;
                        break;
                    default:
                        c = 0;
                        tmp = "";
                        name = "";
                        types = new List<string>();
                        names = new List<string>();
                        while (char.IsWhiteSpace(current)) Advance();
                        line = current + reader.ReadLine();
                        do
                        {
                            name += line[c];
                            c++;
                        }
                        while (c < line.Length && !char.IsWhiteSpace(line[c]));

                        while (c < line.Length)
                        {
                            while (char.IsWhiteSpace(line[c])) c++;
                            while (!char.IsWhiteSpace(line[c]))
                            {
                                tmp += line[c];
                                c++;
                            }
                            types.Add(tmp);
                            tmp = "";
                            while (char.IsWhiteSpace(line[c])) c++;
                            while (c < line.Length && !char.IsWhiteSpace(line[c]))
                            {
                                tmp += line[c];
                                c++;
                            }
                            names.Add(tmp);
                            tmp = "";
                        }
                        writer = new StreamWriter(Path.Combine(folder, name.ToUppercase()) + ".cs");
                        writer.AutoFlush = true;

                        writer.Write(
$@"/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2020
# 
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{{

    /// <summary>
    /// A {name.ToUppercase()} {mode}
    /// </summary>
    public sealed class {name.ToUppercase()} : {mode}
    {{
        /// <summary>
        /// creates a new instance of {name.ToUppercase()}
        /// </summary>
        public {name.ToUppercase()}(");

                        for (int i = 0; i < types.Count; i++)
                        {
                            writer.Write($"{types[i]} {names[i].ToLower()}");
                            if (i < types.Count - 1) writer.Write(", ");
                        }

                        writer.Write(
            @")
        {
");

                        for (int i = 0; i < names.Count; i++)
                        {
                            writer.WriteLine($"            {names[i].ToUppercase()} = {names[i].ToLower()};");
                        }

                        writer.Write(
            @"        }
");

                        for (int i = 0; i < types.Count; i++)
                        {
                            writer.WriteLine($"        /// <summary></summary>");
                            writer.WriteLine($"        public {types[i]} {names[i].ToUppercase()} {{ get; private set; }}");
                        }

                        writer.Write(
            $@"
        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name=""visitor"">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override string Accept(Visitor visitor)
        {{
            return visitor.Visit(this);
        }}
    }}

    /// <summary>
    /// Contains methods to visit all {mode}
    /// </summary>
    public partial interface Visitor
    {{
        /// <summary>
        /// visit a {name.ToUppercase()}
        /// </summary>
        /// <returns></returns>
        string Visit({name.ToUppercase()} {mode.ToLower()});
    }}
}}
");

                        writer.Close();
                        writer = null;
                        break;
//                     case "Stmt":
//                         c = 0;
//                         tmp = "";
//                         name = "";
//                         types = new List<string>();
//                         names = new List<string>();
//                         while (char.IsWhiteSpace(current)) Advance();
//                         line = current + reader.ReadLine();
//                         do
//                         {
//                             name += line[c];
//                             c++;
//                         }
//                         while (!char.IsWhiteSpace(line[c]));

//                         while (c < line.Length)
//                         {
//                             while (char.IsWhiteSpace(line[c])) c++;
//                             do
//                             {
//                                 tmp += line[c];
//                                 c++;
//                             }
//                             while (!char.IsWhiteSpace(line[c]));
//                             types.Add(tmp);
//                             tmp = "";
//                             while (char.IsWhiteSpace(line[c])) c++;
//                             do
//                             {
//                                 tmp += line[c];
//                                 c++;
//                             }
//                             while (c < line.Length && !char.IsWhiteSpace(line[c]));
//                             names.Add(tmp);
//                             tmp = "";
//                         }
//                         writer = new StreamWriter(Path.Combine(folder, name.ToUppercase()) + ".cs");
//                         writer.AutoFlush = true;

//                         writer.Write(
// $@"/*
// #
// # PenguorCS Compiler
// # ------------------
// #
// # (c) Carl Schierig 2020
// # 
// # 
// */

// using System.Collections.Generic;

// namespace Penguor.Parsing.AST
// {{

//     /// <summary>
//     /// A {name.ToUppercase()} statement
//     /// </summary>
//     public sealed class {name.ToUppercase()} : Stmt
//     {{
//         /// <summary>
//         /// creates a new instance of {name.ToUppercase()}
//         /// </summary>
//         public {name.ToUppercase()}(");

//                         for (int i = 0; i < types.Count; i++)
//                         {
//                             writer.Write($"{types[i]} {names[i].ToLower()}");
//                             if (i < types.Count - 1) writer.Write(", ");
//                         }

//                         writer.Write(
//             @")
//         {
// ");

//                         for (int i = 0; i < names.Count; i++)
//                         {
//                             writer.WriteLine($"            {names[i].ToUppercase()} = {names[i].ToLower()};");
//                         }

//                         writer.Write(
//             @"        }
// ");

//                         for (int i = 0; i < types.Count; i++)
//                         {
//                             writer.WriteLine($"        /// <summary></summary>");
//                             writer.WriteLine($"        public {types[i]} {names[i].ToUppercase()} {{ get; private set; }}");
//                         }

//                         writer.Write(
//             $@"
//         /// <summary>
//         /// returns Visit() of this instance
//         /// </summary>
//         /// <param name=""visitor"">the visitor which should visit this instance</param>
//         /// <returns>Visit() of this instance</returns>
//         public override string Accept(Visitor visitor)
//         {{
//             return visitor.Visit(this);
//         }}
//     }}

//     /// <summary>
//     /// Contains methods to visit all statements
//     /// </summary>
//     public partial interface Visitor
//     {{
//         /// <summary>
//         /// visit a {name.ToUppercase()}
//         /// </summary>
//         /// <returns></returns>
//         string Visit({name.ToUppercase()} stmt);
//     }}
// }}
// ");

//                         writer.Close();
//                         writer = null;
//                         break;
                }
            }
        }

        bool Match(char chr)
        {
            if (current == chr)
            {
                Advance();
                return true;
            }
            else return false;
        }

        void Advance()
        {
            current = (char)reader.Read();
        }
    }

}
