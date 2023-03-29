using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NEE.Core.BO
{
    public class Remark
    {
        public static Dictionary<RemarkType, string> RemarkTypeReleaseTextDescription = new Dictionary<RemarkType, string>()
        {
            //{ RemarkType.SingleParentFamilyDeclaredButNotFromKEA, ApplicationValidationRule.SingleParentFamilyAction },
            //{ RemarkType.ForeignResident, ApplicationValidationRule.GreenCardAction },
            //{ RemarkType.MemberUnprotected, ApplicationValidationRule.UnprotectedChild },
            //{ RemarkType.IdentificationEndDateNotFound, ApplicationValidationRule.IdentificationEndDateRequired },
            //{ RemarkType.IdentificationNumberNotAdded, ApplicationValidationRule.IdentificationNumberRequired }
        };

        private static string _regexAitisiPattern = @"Αίτηση \d\d\d-\d\d\d-\d\d\d-\d\d\d-\d\d\d";
        private static string _regexAitisiReplace = @"Αίτηση xxx-xxx-xxx-xxx-xxx";
        private static Regex _regexAitisi = new Regex(_regexAitisiPattern, RegexOptions.Compiled);


        public RemarkType RemarkCode { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
        public int Status { get; set; }
        public NEERemarkSeverity Severity { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        [MaxLength(11)]
        public string RelatedAMKA { get; set; }
        [MaxLength(9)]
        public string RelatedAFM { get; set; }


        public bool? Released { get; set; }

        public string ReleaseText { get; set; }
        [MaxLength(256)]
        public string ReleasedBy { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public string EntityId { get; set; }
        public int Revision { get; set; }

        public string ReleaseTextDescription
        {
            get
            {
                if (RemarkTypeReleaseTextDescription.Keys.Contains(this.RemarkCode))
                    return RemarkTypeReleaseTextDescription[this.RemarkCode];

                return "";
            }
        }

        public RemarkSelection? ReleaseSelection
        {
            get
            {
                if (this.Released == null) return null;
                if (this.Released == false) return RemarkSelection.Reject;
                //if (this.ReleaseValue.GetValueOrDefault() == 2) return RemarkSelection.Approve2;
                return RemarkSelection.Approve;
            }
            set
            {
                if (value != null)
                {
                    switch (value.Value)
                    {
                        case RemarkSelection.Reject:
                            this.Released = false;
                            //this.ReleaseValue = null;
                            return;
                        case RemarkSelection.Approve:
                            this.Released = true;
                            //this.ReleaseValue = null;
                            return;
                        case RemarkSelection.Approve2:
                            this.Released = true;
                            // this.ReleaseValue = 2;
                            return;
                        default:
                            throw new ArgumentException();
                    }
                }
                this.Released = null;
                // this.ReleaseValue = null;
            }
        }

        public bool ApprovedByKK { get; set; }

        public bool IsFromDB { get; set; } = false;
        public bool ReferToMember { get; set; }

        // default constructor for mapping/serialization
        public Remark() { }

        public Remark(RemarkType code, string description, string message, NEERemarkSeverity severity, string amka, string afm, bool referToMember = false)
        {
            this.RemarkCode = code;
            this.Description = description;
            this.Severity = severity;
            this.Message = message;
            this.RelatedAMKA = amka;
            this.RelatedAFM = afm;
            this.ReferToMember = referToMember;
        }

        // Returns a unique hash code based on several column values
        public string DataHash
        {
            get
            {
                var description = _regexAitisi.Replace(this.Description ?? "", _regexAitisiReplace);

                var data = string.Format("<{0}|{1}|{2}|{3}|{4}|{5}|{6}>",
                                        this.RemarkCode,
                                        description,
                                        this.Status,
                                        this.Severity,
                                        this.Message,
                                        this.RelatedAMKA,
                                        this.RelatedAFM
                                    );
                var hash = data.GetHashCode();
                var ret = hash.ToString("x8");  // as 8 digit hex left-padded with zeroes if needed
                return ret;
            }
        }


        private string DebuggerDisplay => $"{RelatedAFM} {RelatedAMKA} | {RemarkCode}, {Description}";

        public Remark GetNewEntityId(INEEAppIdCreator idGenerator)
        {
            this.EntityId = "r-" + idGenerator.CreateIdFromDateTime(DateTime.Now);

            return this;
        }

        public string MemberFirstName { get; set; }
        public string MemberLastName { get; set; }
        public string FullNameWithAMKA
        {
            get
            {
                return $"{MemberLastName} {MemberFirstName} AMKA({RelatedAMKA})";
            }
        }

        public bool ReasonForNoApproval
        {
            get
            {
                return (Severity == NEERemarkSeverity.Low && (!Released.HasValue || (Released.HasValue && !Released.Value))) ||
                     (Severity == NEERemarkSeverity.High);
            }
        }

        public bool ReasonForNoSubmitted
        {
            get
            {
                return (Severity == NEERemarkSeverity.Medium);
            }
        }

        public string MessageHtmlEncode
        {
            get
            {
                return HttpUtility.HtmlDecode(Message);
            }
        }
    }
}
