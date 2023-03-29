using NEE.Core.Contracts;
using System.Collections.Generic;
using System;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Validation;

namespace XServices.Idika
{
    public class GetOtherBenefitsInfoResponse : XServiceResponseBase
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
        public List<OtherBenefit> OtherBenefits { get; set; }

        public static GetOtherBenefitsInfoResponse Exception(Exception ex, string ServiceName)
        {
            var res = new GetOtherBenefitsInfoResponse();
            res.AddError(ErrorCategory.Unhandled, null, ex);
            res.AddError(ErrorCategory.UIDisplayedServiceCallFailure, String.Format(ServiceErrorMessages.UnableToCommunicateWithService, ServiceName));
            return res;
        }
    }

    public class OtherBenefit
    {
        public string BenefitCategoryCode { get; set; }
        public decimal Amount { get; set; }
    }
}
