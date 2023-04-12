using NEE.Core;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Validation;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using static NEE.Web.Models.Admin.SearchViewModel;

namespace NEE.Web.Models.Admin
{
    public class SearchViewModel
    {
        public class StateList
        {
            public int StateId { get; set; }
            public string State { get; set; }

        }

        public class DistrictList
        {
            public int DistrictId { get; set; }
            public string District { get; set; }

        }

        public bool IsEmpty =>
            string.IsNullOrWhiteSpace(Id)
            && string.IsNullOrWhiteSpace(AMKA)
            && string.IsNullOrWhiteSpace(AFM)
            && string.IsNullOrWhiteSpace(IBAN)
            && string.IsNullOrWhiteSpace(Email)
            && string.IsNullOrWhiteSpace(MobilePhone)
            && string.IsNullOrWhiteSpace(HomePhone)
            && string.IsNullOrWhiteSpace(LastName)
            && string.IsNullOrWhiteSpace(FirstName)
            && string.IsNullOrWhiteSpace(Zip)
            && string.IsNullOrWhiteSpace(City)
            ;

        [Display(Name = "Κωδικός Αίτησης", Prompt = @"Κωδικός Αίτησης (xxxx-xxxx-xxxx-xxxx)"), RegularExpression(@"\d\d\d\d-\d\d\d\d-\d\d\d\d-\d\d\d\d", ErrorMessage = "Ο \"{0}\" πρέπει να έχει τη μορφή: xxxx-xxxx-xxxx-xxxx")]
        public string Id { get; set; }

        [Display(Name = "ΑΜΚΑ", Prompt = @"ΑΜΚΑ (11 ψηφία)"), NEEStringLength(11, 11)]
        [StringLength(11, ErrorMessage = "ΑΜΚΑ (11 ψηφία)")]
        public string AMKA { get; set; }

        [Display(Name = "ΑΦΜ", Prompt = @"ΑΦΜ (9 ψηφία)"), NEEStringLength(9, 9)]
        [StringLength(9, ErrorMessage = "ΑΦΜ (9 ψηφία)")]
        public string AFM { get; set; }

        [Display(Name = "Επώνυμο", Prompt = "Επώνυμο (να ξεκινάει με)")]
        public string LastName { get; set; }

        [Display(Name = "Όνομα", Prompt = "Όνομα (να ξεκινάει με)")]
        public string FirstName { get; set; }

        [Display(Name = "Δήμος Κατοικίας", Prompt = "Δήμος (να περιέχει)")]
        public string City { get; set; }

        [Display(Name = "ΤΚ Κατοικίας", Prompt = "ΤΚ")]
        public string Zip { get; set; }

        [Display(Name = "IBAN", Prompt = @"IBAN")]//, IBAN]
        public string IBAN { get; set; }

        [Display(Name = "E-mail", Prompt = @"E-mail")]
        public string Email { get; set; }

        [Display(Name = "Κινητό Τηλέφωνο", Prompt = "Αριθμός Κινητού Τηλεφώνου")]
        public string MobilePhone { get; set; }

        [Display(Name = "Σταθερό Τηλέφωνο", Prompt = "Αριθμός Σταθερού Τηλεφώνου")]
        public string HomePhone { get; set; }
        [Display(Name = "Αναζήτηση σε όλα τα μέλη")]
        public bool SearchInAppPerson { get; set; }
        public int StateId { get; set; } = -2;
        [Display(Name = "Κατάσταση Αίτησης")]
        public List<StateList> StatesList
        {
            get
            {
                List<StateList> enums = ((AppState[])Enum.GetValues(typeof(AppState))).Select(c => new StateList() { StateId = (int)c, State = c.GetDisplayName() }).Where(x => x.StateId != -1).OrderBy(x => x.StateId).ToList();
                enums.Insert(0, new StateList { StateId = -2, State = "Όλες" });

                return enums;
            }

        }
        public string ApplicationOrigin { get; set; }
        [Display(Name = "Αιτήσεις από")]
        public List<string> ApplicationOriginList
        {
            get
            {
                return new List<string> { "Πολίτες", "Κέντρα Κοινότητας" };
            }
        }
        public bool SubmittedByKK
        {
            get
            {
                return ApplicationOrigin != "Πολίτες";
            }
        }
        [Display(Name = "Περιφέρεια ΟΠΕΚΑ")]
        public List<DistrictList> DistrictsList
        {
            get
            {
                List<DistrictList> enums = ((OpekaDistricts[])Enum.GetValues(typeof(OpekaDistricts))).Select(c => new DistrictList() { DistrictId = (int)c, District = c.GetDisplayName() }).OrderBy(x => x.DistrictId).ToList();
                return enums;                
            }
        }

        public int DistrictId { get; set; }
        public int? UserDistrictId { get; set; }
        public int DistrictIdFilter { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; } = 50;
        public int Total { get; set; }

        public bool HasResults { get; set; }
        public bool IsOpekaUser { 
            get
            {
                return IsInRole("OpekaNEEUsers");
            }
        }
        public bool IsCentralRegionUser
        {
            get
            {
                return UserDistrictId == 0;
            }
        }
        public bool CanEditResults { get; set; } = true;
        public bool IsInRole(string role) => HttpContext.Current.GetOwinContext().Request.User.IsInRole(role);

