using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.Helpers
{
    public class Amka : StringValueObject<Amka>
    {
        public Amka(string amka)
            : base(amka)
        {
            if (!IsValid(amka))
                throw new ArgumentException("Ο ΑΜΚΑ δεν είναι αποδεκτός");
        }


        public static explicit operator Amka(string amka) => new Amka(amka);
        public static implicit operator string(Amka amka) => amka?.Value;


        public static bool IsValid(string amka) =>
            !string.IsNullOrWhiteSpace(amka)
            && amka.All(char.IsDigit)
            && amka.Length == 11
            && Digits(amka)
                .Select((digit, index) => IsOdd(index) ? 2 * digit : digit)
                .Select(SumOfDigitsIfAboveTen)
                .Sum() % 10 == 0;

        private static IEnumerable<int> Digits(string amka) => amka.Select(c => (int)char.GetNumericValue(c));
        private static bool IsOdd(int i) => i % 2 != 0;
        private static int SumOfDigitsIfAboveTen(int num) => num > 9 ? num - 9 : num;
    }
}
