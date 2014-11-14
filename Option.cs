using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.Caching
{
    /// <summary>
    /// An F#/Scheme inspired Option type, for values that are may or may not be present.
    /// </summary>
    public class Option<T> : IEnumerable<T>, IEquatable<T>, IEquatable<Option<T>>
    {
        public static readonly Option<T> None = new Option<T>();
        
        readonly bool isSome;
        readonly T value;

        public bool IsSome { get { return isSome; } }
        public T Value 
        { 
            get 
            {
                if (isSome)
                    return value;
                else
                    throw new InvalidOperationException("Cannot access the value of Option.None"); 
            }
        }




        #region Constructors - either construct with a value or not

        internal Option()
        {
            //this.value = NullVal;
            isSome = false;
        }

        internal Option(T value)
        {
            this.value = value;
            isSome = true;
        }

        #endregion




        #region Equality and GetHashCode overrides

        public override bool Equals(object obj)
        {
            var other = obj as Option<T>;
            if (other == null)
                return false;

            return Equals(other);
        }

        public bool Equals(Option<T> other)
        {
            return Equals(other.value);
        }
        
        public bool Equals(T other)
        {
            return value.Equals(other);
        }

        public static bool operator ==(Option<T> t1, Option<T> t2)
        {
            if (object.ReferenceEquals(t1, t2)) return true;
            if (object.ReferenceEquals(t1, null)) return false;
            if (object.ReferenceEquals(t2, null)) return false;

            return t1.Equals(t2);
        }

        public static bool operator !=(Option<T> t1, Option<T> t2)
        {
            if (object.ReferenceEquals(t1, t2)) return false;
            if (object.ReferenceEquals(t1, null)) return true;
            if (object.ReferenceEquals(t2, null)) return true;

            return !t1.Equals(t2);
        }

        public override int GetHashCode()
        {
            if (isSome)
                return value.GetHashCode();

            else
                return 0;// typeof(T).GetHashCode();// false.GetHashCode();
        }

        #endregion




        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            if (isSome)
                yield return value;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion




        public override string ToString()
        {
            return isSome ? value.ToString() : string.Format("Option<{0}>.None", typeof(T).Name);
        }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException("You cannot instantiate a reference Option with null");
            
            return new Option<T>(value);
        }

        public static Option<T> SomeValueType<T>(T value)
            where T : struct
        {
            return new Option<T>(value);
        }

        public static Option<T> None<T>()
        {
            return Option<T>.None;
        }
    }
}