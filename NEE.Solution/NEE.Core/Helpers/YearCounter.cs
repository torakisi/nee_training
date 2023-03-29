using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.Helpers
{
    public class YearCounter
    {
        private readonly DateTime until;

        public YearCounter(DateTime until)
        {
            this.until = until;
        }

        public int YearsFrom(DateTime date)
        {
            int years = 0;
            DateTime tmpdob = date;
            while (tmpdob.AddYears(1) <= until)
            {
                tmpdob = tmpdob.AddYears(1);
                years++;
            }
            return years;
        }

        public bool IsMoreThan(DateTime date, int years) => date.AddYears(years) <= until;

        public static YearCounter UntilToday() => new YearCounter(DateTime.Today);
    }
}
