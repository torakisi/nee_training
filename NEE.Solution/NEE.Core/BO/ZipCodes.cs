using System;
using System.Collections.Generic;
using System.Linq;

namespace NEE.Core.BO
{
    public sealed class ZipCode
    {
        public string Code { get; set; }
        public string City { get; set; }
        public string District { get; set; }


        public ZipCode(string code, string city, string district)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException(nameof(code));
            code = code.Trim();
            int _;
            if (code.Length > 5 || !int.TryParse(code, out _))
                throw new ArgumentException(nameof(code));

            Code = code;
            City = city;
            District = district;

        }
        public ZipCode() { }
    }
    public class ZipCodes
    {
        private static ZipCodes _zipCodes = null;

        public static IEnumerable<ZipCode> Match(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Enumerable.Empty<ZipCode>();

            q = q.Trim();
            int _;
            if (q.Length > 5 || !int.TryParse(q, out _))
                return Enumerable.Empty<ZipCode>();

            var index = _zipCodes.indexes[q.Length - 1];
            if (!index.ContainsKey(q))
                return Enumerable.Empty<ZipCode>();

            return index[q];
        }
        public static IEnumerable<ZipCode> MatchCity(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Enumerable.Empty<ZipCode>();

            q = q.Trim();


            var index = _zipCodes.indexes[q.Length - 1];
            if (!index.ContainsKey(q))
                return Enumerable.Empty<ZipCode>();

            return index[q];
        }

        public static bool Exists(string code) => Match(code).Any();


        private readonly List<ZipCode> zipCodes;
        private readonly List<Dictionary<string, List<ZipCode>>> indexes;
        private readonly List<string> cities = new List<string>();
        private readonly List<string> districts = new List<string>();

        private static Dictionary<string, ZipCode> _EligibleZipCodes = new Dictionary<string, ZipCode>();


        // NOTE: you MUST call ZipCodes.SetData(null) at global.asax to work as before with internal hard-coded zip-codes.
        public static void SetData(List<ZipCode> items)     // null: use internal hard-coded list
        {
            _zipCodes = new ZipCodes(items);
            // _EligibleZipCodes = items.Where(x => x.IsGmiEligible).ToDictionary(x => BuildZipCodeKey(x.Code, x.City));
        }

        public static IEnumerable<string> Cities => _zipCodes.cities;
        public static IEnumerable<string> Districts => _zipCodes.districts;

        private ZipCodes(List<ZipCode> items)
        {
            zipCodes = items.OrderBy(z => z.Code).ToList();
            indexes = Enumerable.Range(1, 5)
                .Select(BuildIndex)
                .ToList();

            cities.Add("");
            cities.AddRange(zipCodes.Select(z => z.City).Distinct().OrderBy(x => x).ToList());

            districts.Add("");
            districts.AddRange(zipCodes.Select(z => z.District).Distinct().OrderBy(x => x).ToList());

        }
        private Dictionary<string, List<ZipCode>> BuildIndex(int length)
        {
            return zipCodes
                .GroupBy(z => z.Code.Substring(0, length))
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
