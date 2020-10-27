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
using System.Text;
using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler
{
    public class State : IEnumerable<AddressFrame>, ICollection, ICollection<AddressFrame>
    {
        private List<AddressFrame> addressFrames;

        /// <summary>
        /// the amount of <c>AddressFrame</c>s the <c>State</c> consists of
        /// </summary>
        public int Count => addressFrames.Count;

        /// <summary>
        /// Gets a value indicating whether access to AddressFrame is threadsafe
        /// </summary>
        public bool IsSynchronized => ((ICollection)addressFrames).IsSynchronized;

        public object SyncRoot => ((ICollection)addressFrames).SyncRoot;

        public bool IsReadOnly => ((ICollection<AddressFrame>)addressFrames).IsReadOnly;

        /// <summary>
        /// create a new State without any content
        /// </summary>
        public State()
        {
            addressFrames = new List<AddressFrame>();
        }

        /// <summary>
        /// create a new State from a List
        /// </summary>
        /// <param name="frames"></param>
        public State(List<AddressFrame> frames)
        {
            addressFrames = frames;
        }

        /// <summary>
        /// create a new State from a string array
        /// </summary>
        /// <param name="frames"></param>
        public State(AddressFrame[] frames)
        {
            addressFrames = new List<AddressFrame>(frames);
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
                Call c = call.Callee[i];
                frames.Add(c switch
                {
                    IdfCall a => a.Name,
                    FunctionCall a => a.Name,
                    _ => throw new ArgumentException("this call is not known to the parser")
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
        public AddressFrame[] ToArray() => addressFrames.ToArray();

        /// <summary>
        /// gets the Enumerator of the State
        /// </summary>
        public IEnumerator<AddressFrame> GetEnumerator() => ((IEnumerable<AddressFrame>)addressFrames).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => addressFrames.GetEnumerator();

        public void CopyTo(Array array, int index) => addressFrames.CopyTo((AddressFrame[])array, index);

        public void Push(AddressFrame frame) => addressFrames.Add(frame);

        /// <summary>
        /// Pops an AddressFrame from the state
        /// </summary>
        /// <returns>the AddressFrame which was removed</returns>
        /// <Exception cref="InvalidOperationException">Thrown when state is empty</Exception>
        public AddressFrame Pop()
        {
            if (addressFrames.Count > 0)
            {
                var frame = addressFrames[^1];
                addressFrames.RemoveAt(addressFrames.Count - 1);
                return frame;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static State operator +(State a, State b)
        {
            a.addressFrames.AddRange(b);
            return a;
        }

        public static State operator -(State minuend, State subtrahend)
        {
            while (subtrahend.Count != 0 && minuend.Count != 0)
            {
                if (minuend[^1].Equals(subtrahend[^1])) minuend.Pop();
                else break;
            }
            return minuend;
        }

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
            for (int i = 0; i < addressFrames.Count; i++)
            {
                if (addressFrames[i].Symbol != state[i].Symbol) return false;
            }
            return true;
        }

        /// <summary>
        /// generate a hash code for this instance of State
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = 25937216;
            foreach (var i in addressFrames)
            {
                hashCode *= -28674107 + i.GetHashCode();
            }
            return hashCode;
        }
        public void Append(IEnumerable<AddressFrame> item) => addressFrames.AddRange(item);

        public void Add(AddressFrame item) => addressFrames.Add(item);

        public void Clear() => addressFrames.Clear();

        public bool Contains(AddressFrame item) => addressFrames.Contains(item);

        public void CopyTo(AddressFrame[] array, int arrayIndex) => addressFrames.CopyTo(array, arrayIndex);

        public bool Remove(AddressFrame item) => addressFrames.Remove(item);
        public void Remove(State item) => addressFrames = (this - item).addressFrames;

        /// <summary>
        /// returns the AddressFrame at the index i
        /// </summary>
        public AddressFrame this[int i]
        {
            get => addressFrames[i];
        }

        public override string ToString() => string.Join('.', addressFrames);
    }
}