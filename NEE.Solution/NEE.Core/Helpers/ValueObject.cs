using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.Helpers
{
    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            T other = obj as T;

            if (other == null)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            object[] values = this.GetValuesForHashCode();

            if (values == null || values.Length == 0)
                return GetHashCodeByReflection();

            return GetHashCodeForValues(values);
        }

        protected virtual object[] GetValuesForHashCode()
        {
            return null;
        }

        private int GetHashCodeForValues(params object[] values)
        {
            int startValue = 17;
            int multiplier = 59;

            int hashCode = startValue;
            unchecked
            {
                foreach (object value in values)
                {
                    if (value != null)
                        hashCode = hashCode * multiplier + value.GetHashCode();
                }

                return hashCode;
            }
        }

        private int GetHashCodeByReflection()
        {
            IEnumerable<FieldInfo> fields = GetFields();

            int startValue = 17;
            int multiplier = 59;

            int hashCode = startValue;

            unchecked
            {
                foreach (FieldInfo field in fields)
                {
                    object value = field.GetValue(this);

                    if (value != null)
                        hashCode = hashCode * multiplier + value.GetHashCode();
                }

                return hashCode;
            }
        }


        public virtual bool Equals(T other)
        {
            if (other == null)
                return false;

            Type t = GetType();
            Type otherType = other.GetType();

            if (t != otherType)
                return false;

            FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                object value1 = field.GetValue(other);
                object value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                    return false;
            }

            return true;
        }


        private IEnumerable<FieldInfo> GetFields()
        {
            Type t = GetType();

            List<FieldInfo> fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

                t = t.BaseType;
            }

            return fields;
        }

        public static bool operator ==(ValueObject<T> x, ValueObject<T> y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }
    }
}
