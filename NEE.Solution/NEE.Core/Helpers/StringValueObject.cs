using System;

namespace NEE.Core.Helpers
{
    public class StringValueObject<T> : ValueObject<T>
        where T : StringValueObject<T>
    {
        public string Value { get; }
        protected StringValueObject(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(nameof(value));
            Value = value;
        }

        public override string ToString() => Value;

        public override bool Equals(T other) =>
            (other == null)
            ? false
            : string.Equals(Value, other.Value);

        public override bool Equals(object obj)
        {
            if (obj is string) return string.Equals(Value, (string)obj);
            return base.Equals(obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(StringValueObject<T> x, string y)
        {
            return Equals(x, y);
        }
        public static bool operator ==(string x, StringValueObject<T> y)
        {
            return Equals(y, x);
        }
        public static bool operator !=(StringValueObject<T> x, string y)
        {
            return !(x == y);
        }
        public static bool operator !=(string x, StringValueObject<T> y)
        {
            return !(x == y);
        }
    }
}
