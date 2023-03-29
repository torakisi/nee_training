using HousingBenefit.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace XServices.Idika.Contract
{
    public class GetBenefitResponse : HBServiceResponseBase
	{

        // Actual Date Used as From Period
        public DateTime FromPeriod { get; set; }

        // Actual Date Used as To Period
        public DateTime ToPeriod { get; set; }

        /// <summary>
        /// Άντληση συνολικού ποσού από Προνοιακά επιδόματα τους τελευταίους ΧΧ μήνες
        /// </summary>
        /// <remarks>
        /// Πεδίο [Προνοιακά επιδόμ.]. Είναι το σύνολο των εισοδημάτων από προνοιακά επιδόματα, τους τελευταίους ΧΧ μήνες. Προ-συμπληρώνεται. Εξαιρούνται τα επιδόματα που έχει οριστεί ότι δεν περιλαμβάνονται στο πραγματικό εισόδημα. Τα στοιχεία αντλούνται από το Benefits Registry.
        /// </remarks>
        public decimal TotalBenefitAmount { get; set; }

    }
}
