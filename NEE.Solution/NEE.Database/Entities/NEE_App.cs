using NEE.Core.Contracts.Enumerations;
using NEE.Database.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEE.Database
{
    public class NEE_App : IProvideCreatedAndModified
    {
        public string Id { get; set; }
        public int Revision { get; set; }
        public int Version { get; set; }
        public AppState State { get; set; }
        public int ApplicationIndex { get; set; }
        public string AFM { get; set; }
        public string AMKA { get; set; }
        public string SpouseAFM { get; set; }
        public string SpouseAMKA { get; set; }
        public string IBAN { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
        public decimal? Amount { get; set; }
        public bool DeclarationLaw1599 { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public DateTime? DraftAt { get; set; }
        public string DraftBy { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string CanceledBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string RejectedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public string ArchiveReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? PayFrom { get; set; }
        public DateTime? PayTo { get; set; }
        public bool IsCreatedByKK { get; set; }
        public decimal? EntitledAmount { get; set; }
        public DateTime? RecalledAt { get; set; }
        public string RecalledBy { get; set; }
        public bool IsModifiedByKK { get; set; }
        public DateTime? InitialPayTo { get; set; }
        public DateTime? InitialPeriodTo { get; set; }
        public List<NEE_AppPerson> Members { get; set; } = new List<NEE_AppPerson>();
        public List<NEE_AppRemark> Remarks { get; set; } = new List<NEE_AppRemark>();
        public List<NEE_AppFile> Files { get; set; } = new List<NEE_AppFile>();
        public List<NEE_AppStateChange> ApplicationStateChange { get; set; } = new List<NEE_AppStateChange>();
        public IbanValidationServiceResult? IbanValidationResult { get; set; }
        [NotMapped]
        public string EntityId { get; set; }
        public string PensionDocumentAlbaniaId { get; set; }
        public string SpousePensionDocumentAlbaniaId { get; set; }
        public string MaritalStatusId { get; set; }
        public string FEKDocumentId { get; set; }
        public bool ProvidedFEKDocument { get; set; }        
        public string FEKDocumentId2 { get; set; }
        public string MaritalStatusId2 { get; set; }
        public string PensionDocumentAlbaniaId2 { get; set; }
        public string SpousePensDocumentAlbaniaId2 { get; set; }
    }
}

