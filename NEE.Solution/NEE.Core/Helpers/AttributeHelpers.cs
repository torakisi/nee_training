using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NEE.Core.Helpers
{
    public static class AttributeHelpers
    {

        public static DisplayAttribute GetDisplayAttribute(object value)
        {
            if (value == null) return null;
            var type = value.GetType();
            var member = type.GetMember(value.ToString()).FirstOrDefault();
            DisplayAttribute ret;
            if (member != null)
                ret = member.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
            else
                ret = type.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
            return ret;
        }

        public static string GetDisplayName(object value) => GetDisplayAttribute(value)?.Name;

    }
}
