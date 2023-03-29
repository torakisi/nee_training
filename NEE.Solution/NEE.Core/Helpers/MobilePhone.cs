using System.Text.RegularExpressions;

namespace NEE.Core.Helpers
{
    public class MobilePhone
    {
        public static bool IsValid(string number)
        {
            return Regex.Match(number, @"^\d{10}$").Success;
        }
    }
}

