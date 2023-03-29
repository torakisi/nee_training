using System;

namespace NEE.Core.Helpers
{
    public enum IbanValidationResult
    {
        Valid = 0,
        InvalidCharacter = 1, // λαθος σε καποιο χαρακτηρα του ΙΒΑΝ
        Invalid = 2           // δεν ειναι ΙΒΑΝ π.χ. ενα τηλεφωνο
    }
    public class Iban : StringValueObject<Iban>
    {
        enum IbanCountry { AT, BE, DK, FI, FR, DE, GR, IS, IE, IT, LU, NL, NO, PL, PT, ES, SE, CH, GB, AD, GI, LI, MC, SM, CY, LT, MT, HU, SK, SI, RO, LV, CZ, EE, BG, SA, AE }
        enum IbanLength { AT = 20, BE = 16, DK = 18, FI = 18, FR = 27, DE = 22, GR = 27, IS = 26, IE = 22, IT = 27, LU = 20, NL = 18, NO = 15, PL = 28, PT = 25, ES = 24, SE = 24, CH = 21, GB = 22, AD = 24, GI = 23, LI = 21, MC = 27, SM = 27, CY = 28, LT = 20, MT = 31, HU = 28, SK = 24, SI = 19, RO = 24, LV = 21, CZ = 24, EE = 20, BG = 22, SA = 24, AE = 23 }

        public Iban(string iban)
            : base(TrimSpaces(iban))
        {
            if (!IsValid(iban))
                throw new ArgumentException("Invalid IBAN");
        }


        public static explicit operator Iban(string iban) => new Iban(iban);
        public static explicit operator string(Iban iban) => iban.Value;


        public static bool IsValid(string iban) => Validate(iban) == IbanValidationResult.Valid;


        private static string TrimSpaces(string iban)
        {
            return iban?.Replace(" ", "");
        }

        /// <summary>
        /// Ελεγχει την ορθοτητα ενός λογαριασμου IBAN.
        /// </summary>
        /// <param name="IBANAccount">The IBAN account.</param>
        /// <returns>
        /// 0 οκ
        /// 1 λαθος σε καποιο χαρακτηρα του ΙΒΑΝ
        /// 2 δεν ειναι ΙΒΑΝ π.χ. ενα τηλεφωνο
        /// </returns>
        public static IbanValidationResult Validate(string IBANaccount)
        {
            try
            {
                if (IBANaccount == null)
                    return IbanValidationResult.Invalid;

                string IBANTableStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                IbanValidationResult ValidIBAN;
                int IBANNumber;
                string LeftStr, RightStr, IBANChar;
                string CheckNum34Char;

                string remainingStr, stepStr;
                int step;
                long stepMod = 0;

                IBANaccount = IBANaccount.Trim();
                IBANaccount = IBANaccount.Replace(" ", "");

                ValidIBAN = IbanValidationResult.Invalid;
                int counter = 0;
                object ibanlengthobject = null;
                int ibanlength = 0;
                foreach (string country in Enum.GetNames(typeof(IbanCountry)))
                {
                    counter++;
                    if (country == IBANaccount.Substring(0, 2))
                    {
                        ibanlengthobject = Enum.Parse(typeof(IbanLength), country);
                        if (ibanlengthobject != null) ibanlength = (int)ibanlengthobject;
                        ValidIBAN = 0;
                        break;
                    }
                }
                if (ValidIBAN == IbanValidationResult.Invalid) return ValidIBAN;

                if (ibanlength == IBANaccount.Length)
                {
                    ValidIBAN = IbanValidationResult.Valid;
                }
                else
                {
                    ValidIBAN = IbanValidationResult.Invalid;
                    return ValidIBAN;
                }

                if (ValidIBAN == IbanValidationResult.Valid)
                {
                    CheckNum34Char = IBANaccount.Substring(2, 2);
                    if (IsNumeric(CheckNum34Char, typeof(Int32))) ValidIBAN = IbanValidationResult.Valid;
                    else
                    {
                        ValidIBAN = IbanValidationResult.Invalid; return ValidIBAN;
                    }

                }
                if (ValidIBAN == 0)
                {
                    int i = 1;
                    LeftStr = IBANaccount.Substring(0, 4);
                    //LeftStr = CommonLib.Left(IBANaccount, 4);
                    RightStr = IBANaccount.Substring(4, IBANaccount.Length - 4);
                    //RightStr = CommonLib.Right(IBANaccount, IBANaccount.Length - 4);
                    IBANaccount = RightStr + LeftStr;
                    while (i <= IBANaccount.Length)
                    {
                        IBANChar = IBANaccount.Substring(i - 1, 1);

                        if (!IsNumeric(IBANChar, typeof(Int32)))
                        {
                            if (IBANTableStr.IndexOf(IBANChar) == -1)
                            {
                                ValidIBAN = IbanValidationResult.Invalid; return ValidIBAN;
                            }
                            IBANNumber = IBANTableStr.IndexOf(IBANChar) + 1 + 9;
                            LeftStr = IBANaccount.Substring(0, i - 1);
                            //LeftStr = CommonLib.Left(IBANaccount, i - 1);

                            RightStr = IBANaccount.Substring(i, IBANaccount.Length - i);
                            //RightStr = CommonLib.Right(IBANaccount, IBANaccount.Length - i);
                            IBANaccount = LeftStr + IBANNumber.ToString() + RightStr;
                            i++;
                        }

                        i++;
                    }

                    if (ValidIBAN == 0)
                    {
                        remainingStr = "RemainingStr";
                        step = 9;

                        while (remainingStr != "")
                        {
                            if (IBANaccount.Length < step) step = IBANaccount.Length;
                            stepStr = IBANaccount.Substring(0, step);

                            remainingStr = IBANaccount.Substring(step, IBANaccount.Length - step);
                            //remainingStr = CommonLib.Right(step, IBANaccount.Length - step);

                            stepMod = Convert.ToInt64(stepStr) % 97;
                            IBANaccount = stepMod.ToString() + remainingStr;
                        }
                        if (stepMod == 1) ValidIBAN = 0;
                        else ValidIBAN = IbanValidationResult.InvalidCharacter;
                    }
                }

                return ValidIBAN;
            }
            catch (Exception ex)
            {
                return IbanValidationResult.Invalid;
            }
        }

        static bool IsNumeric(string value, Type numericdatatype)
        {
            Int32 int32res;
            Int64 int64;
            Double doubleres;

            if (numericdatatype == typeof(Int32))
            {
                return Int32.TryParse(value, out int32res);
            }
            else if (numericdatatype == typeof(Int64))
            {
                return Int64.TryParse(value, out int64);
            }
            else if (numericdatatype == typeof(Double))
            {
                return Double.TryParse(value, out doubleres);
            }
            return false;
        }

    }
}
