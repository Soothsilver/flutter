using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auxiliary
{
    /// <summary>
    /// Basically an improved list that provides access to the entire array, but provides methods for Push, Pop and Peek.
    /// </summary>
    /// <typeparam name="T">The type of elements on the stack.</typeparam>
    public class ImprovedStack<T>: List<T>
    {
        public T Peek()
        {
            if (this.Count > 0) return this[this.Count - 1]; else return default(T);
        }
        public void Push(T t)
        {
            this.Add(t);
        }

        public T Pop()
        {
            if (this.Count > 0)
            {
                T t = this[this.Count - 1];
                this.Remove(t);
                return t;
            }
            else return default(T);
        }
    }
}
