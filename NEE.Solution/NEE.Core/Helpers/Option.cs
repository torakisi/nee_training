using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.Helpers
{
    public sealed class Option<T> : IEnumerable<T>
    {
        private readonly T[] data;
        private Option(T[] data) { this.data = data; }
        public static Option<T> Create(T element) => element == null ? Empty() : new Option<T>(new T[] { element });
        public static Option<T> Empty() => new Option<T>(new T[0]);
        public bool IsEmpty => data.Length == 0;
        public bool HasValue => data.Length == 1;
        public override string ToString() => this.Any() ? data[0].ToString() : null;

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)data).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public static implicit operator Option<T>(T element) => Create(element);
        public static implicit operator Option<T>(None none) => Empty();

        public T GetValueOrDefault() => !IsEmpty ? data[0] : default(T);
        public T Value
        {
            get
            {
                if (IsEmpty) throw new InvalidOperationException("Value does not exist because Option is empty");
                return data[0];
            }
        }
    }

    public sealed class None
    {
    }
    public static class Option
    {
        public static None None => new None();
        public static Option<T> Create<T>(T element) => element == null ? Empty<T>() : Option<T>.Create(element);
        public static Option<T> Empty<T>() => Option<T>.Empty();

        public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate) =>
            option.HasValue && predicate(option.Value)
                ? option
                : None;

        /// <summary>
        /// Wraps the specified function within a try/catch block and returns empty option value if 
        /// an exception is thrown when calling the function
        /// </summary>
        public static Option<T> Try<T>(Func<T> fn)
        {
            try
            {
                return Create(fn());
            }
            catch
            {
                return Empty<T>();
            }
        }

        public static Option<T> If<T>(bool condition, Func<T> fn) =>
            condition ? Create(fn()) : Empty<T>();
        public static Option<T> If<T>(this T value, Func<T, bool> fn) =>
            fn(value) ? Create(value) : Empty<T>();

        public static Option<T> If<T>(this Option<T> option, Func<T, bool> fn) =>
            option.HasValue
                ? option.Value.If(fn)
                : option;

        public static Option<T> Do<T>(this Option<T> option, Action<T> action)
        {
            if (option.HasValue)
                action(option.Value);
            return option;
        }
    }
}
