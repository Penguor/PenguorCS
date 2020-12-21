

using System;
using System.IO;
using System.Collections.Generic;

namespace Penguor.Compiler.Tools
{
    internal class ASTPartGenerator
    {
        private StreamReader? reader;

        private char current;

        public void Generate(string file)
        {
            reader = new StreamReader(file);
            string? longMode = null;
            string? mode = null;
            string? folder = null;
            while (!reader.EndOfStream)
            {
                Advance();
                if (current == ';')
                {
                    mode = "groupHead";
                    Advance();
                }

                List<string> types;
                List<string> names;

                string tmp;
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
                        longMode = tmp;
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
                        Directory.Delete(folder, true);
                        Directory.CreateDirectory(folder);
                        using (StreamWriter writer = new StreamWriter(Path.Combine(folder, mode + ".cs")))
                        {
                            writer.Write($@"
#pragma warning disable 1591

namespace Penguor.Compiler.Parsing.AST
{{
    /// <summary>
    /// Base class for penguor {mode}
    /// </summary>
    public abstract record {mode}
    {{
        /// <summary>
        /// <c>Accept</c> returns the visit method for the {mode}
        /// </summary>
        /// <param name=""visitor"">the visitor which visits this instance of {mode}</param>
        public abstract T Accept<T>(I{mode.ToUppercase()}Visitor<T> visitor);
    }}
}}
");
                        }
                        break;
                    default:
                        int c = 0;
                        tmp = "";
                        string name = "";
                        types = new List<string>();
                        names = new List<string>();

                        types.Add("int");
                        names.Add("id");
                        types.Add("int");
                        names.Add("offset");

                        while (char.IsWhiteSpace(current)) Advance();
                        string? line = current + reader.ReadLine();
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
                        using (StreamWriter writer = new StreamWriter(Path.Combine(folder!, name.ToUppercase()) + mode + ".cs"))
                        {
                            writer.AutoFlush = true;

                            writer.Write($@"
#pragma warning disable 1591

using System.Collections.Generic;

namespace Penguor.Compiler.Parsing.AST
{{
    /// <summary>
    /// A {name.ToUppercase()} {mode}
    /// </summary>
    public sealed record {name.ToUppercase()}{mode} : {mode}
    {{
        /// <summary>
        /// creates a new instance of {name.ToUppercase()}{mode}
        /// </summary>
        public {name.ToUppercase()}{mode}(");

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
                                writer.WriteLine($"        public {types[i]} {names[i].ToUppercase()} {{ get; init; }}");
                            }

                            writer.Write(
                $@"

        public override string ToString() => ""{name.ToLower()} {longMode!.ToLower()}"";

        /// <summary>
        /// returns Visit() of this instance
        /// </summary>
        /// <param name=""visitor"">the visitor which should visit this instance</param>
        /// <returns>Visit() of this instance</returns>
        public override T Accept<T>(I{mode!.ToUppercase()}Visitor<T> visitor)
        {{
            return visitor.Visit(this);
        }}
    }}

    /// <summary>
    /// Contains methods to visit all {longMode}s
    /// </summary>
    public partial interface I{mode!.ToUppercase()}Visitor<T>
    {{
        /// <summary>
        /// visit a {name.ToUppercase()}{mode}
        /// </summary>
        T Visit({name.ToUppercase()}{mode} {mode!.ToLower()});
    }}
}}
");
                        }
                        break;
                }
            }
        }

        private bool Match(char chr)
        {
            if (current == chr)
            {
                Advance();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Advance()
        {
            if (reader != null) current = (char)reader.Read();
            else throw new NullReferenceException();
        }
    }
}
