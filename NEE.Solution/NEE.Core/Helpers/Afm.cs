using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.Helpers
{
    public class Afm : StringValueObject<Afm>
    {
        public Afm(string afm)
            : base(afm)
        {
            if (!IsValid(afm))
                throw new ArgumentException($"Ο ΑΦΜ ({afm}) δεν είναι αποδεκτός");
        }

        public static Option<Afm> TryParse(string afm) =>
            IsValid(afm)
                ? Option.Create(new Afm(afm))
                : Option.Empty<Afm>();

        public static explicit operator Afm(string afm) => new Afm(afm);
        public static implicit operator string(Afm afm) => afm?.Value;


        public static bool IsValid(string afm)
        {
            if (string.IsNullOrWhiteSpace(afm))
                return false;

            bool res = false;
            if (afm.Trim().Length != 9)
                res = false;
            else
            {
                var digits = afm.ToCharArray();
                int checkDigit = digits[8] - 48;
                long sum = ((digits[7] - 48) << 1) +
                    ((digits[6] - 48) << 2) +
                    ((digits[5] - 48) << 3) +
                    ((digits[4] - 48) << 4) +
                    ((digits[3] - 48) << 5) +
                    ((digits[2] - 48) << 6) +
                    ((digits[1] - 48) << 7) +
                    ((digits[0] - 48) << 8);
                long mod = sum % 11;
                if (mod == 10)
                    mod = 0;
                res = (mod == checkDigit);
            }
            return res;
        }
    }
}