        public string Summary =>
            Results == null ? "" :
            Total == 0 ? "Δεν βρέθηκαν πρόσωπα με τα κριτήρια που επιλέξατε στις αιτήσεις" :
            Total == 1 ? "Βρέθηκε 1 πρόσωπο που πληροί τα στοιχεία αναζήτησης σε αίτηση" :
            Total == Results.Count ? $"Βρέθηκαν {Total.ToString("#,##0")} πρόσωπα που πληρούν τα στοιχεία αναζήτησης σε αιτήσεις." :
            $"Βρέθηκαν {Total.ToString("#,##0")} πρόσωπα που πληρούν τα στοιχεία αναζήτησης, εμφανίζονται οι {Results.Count.ToString("#,##0")} πιο πρόσφατες εγγραφές";

        public List<SearchResultTableViewModel> Results { get; set; }
        public IPagedList<SearchResultTableViewModel> PagedResults { get; set; }

        public string GetUrlForSearch()
        {
            string originalUrl = HttpContext.Current.Request.Url.AbsolutePath;
            if (HttpContext.Current.Request.Url.Query.Length > 0)
                originalUrl = originalUrl.Replace(HttpContext.Current.Request.Url.Query, string.Empty);

            string urlString = originalUrl + "?" + CreateSearchCriteriaQueryParameters();
            return HttpUtility.UrlEncode(urlString);
        }

        public string CreateSearchCriteriaQueryParameters()
        {

            string ret = "";
            if (!string.IsNullOrEmpty(Id))
            {
                ret = ret + "AppId=" + HttpContext.Current.Server.HtmlEncode(Id) + "&";
            }
            if (!string.IsNullOrEmpty(AMKA))
            {
                ret = ret + "AMKA=" + HttpContext.Current.Server.HtmlEncode(AMKA) + "&";
            }
            if (!string.IsNullOrEmpty(AFM))
            {
                ret = ret + "AFM=" + HttpContext.Current.Server.HtmlEncode(AFM) + "&";
            }
            if (!string.IsNullOrEmpty(IBAN))
            {
                ret = ret + "IBAN=" + HttpContext.Current.Server.HtmlEncode(IBAN) + "&";
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                ret = ret + "LastName=" + HttpContext.Current.Server.HtmlEncode(LastName) + "&";
            }
            if (!string.IsNullOrEmpty(FirstName))
            {
                ret = ret + "FirstName=" + HttpContext.Current.Server.HtmlEncode(FirstName) + "&";
            }
            if (!string.IsNullOrEmpty(City))
            {
                ret = ret + "City=" + HttpContext.Current.Server.HtmlEncode(City) + "&";
            }
            if (!string.IsNullOrEmpty(Zip))
            {
                ret = ret + "Zip=" + HttpContext.Current.Server.HtmlEncode(Zip) + "&";
            }
            if (!string.IsNullOrEmpty(Email))
            {
                ret = ret + "Email=" + HttpContext.Current.Server.HtmlEncode(Email) + "&";
            }
            if (!string.IsNullOrEmpty(MobilePhone))
            {
                ret = ret + "MobilePhone=" + HttpContext.Current.Server.HtmlEncode(MobilePhone) + "&";
            }
            if (!string.IsNullOrEmpty(HomePhone))
            {
                ret = ret + "HomePhone=" + HttpContext.Current.Server.HtmlEncode(HomePhone) + "&";
            }
            ret = ret + "SelState=" + HttpContext.Current.Server.HtmlEncode(Convert.ToString(StateId)) + "&";
            ret = ret + "SearchInPerson=" + Convert.ToString(SearchInAppPerson) + "&";
            ret = ret + "isSearched=1&";
            ret = ret + "CanEditResults=" + Convert.ToString(CanEditResults);
            if (!string.IsNullOrEmpty(ret))
            {
                ret.Remove(ret.Length - 1);
            }

            return ret;
        }

        public class SearchResultTableViewModel
        {
            public int Index { get; set; }
            public string Id { get; set; }
            public string AFM { get; set; }
            public string AMKA { get; set; }

            public string IBAN { get; set; }
            public string Email { get; set; }
            public string MobilePhone { get; set; }
            public string HomePhone { get; set; }
            public string Municipality { get; set; }
            public string Zip { get; set; }

            public AppState State { get; set; }

            public string StateDescription
            {
                get
                {
                    var descr = NEE.Core.Helpers.AttributeHelpers.GetDisplayName(State);
                    if (ApprovedAt.HasValue && State == AppState.Canceled)
                        descr = $"{descr} (Εγκεκριμένη)";
                    return descr;
                }
            }


            public string Applicant_LastName { get; set; }
            public string Applicant_FirstName { get; set; }
            public string Applicant_LastNameEN { get; set; }
            public string Applicant_FirstNameEN { get; set; }
            public string Applicant_CitizenCountry { get; set; }


            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string LastNameEN { get; set; }
            public string FirstNameEN { get; set; }
            public string CitizenCountry { get; set; }

            public string Applicant_FullName
            {
                get
                {
                    if (Applicant_CitizenCountry != "ΕΛΛΑΔΑ")
                    {
                        return $"{this.Applicant_LastNameEN} {this.Applicant_FirstNameEN}".Trim();
                    }

                    return $"{this.Applicant_LastName} {this.Applicant_FirstName}".Trim();
                }
            }

            public string FullName
            {
                get
                {
                    if (CitizenCountry != "ΕΛΛΑΔΑ")
                    {
                        return $"{this.LastNameEN} {this.FirstNameEN}".Trim();
                    }

                    return $"{this.LastName} {this.FirstName}".Trim();
                }
            }

            public DateTime? ApprovedAt { get; set; }
            public DateTime? RejectedAt { get; set; }
            public bool IsEditableApplicationSearch { get; set; }
            public bool CanViewOnlyApplicationSearch { get; set; }
            public OpekaDistricts DistrictId { get; set; }
            public string DistrictName { 
                get
                {
                    return DistrictId.GetDisplayName();
                }
            }

        }
    }
}