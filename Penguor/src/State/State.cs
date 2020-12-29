using System;
using System.Collections;
using System.Collections.Generic;
using Penguor.Compiler.Parsing.AST;

namespace Penguor.Compiler
{
    /// <summary>
    /// represents an address in a Penguor program
    /// </summary>
    public class State : ICollection, ICollection<AddressFrame>, ICloneable
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

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        /// <returns>An object that can be used to synchronize access to the ICollection.</returns>
        public object SyncRoot => ((ICollection)addressFrames).SyncRoot;

        /// <summary>
        /// Indicates whether the state is read-only
        /// </summary>
        public bool IsReadOnly => ((ICollection<AddressFrame>)addressFrames).IsReadOnly;

        /// <summary>
        /// create a new State without any content
        /// </summary>
        public State()
        {
            addressFrames = new List<AddressFrame>();
        }

        /// <summary>
        /// create a new State with an AddressFrame
        /// </summary>
        /// <param name="frame"></param>
        public State(AddressFrame frame)
        {
            addressFrames = new() { frame };
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
            var callees = (Call[])call.Callee.ToArray().Clone();
            List<AddressFrame> frames = new List<AddressFrame>(call.Callee.Count);
            for (int i = 0; i < call.Callee.Count; i++)
            {
                Call c = callees[i];
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

        /// <summary>
        /// copy the State to a new AddressFrame array
        /// </summary>
        /// <param name="array">the array to copy the State to</param>
        /// <param name="index">the first index the elements will be copied to</param>
        public void CopyTo(Array array, int index) => addressFrames.CopyTo((AddressFrame[])array, index);

        /// <summary>
        /// push a single AddressFrame onto the top of the state
        /// equivalent to State.Add()
        /// </summary>
        /// <param name="frame">the frame to add to the state</param>
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

        /// <summary>
        /// add two States together
        /// </summary>
        public static State operator +(State a, State b)
        {
            AddressFrame[] frames = new AddressFrame[a.Count + b.Count];
            a.CopyTo(frames, 0);
            b.CopyTo(frames, a.Count - 1);
            return new State(frames);
        }

        /// <summary>
        /// add two States together
        /// </summary>
        public static State operator +(State a, AddressFrame b)
        {
            AddressFrame[] frames = new AddressFrame[a.Count + 1];
            a.CopyTo(frames, 0);
            frames[^1] = b;
            return new State(frames);
        }

        /// <summary>
        /// substract one State from another
        /// </summary>
        public static State operator -(State minuend, State subtrahend)
        {
            //todo: this doesn't work the way it should
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

        /// <summary>
        /// Append multiple AddressFrames to the state
        /// </summary>
        /// <param name="item">an IEnumerable containing the AddressFrames to append</param>
        public void Append(IEnumerable<AddressFrame> item) => addressFrames.AddRange(item);

        /// <summary>
        /// adds an AddressFrame to the end of the State
        /// </summary>
        /// <param name="item">the AddressFrame to append to the end</param>
        public void Add(AddressFrame item) => addressFrames.Add(item);

        /// <summary>
        /// clear the contents of the State
        /// </summary>
        public void Clear() => addressFrames.Clear();
        /// <summary>
        /// check whether the State contains a specific AddressFrame
        /// </summary>
        /// <param name="item">the AddressFrame which should be searched for</param>
        /// <returns>true if the AddressFrame exists in the State, otherwise false</returns>
        public bool Contains(AddressFrame item) => addressFrames.Contains(item);

        public bool ContainsAdType(AddressType type)
        {
            foreach (var i in addressFrames)
                if (i.Type == type) return true;
            return false;
        }

        /// <summary>
        /// copy the State to a new AddressFrame array
        /// </summary>
        /// <param name="array">the array to copy the State to</param>
        /// <param name="arrayIndex">the first index the elements will be copied to</param>
        public void CopyTo(AddressFrame[] array, int arrayIndex) => addressFrames.CopyTo(array, arrayIndex);

        /// <summary>
        /// remove the specified AddressFrame from the State
        /// </summary>
        /// <param name="item">the AddressFrame to remove</param>
        public bool Remove(AddressFrame item) => addressFrames.Remove(item);
        /// <summary>
        /// Remove a State from the end of this State
        /// </summary>
        /// <param name="item">the State to substract from this instance</param>
        public void Remove(State item) => addressFrames = (this - item).addressFrames;

        /// <summary>
        /// check whether the State is a child of <paramref name="item"/>
        /// </summary>
        /// <param name="item">the State which should be compared to</param>
        public bool IsChildOf(State item)
        {
            if (item.Count <= Count) return false;
            for (int i = 0; i < Count; i++)
            {
                if (addressFrames[i] == item[i]) continue;
                else return false;
            }
            return true;
        }

        /// <summary>
        /// check whether the State is a parent of <paramref name="item"/>
        /// </summary>
        /// <param name="item">the State which should be compared to</param>
        public bool IsParentOf(State item)
        {
            if (item.Count >= Count) return false;
            for (int i = 0; i < item.Count; i++)
            {
                if (addressFrames[i] == item[i]) continue;
                else return false;
            }
            return true;
        }

        /// <summary>
        /// get the library of the State
        /// </summary>
        public State GetLibrary()
        {
            int num = 0;
            foreach (var i in addressFrames)
            {
                num++;
                if (i.Type != AddressType.LibraryDecl) break;
            }
            AddressFrame[] libraryFrames = new AddressFrame[num];
            addressFrames.CopyTo(0, libraryFrames, 0, num);
            return new State(libraryFrames);
        }

        /// <summary>
        /// returns the AddressFrame at the index i
        /// </summary>
        public AddressFrame this[int i]
        {
            get => addressFrames[i];
        }

        /// <summary>
        /// returns the AddressFrame at the index i
        /// </summary>
        public AddressFrame[] this[Range r]
        {
            get
            {
                if (addressFrames.Count < 2)
                    return Array.Empty<AddressFrame>();
                return addressFrames.ToArray()[r];
            }
        }

        /// <summary>
        /// convert the State to a string of the AddressFrames joined by dots
        /// </summary>
        public override string ToString() => string.Join('.', addressFrames);

        public object Clone()
        {
            var list = new List<AddressFrame>(addressFrames.Count);
            addressFrames.ForEach((item) => list.Add(item with { }));
            return new State(list);
        }
    }
}