using NEE.Core.Contracts.Enumerations;
using NEE.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Core.BO
{
    public class ApplicationOwner
    {
        private readonly List<ApplicationParticipation> participations;
        private readonly List<ApplicationParticipation> activeParticipations; 

        public string AFM { get; }
        public string AMKA { get; }
        public bool IsAFMUser { get; set; }
        public bool IsNormalUser { get; set; }
        public bool CanHandleRemarks { get; set; }
        public bool IsReadOnlyUser { get; set; }
        public bool IsKKUser { get; set; }

        public Person Owner { get; set; } = new Person();

        public ApplicationOwner(string afm, string amka, List<ApplicationParticipation> participations, INEECurrentUserContext user)
        {
            AFM = afm;
            AMKA = amka;

            this.participations = participations;
            this.activeParticipations = participations;
            this.IsAFMUser = user.IsAFMUser;
            this.IsNormalUser = user.IsNormalUser;
            this.CanHandleRemarks = user.IsKKUser && !user.IsReadOnlyUser;
            this.IsKKUser = user.IsKKUser;
            this.IsReadOnlyUser = user.IsReadOnlyUser;
        }

        public List<ApplicationParticipation> GetParticipations()
        {
            return participations;
        }

        public CanCreateNewResult CanCreateNew(DateTime today)
        {
            CanCreateNewResult res = new CanCreateNewResult();

            if (activeParticipations.Where(x => x.Relationship == MemberRelationship.Applicant)
                .Any(x => x.State == AppState.Draft
                || x.State == AppState.Submitted
                || x.State == AppState.Approved
                || x.State == AppState.PendingDocumentsApproval
                || x.State == AppState.RejectedDocuments
                || x.State == AppState.Suspended))
            {
                res.CanCreateNew = false;
                res.Reason = "Η επιλογή [Νέα Αίτηση] δεν είναι διαθέσιμη επειδή το πρόσωπο εμπλέκεται σε άλλη αίτηση. Ανατρέξτε στον παραπάνω πίνακα για λεπτομέρειες";
                return res;
            }

            if (Owner.IsDead)
            {
                res.CanCreateNew = false;
                res.Reason = "Η επιλογή [Νέα Αίτηση] δεν είναι διαθέσιμη επειδή υπάρχει ένδειξη θανάτου στο μητρώο του ΑΜΚΑ. Παρακαλούμε απευθυνθείτε σε ΚΕΠ για έλεγχο των στοιχείων.";
                return res;
            }

            if (IsNormalUser && IsReadOnlyUser)
            {
                res.CanCreateNew = false;
                res.Reason = "Η επιλογή [Νέα Αίτηση] δεν είναι διαθέσιμη επειδή ο χρήστης δεν έχει δικαίωμα δημιουργίας νέας αίτησης.";
                return res;
            }

            res.CanCreateNew = true;
            res.Reason = "";
            return res;
        }

        public bool CanViewApplication(ApplicationParticipation app)
        {
            if (app.ApplicantAFM == AFM && (app.State != AppState.Draft ||
                                            ((this.IsAFMUser && (app.IsApplicationCreatedByKK || app.IsApplicationModifiedByKK)) || (this.IsNormalUser && this.IsReadOnlyUser))))
            {
                return true;
            }

            return false;
        }

        public bool CanEditApplication(ApplicationParticipation app)
        {
            if (app.ApplicantAFM == AFM && app.State == AppState.Draft && (this.CanHandleRemarks || (this.IsAFMUser && !app.IsApplicationCreatedByKK && !app.IsApplicationModifiedByKK)))
            {
                return true;
            }

            return false;
        }

        public List<ApplicationParticipation> GetActiveParticipations()
        {
            return activeParticipations;
        }

        public Application NewApplication(Helpers.INEEAppIdCreator idCreator)
        {
            var index = participations.Count + 1;
            var dateFromMin = this.activeParticipations
                .Where(x => x.State == AppState.Approved)
                .Max(x => x.PeriodTo);
            return Application.NewEmpty(idCreator, index, dateFromMin);
        }
    }
}
