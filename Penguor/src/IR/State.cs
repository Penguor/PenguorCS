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
using System.Collections;
using System.Collections.Generic;

using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler
{
    /// <summary>
    /// 
    /// </summary>
    public class State : IEnumerable<AddressFrame>, ICollection
    {
        private readonly AddressFrame[] addressFrames;

        /// <summary>
        /// the amount of <c>AddressFrame</c>s the <c>State</c> consists of
        /// </summary>
        public int Count => addressFrames.Length;

        /// <summary>
        /// Gets a value indicating whether access to the AddressFrame is threadsafe
        /// </summary>
        public bool IsSynchronized => addressFrames.IsSynchronized;

        /// <summary>
        /// 
        /// </summary>
        public object SyncRoot => addressFrames.SyncRoot;

        /// <summary>
        /// create a new State from a string array
        /// </summary>
        /// <param name="frames"></param>
        public State(AddressFrame[] frames)
        {
            frames[^1].IsLastItem = true;
            addressFrames = frames;
        }

        /// <summary>
        /// create a State from a CallExpr
        /// </summary>
        /// <param name="call">the CallExpr to create the state from</param>
        public static State FromCall(CallExpr call)
        {
            List<AddressFrame> frames = new List<AddressFrame>(call.Callee.Count);
            for (int i = 0; i < call.Callee.Count; i++)
            {
                bool isLast = i == call.Callee.Count - 1;
                Call c = call.Callee[i];
                frames.Add(c switch
                {
                    IdfCall a => new AddressFrame(a.Name, AddressType.Call, isLast),
                    FunctionCall a => new AddressFrame(a.Name, AddressType.Call, isLast),
                    _ => throw new ArgumentException()
                });
            }

            return new State(frames.ToArray());
        }

        /// <summary>
        /// create a State from a Stack
        /// </summary>
        /// <param name="stack">the Stack to create the state from</param>
        public static State FromStack(Stack<AddressFrame> stack)
        {
            var frames = stack.ToArray();
            Array.Reverse(frames);
            return new State(frames);
        }

        /// <summary>
        /// Convert the <c>State</c> into an array
        /// </summary>
        /// <returns>the array this instance of <c>State</c> encapsulates</returns>
        public AddressFrame[] ToArray() => addressFrames;

        /// <summary>
        /// gets the Enumerator of the State
        /// </summary>
        public IEnumerator<AddressFrame> GetEnumerator() => ((IEnumerable<AddressFrame>)addressFrames).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => addressFrames.GetEnumerator();

        /// <summary>
        /// Copy the State to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index) => addressFrames.CopyTo(array, index);

        /// <summary>
        /// check for equality of two states
        /// </summary>
        /// <param name="obj">the object to compare the State to</param>
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var state = (State)obj;
            for (int i = 0; i < addressFrames.Length; i++)
            {
                if (!(addressFrames[i].Symbol.token == state[i].Symbol.token)) return false;
            }
            return true;
        }

        /// <summary>
        /// generate a hash code for this instance of State
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = 25937216;
            hashCode *= -28674107 + addressFrames.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// returns the AddressFrame at the index i
        /// </summary>
        public AddressFrame this[int i]
        {
            get => addressFrames[i];
        }
    }
}