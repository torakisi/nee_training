
using NEE.Core.BO.Metadata;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace NEE.Core.BO
{
    public class Application
    {
        #region application fields
        public List<Person> Members { get; set; }
        public Person Applicant { get; set; }
        public Person Spouse { get; set; }
        public List<StateChange> ApplicationStateChange { get; set; } = new List<StateChange>();
        public List<ApplicationLog> ApplicationLog { get; set; }
        public List<Remark> Remarks { get; set; }
        public List<AppFile> Files { get; set; }

        /// <summary>
        /// get remote host ip to pass to the Validate Iban WS
        /// </summary>
        public string RemoteHostIP { get; set; }
        public string Id { get; set; }
        public int Revision { get; set; }
        public int Version { get; set; }
        public AppState? State { get; private set; }
        public string AFM { get; set; }
        public string AMKA { get; set; }
        public string SpouseAFM { get; set; }
        public string SpouseAMKA { get; set; }
        public decimal? Amount { get; set; }
        public bool DeclarationLaw1599 { get; set; }
        public int ApplicationIndex { get; set; }        

        public DateTime? PeriodFrom { get; set; }
        public bool IsReadOnlyApplication { get; set; }
        public bool IsEditableApplication { get; set; }
        public bool CanViewOnlyApplication { get; set; }
        public bool LockedByKK { get; set; }
        public bool IsInFinalStatus { get; set; }
        public bool IsCreatedByKK { get; set; }
        public bool IsModifiedByKK { get; set; }
        public bool IsApplicationCreatedByKK => IsCreatedByKK;
        public bool IsApplicationModifiedByKK => IsModifiedByKK;
        public DateTime? DraftAt { get; set; }
        [MaxLength(256)]
        public string DraftBy { get; set; }
        public DateTime? CanceledAt { get; set; }
        [MaxLength(256)]
        public string CanceledBy { get; set; }
        public bool IsAdminUser { get; set; }
        public bool IsAFMUser { get; set; }
        public bool CanViewApplicationSubmittedDetails { get; set; }
        public bool CanHandleRemarks { get; set; }
        public DateTime? ApprovedAt { get; set; }
        [MaxLength(256)]
        public string ApprovedBy { get; set; }
        public DateTime? RejectedAt { get; set; }
        [MaxLength(256)]
        public string RejectedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        [MaxLength(256)]
        public string SubmittedBy { get; set; }
        public IbanValidationServiceResult? IbanValidationResult { get; set; }
        public bool ProvidedFEKDocument { get; set; }
        # region documents
        #region fek
        public string FEKDocumentId { get; set; }
        public string FEKDocumentId2 { get; set; }
        public string FEKDocumentName { get; set; }
        public string FEKDocumentRejectionReason { get; set; }
        public string FEKDocumentRejectionReason2 { get; set; }
        public bool? IsFEKDocumentApproved { get; set; }
        public bool HasFEKDocument 
        { 
            get
            {
                return FEKDocumentId != null;
            }
        }
        public bool HasFEKDocument2
        {
            get
            {
                return FEKDocumentId2 != null;
            }
        }
        public bool HasFEKDocumentDecision
        {
            get
            {
                return IsFEKDocumentApproved != null;
            }
        }
        #endregion

        #region marital status
        public string MaritalStatusId { get; set; }
        public string MaritalStatusId2 { get; set; }
        public string MaritalStatusDocumentName { get; set; }     
        public string MaritalStatusDocumentRejectionReason { get; set; }
        public string MaritalStatusDocumentRejectionReason2 { get; set; }
        public bool? IsMaritalStatusDocumentApproved { get; set; }
        public bool HasMaritalStatusDocument
        {
            get
            {
                return MaritalStatusId != null;
            }
        }
        public bool HasMaritalStatusDocument2
        {
            get
            {
                return MaritalStatusId2 != null;
            }
        }
        public bool HasMaritalStatusDocumentDecision
        {
            get
            {
                return IsMaritalStatusDocumentApproved != null;
            }
        }
        #endregion

        #region pension albania
        public string PensionDocumentAlbaniaId { get; set; }
        public string PensionDocumentAlbaniaId2 { get; set; }
        public string PensionDocumentAlbaniaName { get; set; }
        public string PensionAlbaniaDocumentRejectionReason { get; set; }
        public string PensionAlbaniaDocumentRejectionReason2 { get; set; }
        public bool? IsPensionAlbaniaDocumentApproved { get; set; }
        public bool HasPensionAlbaniaDocument
        {
            get
            {
                return PensionDocumentAlbaniaId != null;
            }
        }
        public bool HasPensionAlbaniaDocument2
        {
            get
            {
                return PensionDocumentAlbaniaId2 != null;
            }
        }
        public bool HasPensionAlbaniaDocumentDecision
        {
            get
            {
                return IsPensionAlbaniaDocumentApproved != null;
            }
        }
        #endregion

        #region spouse pension albania
        public string SpousePensionDocumentAlbaniaId { get; set; }
        public string SpousePensDocumentAlbaniaId2 { get; set; }
        public string SpousePensDocumentName { get; set; }
        public string SpousePensionDocumentRejectionReason { get; set; }
        public string SpousePensionDocumentRejectionReason2 { get; set; }
        public bool? IsSpousePensionDocumentApproved { get; set; }
        public bool HasSpousePensionDocument
        {
            get
            {
                return SpousePensionDocumentAlbaniaId != null;
            }
        }
        public bool HasSpousePensionDocument2
        {
            get
            {
                return SpousePensDocumentAlbaniaId2 != null;
            }
        }
        public bool HasSpousePensionDocumentDecision
        {
            get
            {
                return IsSpousePensionDocumentApproved != null;
            }
        }
        #endregion

        #endregion

        public bool CanBeSubmitted
        {
            get
            {
                if (Remarks.Any(x => x.ReasonForNoSubmitted))
                {
                    return false;
                }

                return true;
            }
        }
        public bool CanBeApproved
        {
            get
            {
                if (Remarks.Any(x => x.ReasonForNoApproval))
                {
                    return false;
                }

                return true;
            }
        }
        public bool IsApprovable
        {
            get
            {
                if (Remarks.Any(x => x.Severity == NEERemarkSeverity.High))
                {
                    return false;
                }

                return true;
            }
        }
        public bool AllDocumentsAccepted
        {
            get
            {
                if ((HasFEKDocument && IsFEKDocumentApproved == false) ||
                    (HasMaritalStatusDocument && IsMaritalStatusDocumentApproved == false) ||
                    (HasPensionAlbaniaDocument && IsPensionAlbaniaDocumentApproved == false) ||
                    (HasSpousePensionDocument && IsSpousePensionDocumentApproved == false))
                {
                    return false;
                }
                return true;
            }
        }
        public bool CanCancel() =>
            State == AppState.Draft ||
            State == AppState.Approved ||
            State == AppState.Submitted;

        #endregion



        #region applicant info
        public string IBAN { get; set; }
        public string Email { get; set; }
        [MaxLength(10)]
        public string HomePhone { get; set; }
        [MaxLength(10)]
        public string MobilePhone { get; set; }
        #endregion




        public Application()
        {
            Applicant = new Person();
            //Spouse = new Person();
            Members = new List<Person>();
            Remarks = new List<Remark>();
            Files = new List<AppFile>();
            ApplicationLog = new List<ApplicationLog>();

        }
        public static Application NewEmpty(INEEAppIdCreator appIdGenerator, int applicationIndex, DateTime? MinPeriodFrom)
        {
            DateTime dt = DateTime.Now;
            string id = appIdGenerator.CreateIdFromDateTime(dt);

            return new Application
            {
                Id = id,
                ApplicationIndex = applicationIndex,
                State = AppState.Draft,
                PeriodFrom = MinPeriodFrom
            };
        }
        public void PopulateCreatedByFlag(INEECurrentUserContext user)
        {
            this.IsCreatedByKK = user.IsNormalUser;
        }
        public void SetState(AppState state, INEECurrentUserContext currentUser = null, string changeReason = "")
        {
            if (!this.State.HasValue)
            {
                this.State = state;
            }
            else
            {
                if (state != this.State)
                {
                    var changeDate = DateTime.Now;

                    var sameTimeChangedAt = this.ApplicationStateChange
                        .Where(x => x.ChangedAt == changeDate)
                        .OrderBy(x => x.StateTo)
                        .FirstOrDefault();

                    if (sameTimeChangedAt != null)
                    {
                        changeDate.AddMilliseconds(100);
                    }

                    this.ApplicationStateChange.Add(new StateChange()
                    {
                        Id = this.Id,
                        ChangedAt = changeDate,
                        ChangedBy = (currentUser == null) ? null : currentUser.UserName,
                        ChangeId = Guid.NewGuid().ToString(),
                        StateFrom = this.State.Value,
                        StateTo = state,
                        ChangeReason = changeReason,
                        ReferenceDate = changeDate
                    });

                    this.State = state;
                }
            }
        }
        public void LoadFurtherChecks(INEECurrentUserContext user)
        {
            ValidateUserActions(user);
            this.IsAdminUser = user.IsNormalUser;
            this.IsAFMUser = user.IsAFMUser;
        }
        public void ValidateUserActions(INEECurrentUserContext user)
        {
            ValidateIsReadOnlyApplication(user);
            ValidateCanViewOnlyApplication(user);
            ValidateCanViewApplicationSubmittedDetails(user);
            ValidateIsEditableApplication(user);
        }
        public bool ValidateIsEditableApplication(INEECurrentUserContext user)
        {
            var info = user.IsAFMUser ? user.User.Identity.GetAfmUserInfo() : null;
            bool canEdit = (user.IsAFMUser && 
                            info.AMKA == Applicant.AMKA && 
                            info.AFM == Applicant.AFM && 
                            !IsApplicationCreatedByKK && 
                            !IsApplicationModifiedByKK) || 
                        (user.IsKKUser && 
                        !user.IsReadOnlyUser);
            this.IsEditableApplication = canEdit;
            return canEdit;
        }
        public void ValidateIsReadOnlyApplication(INEECurrentUserContext user)
        {
            this.IsReadOnlyApplication = (user.IsNormalUser && user.IsReadOnlyUser) || State != AppState.Draft || !this.IsEditableApplication;
        }
        public void ValidateCanViewOnlyApplication(INEECurrentUserContext user)
        {
            var info = user.IsAFMUser ? user.User.Identity.GetAfmUserInfo() : null;
            CanViewOnlyApplication = (user.IsNormalUser && user.IsReadOnlyUser) ||
                                          (((user.IsAFMUser && info.AMKA == Applicant.AMKA && info.AFM == Applicant.AFM) || (user.IsNormalUser)) &&
                                           (State == AppState.Approved || 
                                           State == AppState.Canceled || 
                                           State == AppState.Rejected || 
                                           State == AppState.Submitted || !this.IsEditableApplication));
            LockedByKK = user.IsAFMUser && 
                            (IsApplicationCreatedByKK || IsApplicationModifiedByKK) 
                            && State == AppState.Draft;

            IsInFinalStatus = State == AppState.Canceled ||
                              State == AppState.Rejected ||
                              State == AppState.Approved;
        }
        public void ValidateCanViewApplicationSubmittedDetails(INEECurrentUserContext user)
        {
            var info = user.IsAFMUser ? user.User.Identity.GetAfmUserInfo() : null;
            if (user.IsAFMUser)
            {
                this.CanViewApplicationSubmittedDetails = info.AMKA == Applicant.AMKA && info.AFM == Applicant.AFM && (State == AppState.Approved || State == AppState.Rejected || State == AppState.Recalled || State == AppState.Submitted || State == AppState.PendingDocumentsApproval || State == AppState.RejectedDocuments);
            }
            else
            {
                this.CanViewApplicationSubmittedDetails = (State == AppState.Approved || State == AppState.Rejected || State == AppState.Recalled || State == AppState.Submitted || State == AppState.PendingDocumentsApproval || State == AppState.RejectedDocuments);
            }
        }
        public void CreateRemarks(List<Remark> newRemarks)
        {
            if (this.State == AppState.Draft || State == AppState.RejectedDocuments || State == AppState.Submitted || State == AppState.PendingDocumentsApproval)
            {
                List<Remark> newlyDefinedFromProcess = this.Remarks.Where(x => !x.IsFromDB).ToList();
                List<Remark> alreadyExistingRemarks = this.Remarks.Where(x => x.IsFromDB).ToList();

                newRemarks.AddRange(newlyDefinedFromProcess);

                UpdateValidationRemarks(newRemarks, alreadyExistingRemarks);
            }
        }
        private void UpdateValidationRemarks(List<Remark> newRemarks, List<Remark> oldRemarks)
        {
            List<Remark> finalRemarks = new List<Remark>();

            foreach (Remark newRemark in newRemarks)
            {
                var oldRemark = oldRemarks.Where(x => x.RelatedAMKA == newRemark.RelatedAMKA && x.RemarkCode == newRemark.RemarkCode).FirstOrDefault();
                bool isSameDescription = true;
                bool isSameSeverity = true;

                if (oldRemark != null)
                {
                    isSameDescription = oldRemark.Description == newRemark.Description;
                    isSameSeverity = oldRemark.Severity == newRemark.Severity;
                    oldRemark.Message = newRemark.Message;
                }

                if (oldRemark == null || !isSameDescription || !isSameSeverity)
                {
                    if (!isSameSeverity)
                    {
                        newRemark.Released = oldRemark.Released;
                        newRemark.ReleasedBy = oldRemark.ReleasedBy;
                        newRemark.ReleasedAt = oldRemark.ReleasedAt;
                    }
                    finalRemarks.Add(newRemark);
                }
                else
                {
                    finalRemarks.Add(oldRemark);
                }
            }

            Remarks = finalRemarks;
        }
        public void CalculateBenefitAmount()
        {
            List<Person> concludedMembers = this.Members.Where(x => x.MemberState == MemberState.Normal).ToList();
            List<Person> currentMemberList = new List<Person>();
            currentMemberList = concludedMembers.ToList();
            decimal amount = 0;

            //παίρνω τον αιτούντα
            Person applicant = concludedMembers.Where(x => x.PersonId == 0).FirstOrDefault();
            if (applicant != null)
            {
                amount += NEEConstants.NEEAmountBasic;
                concludedMembers.Remove(applicant);
                currentMemberList = concludedMembers.ToList();
            }
            currentMemberList = concludedMembers.ToList();                       
            Amount = amount;            
        }

        public void RejectNonEligibleApplication(INEECurrentUserContext _userContext)
        {

            SetState(AppState.Rejected, _userContext);
            RejectedAt = DateTime.Now;
            RejectedBy = _userContext.UserName;
        }

        public void ApproveOrReject(INEECurrentUserContext _userContext)
        {
            //πολίτης, πρώτη υποβολή
            if (State == AppState.Submitted && !IsAdminUser)
            {
                //2. πολίτης, πρώτη υποβολή -> απόρριψη
                if (!IsApprovable)
                {
                    SetState(AppState.Rejected, _userContext);
                    RejectedAt = DateTime.Now;
                    RejectedBy = _userContext.UserName;
                }
                //3. πολίτης, πρώτη υποβολή -> pendingDocuments
                else
                {
                    SetState(AppState.PendingDocumentsApproval, _userContext);
                    SubmittedAt = DateTime.Now;
                    SubmittedBy = _userContext.UserName;
                }
            }
            //οπεκά, πρώτος έλεγχος εγγράφων
            //κκ, πρώτη υποβολή
            else if ((State == AppState.PendingDocumentsApproval && IsAdminUser) //IsOpekaUser 
                || (State == AppState.Submitted && IsAdminUser))
            {
                if (AllDocumentsAccepted)
                {
                    //4. σωστά έγγραφα και κανόνες έγκρισης εγγράφων -> status approved
                    if (IsApprovable) 
                    {
                        SetState(AppState.Approved, _userContext);
                        ApprovedAt = DateTime.Now;
                        ApprovedBy = _userContext.UserName;
                    }
                    //5. σωστά έγγραφα και όχι κανόνες έγκρισης εγγράφων -> status rejected
                    else
                    {
                        SetState(AppState.Rejected, _userContext);
                        RejectedAt = DateTime.Now;
                        RejectedBy = _userContext.UserName;
                    }
                }
                //6. λάθος έγγραφα -> status rejectedDocs
                else
                {
                    SetState(AppState.RejectedDocuments, _userContext);
                }
            }
            //κκ, δεύτερος έλεγχος εγγράφων
            else if (State == AppState.RejectedDocuments && IsAdminUser)
            {
                //7. σωστά έγγραφα και κανόνες έγκρισης εγγράφων -> status approved
                if (AllDocumentsAccepted && IsApprovable)
                {
                    SetState(AppState.Approved, _userContext);
                    ApprovedAt = DateTime.Now;
                    ApprovedBy = _userContext.UserName;
                }
                //8. σωστά έγγραφα και όχι κανόνες έγκρισης εγγράφων->status rejected
                //9. λάθος έγγραφα -> status rejected
                else
                {
                    SetState(AppState.Rejected, _userContext);
                    RejectedAt = DateTime.Now;
                    RejectedBy = _userContext.UserName;
                }
            }
        }

        public void Cancel(INEECurrentUserContext _currentUserContext)
        {
            if (!CanCancel())
                throw new InvalidOperationException($"Δεν υπάρχει δυνατότητα διαγραφής της αίτησης");

            SetState(AppState.Canceled, _currentUserContext);
            CanceledAt = DateTime.Now;
            CanceledBy = _currentUserContext.UserName;
        }
    }

    public class DateInterval
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Description { get; set; }
        public int Order { get; set; } = 0;
    }
}
