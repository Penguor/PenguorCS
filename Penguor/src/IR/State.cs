/*
#
# PenguorCS Compiler
# ------------------
#
# (c) Carl Schierig 2019-2020
# 
# 
*/


using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Penguor.Compiler.Parsing;
using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler.IR
{
    public class State : IEnumerable<string>
    {
        private readonly string[] addressFrames;

        public int Length
        {
            get => addressFrames.Length;
        }

        public State(string[] frames)
        {
            addressFrames = frames;
        }

        /// <summary>
        /// create a State from a token
        /// </summary>
        /// <param name="token">the token to create the State from</param>
        public static State FromToken(Token token) => new State(new string[] { token.token });

        /// <summary>
        /// create a State from a CallExpr
        /// </summary>
        /// <param name="call">the CallExpr to create the state from</param>
        public static State FromCall(CallExpr call)
        {
            List<string> frames = new List<string>(call.Callee.Count);
            foreach (var i in call.Callee)
                frames.Add(i switch
                {
                    IdfCall a => a.Name.token,
                    FunctionCall a => a.Name.token,
                    _ => throw new ArgumentException()
                });
            return new State(frames.ToArray());
        }

        /// <summary>
        /// Convert the state into an array
        /// </summary>
        /// <returns>the array this instance of <c>State</c> encapsulates</returns>
        public string[] ToArray() => addressFrames;

        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)addressFrames).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return addressFrames.GetEnumerator();
        }
        public string this[int i]
        {
            get => addressFrames[i];
        }
    }
}