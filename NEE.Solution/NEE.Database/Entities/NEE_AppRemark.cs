using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database
{
    public class NEE_AppRemark : IProvideCreatedAndModified
    {
        [Key, Column(Order = 0)]
        [Required]
        [MaxLength(29)]
        public string Id { get; set; }

        public static class RemarkGroupNames
        {
            /// <summary>
            /// Constant: the system remark validation group name
            /// </summary>
            public const string System = "(system)";

            /// <summary>
            /// Constant: the default remark validation group name
            /// </summary>
            public const string Default = "default";

            /// <summary>
            /// Constant: the audit remark validation group name
            /// </summary>
            public const string Audit = "audit";
        }

        ///// <summary>
        ///// Name of the validation group (ex: "default")
        ///// </summary>
        [Key, Column(Order = 1)]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        ///// <summary>
        ///// Index of the remark within the validation group (1,2,3,...)
        ///// </summary>
        [Key, Column(Order = 2)]
        [Required]
        public int Index { get; set; }


        //	--- Payload: General ---


        [Required]
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

        [MaxLength(1000)]
        public string ReleaseText { get; set; }

        public DateTime? ReleasedAt { get; set; }

        [MaxLength(256)]
        public string ReleasedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string EntityId { get; set; }
        public int Revision { get; set; }
        public bool ReferToMember { get; set; }

        public static bool RemarkEquals(NEE_AppRemark a, NEE_AppRemark b)
        {
            if (a == b) return true;        // same reference => equal

            if
            (
                   (a.Id == b.Id)
                && (a.RemarkCode == b.RemarkCode)
                && (a.Description == b.Description)
                && (a.Status == b.Status)
                && (a.Severity == b.Severity)
                && (a.Message == b.Message)
                && (a.RelatedAMKA == b.RelatedAMKA)
                && (a.RelatedAFM == b.RelatedAFM)
                && (a.Released == b.Released)
                && (a.ReleaseText == b.ReleaseText)
                && (a.ReleasedAt == b.ReleasedAt)
                && (a.ReleasedBy == b.ReleasedBy)
            )
            {
                return true;    // same values => equal
            }

            return false;   // not equal
        }

        public static bool RemarkListEquals(List<NEE_AppRemark> list_a, List<NEE_AppRemark> list_b)
        {
            if (list_a == list_b) return true;                  // same reference => equal
            if (list_a.Count != list_b.Count) return false;     // different Count => diff
            for (int i = 0; i < list_a.Count; i++)
            {
                if (!RemarkEquals(list_a[i], list_b[i])) return false;   // different item => diff
            }
            return true;    // equal
        }
    }
}
