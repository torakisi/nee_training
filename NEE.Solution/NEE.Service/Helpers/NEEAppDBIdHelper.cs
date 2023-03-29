using NEE.Core.Helpers;
using NEE.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NEE.Service.Helpers
{
    public class NEEAppDBIdHelper : INEEAppIdCreator
    {
        private static Regex _regex = new Regex(@"\d\d\d\d-\d\d\d\d-\d\d\d\d-\d\d\d\d", RegexOptions.Compiled);

        private readonly NEEDbContextFactory dbFactory;

        public NEEAppDBIdHelper(NEEDbContextFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        private NEEDbContext CreateDb(string methodName) => dbFactory.Create(methodName, ApplicationConfigurationHelper.IsDbLoggingEnabled);

        /// <summary>
        /// gets a datetime and returns a string: "yyMM-ddHH-mmXX-XXXX", XXXXXX from database sequence "AppIdSequence"
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string CreateIdFromDateTime(DateTime dt)
        {
            decimal seqId = GetNextSeqIdFromDB();

            string seqIdPart = seqId.ToString().PadLeft(6, '0');

            string datePart = GenerateDatePart(dt);

            string appId = SplitWithDashes($"{datePart}{seqIdPart}");


            return appId;

        }

        private decimal GetNextSeqIdFromDB()
        {
            using (var db = CreateDb("GetNextSeqIdFromDB"))
            {
                decimal nextID = db.Database.SqlQuery<decimal>(@"SELECT ""AppIdSequence"".nextval FROM dual").First();
                return nextID;
            }
        }
        private string GenerateDatePart(DateTime dt)
        {
            return dt.ToString($"yyMMddHHmm");
        }
        private string SplitWithDashes(string id)
        {
            return id.Substring(0, 4) + '-'
                + id.Substring(4, 4) + '-'
                + id.Substring(8, 4) + '-'
                + id.Substring(12, 4);
        }

        /// <summary>
        /// gets a string in the form "yyMM-ddHH-mmXX-XXXX" and returns a datetime (for 20xx years of cource) truncated to minute precision
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Null if invalid string, else the datetime</returns>
        public DateTime? GetDateTimeFromId(string id)
        {
            if (!_regex.IsMatch(id)) return null;
            DateTime ret;
            var val = "20" + id.Replace("-", "");
            if (!DateTime.TryParseExact(val.Substring(0, 12), "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.AssumeLocal, out ret)) return null;
            return ret;
        }
    }
}
