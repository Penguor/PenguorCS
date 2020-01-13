/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019
# all rights reserved
# 
*/

#if(DEBUG)

using System.IO;
using System.Collections.Generic;

namespace Penguor.Tools
{
    public class ASTPartGenerator
    {
        StreamWriter writer;
        StreamReader reader;

        char current;

        public void Generate(string file)
        {
            reader = new StreamReader(file);
            string mode = null;
            string folder = null;
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
                List<string> types = new List<string>();
                List<string> names = new List<string>();
                switch (mode)
                {

                    case "groupHead":
                        tmp = "";
                        while (!Match('"')) Advance();
                        while (!Match('"'))
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
                    case "Expr":
                        tmp = "";
                        name = "";
                        types = new List<string>();
                        names = new List<string>();
                        while (!Match('"')) Advance();
                        while (!Match('"'))
                        {
                            name += current;
                            Advance();
                        }
                        while (!Match('\n') && !Match('\r'))
                        {
                            while (!Match('"')) Advance();
                            while (!Match('"'))
                            {
                                tmp += current;
                                Advance();
                            }
                            types.Add(tmp);
                            tmp = "";
                            while (!Match('"')) Advance();
                            while (!Match('"'))
                            {
                                tmp += current;
                                Advance();
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
# all rights reserved
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{{

    /// <summary>
    /// A {name.ToUppercase()} expression
    /// </summary>
    public sealed class {name.ToUppercase()} : Expr
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
    /// Contains methods to visit all expressions
    /// </summary>
    public partial interface Visitor
    {{
        /// <summary>
        /// visit a {name.ToUppercase()}
        /// </summary>
        /// <returns></returns>
        string Visit({name.ToUppercase()} expr);
    }}
}}
");

                        writer.Close();
                        writer = null;
                        break;
                    case "Stmt":
                        tmp = "";
                        name = "";
                        types = new List<string>();
                        names = new List<string>();
                        while (!Match('"')) Advance();
                        while (!Match('"'))
                        {
                            name += current;
                            Advance();
                        }
                        while (!Match('\n') && !Match('\r'))
                        {
                            while (!Match('"')) Advance();
                            while (!Match('"'))
                            {
                                tmp += current;
                                Advance();
                            }
                            types.Add(tmp);
                            tmp = "";
                            while (!Match('"')) Advance();
                            while (!Match('"'))
                            {
                                tmp += current;
                                Advance();
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
# all rights reserved
# 
*/

using System.Collections.Generic;

namespace Penguor.Parsing.AST
{{

    /// <summary>
    /// A {name.ToUppercase()} expression
    /// </summary>
    public sealed class {name.ToUppercase()} : Stmt
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
    /// Contains methods to visit all statements
    /// </summary>
    public partial interface Visitor
    {{
        /// <summary>
        /// visit a {name.ToUppercase()}
        /// </summary>
        /// <returns></returns>
        string Visit({name.ToUppercase()} stmt);
    }}
}}
");

                        writer.Close();
                        writer = null;
                        break;
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
#endif