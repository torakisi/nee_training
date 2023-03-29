using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core
{
    public static class Extensions
    {
        /// <summary>
        /// truncates a string at a specific maxLength if longer
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Truncate(this string instance, int maxLength)
        {
            if (instance != null)
                instance = (instance.Length <= maxLength) ? instance : instance.Substring(0, maxLength);
            return instance;
        }

        /// <summary>
        /// Gets Exception Message String including Inner-Exceptions in a Multi-Line form.
        /// </summary>
        /// <param name="instance">the Exception object</param>
        /// <returns></returns>
        public static string GetMultiLineExceptionMessage(this Exception instance)
        {
            var ret = instance?.Message;
            if (instance?.InnerException != null)
            {
                ret = "";
                for (int i = 0; instance?.Message != null; i++)
                {
                    ret += string.Format("{0}{1}: {2}", ((i == 0) ? "" : "\r\n"), i, instance.Message);
                    instance = instance.InnerException;
                }
            }
            return ret;
        }

        /// <summary>
        /// Gets Exception Message String including Inner-Exceptions in a Single-Line form.
        /// </summary>
        /// <param name="instance">the Exception object</param>
        /// <returns></returns>
        ///
        public static string GetSingleLineExceptionMessage(this Exception instance)
        {
            var ret = instance?.Message;
            if (instance?.InnerException != null)
            {
                for (int i = 1; instance?.InnerException?.Message != null; i++)
                {
                    instance = instance.InnerException;
                    ret += string.Format(", Inner-Exception-{0}: {1}", i, instance.Message);
                }
            }
            return ret;
        }

        public static string TrimAndNullIfEmpty(this string instance) => string.IsNullOrWhiteSpace(instance) ? null : instance.Trim();

        public static string PadRightAndTruncate(this string instance, int totalWidth)
        {
            //if (string.IsNullOrWhiteSpace(instance)) return null;
            if (string.IsNullOrWhiteSpace(instance)) return new string(' ', totalWidth);
            instance = instance.PadRight(totalWidth);
            if (instance.Length > totalWidth) instance = instance.Substring(0, totalWidth);
            return instance;
        }

        public static string FmtInt(this int instance) => instance.ToString("#,##0");
        public static string FmtInt(this int? instance) => (instance == null) ? null : instance.Value.FmtInt();

        public static string FmtMoney(this decimal instance) => instance.ToString("#,##0.00");
        public static string FmtMoney(this decimal? instance) => (instance == null) ? null : instance.Value.FmtMoney();

        public static string FmtEuros(this decimal instance) => instance.ToString("#,##0.00 €");
        public static string FmtEuros(this decimal? instance) => (instance == null) ? null : instance.Value.FmtEuros();

        public static string FmtDateOnly(this DateTime instance) => instance.ToString("dd-MM-yyyy");
        public static string FmtDateOnly(this DateTime? instance) => (instance == null) ? null : instance.Value.FmtDateOnly();

        public static DateTime FmtDateFirstDayOfMonth(this DateTime instance)
        {

            return new DateTime(instance.Year, instance.Month, 1);

        }

        public static string FmtDateTime(this DateTime instance) => instance.ToString("dd-MM-yyyy HH:mm:ss");
        public static string FmtDateTime(this DateTime? instance) => (instance == null) ? null : instance.Value.FmtDateTime();

        public static int NonNegative(this int instance) => (instance < 0) ? 0 : instance;
        public static int? NonNegative(this int? instance) => (instance == null) ? (int?)null : instance.Value.NonNegative();

        public static decimal NonNegative(this decimal instance) => (instance < 0) ? 0 : instance;
        public static decimal? NonNegative(this decimal? instance) => (instance == null) ? (decimal?)null : instance.Value.NonNegative();


        public static string GetToken(this string separator, ref string source)
        {
            if (string.IsNullOrEmpty(separator)) throw new ArgumentException("separator/instance null or empty");
            var idx = source.IndexOf(separator);
            if (idx < 0) idx = source.Length;
            var ret = source.Substring(0, idx);
            source = source.Substring(idx + separator.Length);
            return ret;
        }

        public static string GetTokenTrimmed(this string separator, ref string source)
        {
            separator = separator.TrimAndNullIfEmpty();
            if (string.IsNullOrEmpty(separator)) throw new ArgumentException("separator/instance null or empty");
            source = source.Trim();
            var idx = source.IndexOf(separator);
            if (idx < 0) idx = source.Length;
            var ret = source.Substring(0, idx).Trim();
            source = source.Substring(idx + separator.Length).Trim();
            return ret;
        }

        public static int MonthsInsuredBetween(this DateTime instance, DateTime futureDate)
        {
            if (futureDate < instance)
                return 0;
            var duration = (futureDate.Date.AddDays(1) - instance.Date);
            if (duration.TotalDays < 22)
                return 0;
            if (duration.TotalDays <= 31)
                return 1;

            return duration.Days / 30;
        }

        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static string GetDisplayName(this Enum enumValue)
        {
            if (enumValue != null)
            {
                var enumObj = enumValue.GetType()
                           .GetMember(enumValue?.ToString())
                           .FirstOrDefault();

                if (enumObj != null)
                {
                    return enumObj
                        .GetCustomAttribute<DisplayAttribute>()
                        .GetName();
                }
            }

            return "";
        }

        public static IDictionary<string, object> AddRange(this IDictionary<string, object> instance, IDictionary<string, object> items)
        {
            if ((items != null) && items.Any())
            {
                foreach (var item in items)
                    instance.Add(item);
            }
            return instance;
        }

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                result.Add(property.Name, property.GetValue(obj));
            }
            return result;
        }
    }
}
