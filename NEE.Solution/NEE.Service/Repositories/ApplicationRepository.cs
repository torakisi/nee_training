using NEE.Core;
using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Exceptions;
using NEE.Core.Security;
using NEE.Database;
using NEE.Database.Entities;
using NEE.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Odbc;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using static System.Net.Mime.MediaTypeNames;
using Application = NEE.Core.BO.Application;

namespace NEE.Service
{
    public class ApplicationRepository
    {
        private readonly NEEDbContextFactory dbFactory;
        private readonly INEECurrentUserContext _currentUserContext;
        protected IErrorLogger _errorLogger;
        private readonly IClock clock;

        public ApplicationRepository(
            NEEDbContextFactory dbFactory,
            INEECurrentUserContext currentUserContext,
            IErrorLogger errorLogger,
            IClock clock)
        {
            if (currentUserContext == null)
                throw new ArgumentNullException(nameof(currentUserContext));
            if (dbFactory == null)
                throw new ArgumentNullException(nameof(dbFactory));

            this.dbFactory = dbFactory;
            _currentUserContext = currentUserContext;
            this.clock = clock ?? new SystemClock();
            _errorLogger = errorLogger;
        }

        private NEEDbContext CreateDb(string methodName) => dbFactory.Create(methodName, ApplicationConfigurationHelper.IsDbLoggingEnabled);
        public async Task<ApplicationOwner> LoadApplicationOwner(string afm, string amka)
        {
            using (var db = CreateDb("LoadApplicationOwner"))
            {
                var participationsQuery =
                    from app in db.NEE_App.AsNoTracking()
                    join person in db.NEE_AppPerson.AsNoTracking() on app.Id equals person.Id
                    join personApp in db.NEE_AppPerson.AsNoTracking() on app.Id equals personApp.Id
                    where (person.AMKA == amka && personApp.PersonId == 0)
                    select new ApplicationParticipation
                    {
                        ApplicantAFM = app.AFM,
                        ApplicantFirstName = personApp.FirstName,
                        ApplicantLastName = personApp.LastName,
                        ApplicantFirstNameEN = personApp.FirstNameEN,
                        ApplicantLastNameEN = personApp.LastNameEN,
                        ApplicantCitizenCountry = personApp.CitizenCountry,
                        Relationship = person.Relationship,
                        State = app.State,
                        ApprovedAt = app.ApprovedAt,
                        ApplicationId = app.Id,
                        CreatedAt = app.CreatedAt,
                        PersonId = person.PersonId,
                        PayFrom = app.PayFrom,
                        PayTo = app.PayTo,
                        PeriodFrom = app.PeriodFrom,
                        PeriodTo = app.PeriodTo,
                        RecalledAt = app.RecalledAt,
                        InitialPayTo = app.InitialPayTo,
                        InitialPeriodTo = app.InitialPeriodTo
                    };

                var participations = await participationsQuery.ToListAsync();

                return new ApplicationOwner(afm, amka, participations, _currentUserContext);
            }
        }
        public async Task<Application> Load(string id, int revision = 0)
        {
            using (var db = CreateDb("ApplicationRepository.Load"))
            {
                var dbApp = await LoadDbApp(db, id, revision);
                if (dbApp == null)
                    return null;
                return dbApp.Map();
            }
        }

        public async Task<int?> GetOpekaDistrict(string email)
        {
            using (var db = CreateDb("ApplicationRepository.GetOpekaDistrict"))
            {
                var dbDistrict = await db.GMI_UserDetails.SingleOrDefaultAsync(x => x.UserName == email);
                if (dbDistrict == null)
                    return null;
                return dbDistrict.Map();
            }
        }

        private async Task<NEE_App> LoadDbApp(NEEDbContext db, string id, int revision)
        {
            var dbApp = await db.NEE_App.SingleOrDefaultAsync(x =>
                    x.Id == id
                );

            if (dbApp == null)
                return null;

            await LoadAppDependingObjects(db, dbApp);

            return dbApp;
        }
        
        private async Task LoadAppDependingObjects(NEEDbContext db, NEE_App dbApp)
        {
            await db.Entry(dbApp).Collection(x => x.Members).LoadAsync();
            await db.Entry(dbApp).Collection(x => x.Remarks).LoadAsync();
            await db.Entry(dbApp).Collection(x => x.Files).LoadAsync();
        }

        public async Task<List<ZipCode>> LoadZipCodes()
        {
            using (var db = CreateDb("ApplicationRepository.LoadZipCodes"))
            {
                //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                var dbZipCodes = await db.DIC_ZipCodes.OrderBy(x => x.Code).ToListAsync();

                return dbZipCodes.MapList();
            }
        }
        public async Task Save(ChangeDistrictRequest req)
        {
            using (var db = CreateDb("ApplicationRepository.ChangeDistrict"))
            {
                var dbAppSetting = await db.NEE_AppSettings.SingleOrDefaultAsync();

                if (dbAppSetting.DisableDBUpdates)
                    throw new ApplicationMaintainanceModeException();
                var dbAppDistrict = await db.NEE_AppDistrict.SingleOrDefaultAsync(b => b.AppId == req.AppId);
                if (dbAppDistrict != null)
                {
                    dbAppDistrict.DistrictId = req.DistrictId.ToString();
                    db.Entry(dbAppDistrict).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task Save(Application application, bool isModelStateValid)
        {
            using (var db = CreateDb("ApplicationRepository.Save"))
            {
                var dbAppSetting = await db.NEE_AppSettings.SingleOrDefaultAsync();

                if (dbAppSetting.DisableDBUpdates)
                    throw new ApplicationMaintainanceModeException();
                var dbApp = await LoadDbApp(db, application.Id, application.Revision);

                if (dbApp == null)
                    await SaveNewApplication(db, application);
                else
                {
                    await SaveExistingApplication(db, dbApp, application, isModelStateValid);

                    application.Revision = dbApp.Revision;
                }
            }
        }

        public async Task UpdateFile(AppFileSave file, string rejectionReason, bool? isApproved)
        {
            using (var db = CreateDb("ApplicationRepository.UpdateFile"))
            {
                var dbAppSetting = await db.NEE_AppSettings.SingleOrDefaultAsync();

                if (dbAppSetting.DisableDBUpdates)
                    throw new ApplicationMaintainanceModeException();

                NEE_AppFile[] dbFiles = await db.NEE_AppFile.Where(f => f.ApplicationId == file.ApplicationId).ToArrayAsync();

                var dbFile = dbFiles.FirstOrDefault(x => x.Url == file.Url);
                if (dbFile != null)
                {
                    dbFile.ModifiedAt = DateTime.Now;
                    dbFile.ModifiedBy = _currentUserContext.UserName;
                    dbFile.RejectionReason = rejectionReason;
                    dbFile.IsApproved = isApproved;
                    dbFile.ApprovedAt = isApproved == true ? (DateTime?)DateTime.Now : null;
                    dbFile.ApprovedBy = isApproved == true ? _currentUserContext.UserName : null;
                    dbFile.RejectedAt = isApproved == false ? (DateTime?)DateTime.Now : null;
                    dbFile.RejectedBy = isApproved == false ? _currentUserContext.UserName : null;
                }

                //db.UpdateCreatedAndModifiedState(_currentUserContext, clock.Now);
                await db.SaveChangesAsync();

            }
        }

        public async Task<bool> FileExists(AppFileSave file)
        {
            using (var db = CreateDb("ApplicationRepository.FileExists"))
            {
                var dbAppSetting = await db.NEE_AppSettings.SingleOrDefaultAsync();

                if (dbAppSetting.DisableDBUpdates)
                    throw new ApplicationMaintainanceModeException();

                NEE_AppFile[] dbFiles = await db.NEE_AppFile.Where(f => f.ApplicationId == file.ApplicationId).ToArrayAsync();

                var dbFile = dbFiles.FirstOrDefault(x => x.Url == file.Url);
                return dbFile != null;
            }
        }      
        
        public async Task UpdateCurrentFile(AppFileSave file)
        {
            using (var db = CreateDb("ApplicationRepository.UpdateCurrentFile"))
            {
                var dbAppSetting = await db.NEE_AppSettings.SingleOrDefaultAsync();

                if (dbAppSetting.DisableDBUpdates)
                    throw new ApplicationMaintainanceModeException();

                var dbApp = await LoadDbApp(db, file.ApplicationId, 0);
                await LoadAppDependingObjects(db, dbApp);

                switch (file.DocumentCategory)
                {
                    case NEE.Core.Contracts.Enumerations.DocumentCategory.PensionAlbania:
                        if (dbApp.State == NEE.Core.Contracts.Enumerations.AppState.RejectedDocuments)
                            dbApp.PensionDocumentAlbaniaId2 = file.Url;
                        else
                            dbApp.PensionDocumentAlbaniaId = file.Url;
                        break;
                    case NEE.Core.Contracts.Enumerations.DocumentCategory.SpousePensionAlbania:
                        if (dbApp.State == NEE.Core.Contracts.Enumerations.AppState.RejectedDocuments)
                            dbApp.SpousePensDocumentAlbaniaId2 = file.Url;
                        else
                            dbApp.SpousePensionDocumentAlbaniaId = file.Url;
                        break;
                    case NEE.Core.Contracts.Enumerations.DocumentCategory.MaritalStatus:
                        if (dbApp.State == NEE.Core.Contracts.Enumerations.AppState.RejectedDocuments)
                            dbApp.MaritalStatusId2 = file.Url;
                        else
                            dbApp.MaritalStatusId = file.Url;
                        break;
                    case NEE.Core.Contracts.Enumerations.DocumentCategory.FEK:
                        if (dbApp.State == NEE.Core.Contracts.Enumerations.AppState.RejectedDocuments)
                            dbApp.FEKDocumentId2 = file.Url;
                        else
                            dbApp.FEKDocumentId = file.Url;
                        break;
                }

                //db.UpdateCreatedAndModifiedState(_currentUserContext, clock.Now);
                await db.SaveChangesAsync();
            }
        }

        public async Task SaveFile(AppFileSave file)
        {
            using (var db = CreateDb("ApplicationRepository.SaveFile"))
            {
                var dbAppSetting = await db.NEE_AppSettings.SingleOrDefaultAsync();

                if (dbAppSetting.DisableDBUpdates)
                    throw new ApplicationMaintainanceModeException();

                var dbFile = file.Map();

                db.NEE_AppFile.Add(dbFile);

                //db.UpdateCreatedAndModifiedState(_currentUserContext, clock.Now);
                await db.SaveChangesAsync();

            }
        }

        private Task SaveNewApplication(NEEDbContext db, Application application)
        {
            var dbApp = application.Map();

            db.NEE_App.Add(dbApp);

            db.NEE_AppDistrict.Add(new NEE_AppDistrict { AppId= application.Id, DistrictId = "0" });

            db.UpdateCreatedAndModifiedState(_currentUserContext, clock.Now);
            return db.SaveChangesAsync();
        }
        private Task SaveExistingApplication(NEEDbContext db, NEE_App dbApp, Application application, bool isModelStateValid)
        {
            application.Map(dbApp);
            if (isModelStateValid)
            {
                db.UpdateCreatedAndModifiedState(_currentUserContext, clock.Now);
            }

            return db.SaveChangesAsync();
        }

        public async Task<List<PaymentTransactions>> GetPaymentTransactions(string afm = null, string id = null)
        {
            using (var db = CreateDb("ApplicationRepository.GetPaymentTransactions"))
            {
                var dbPaymentTransactionsQuery = db.NEE_PaymentTransactions
                    .Where(x => true);

                if (!string.IsNullOrEmpty(afm))
                {
                    dbPaymentTransactionsQuery = dbPaymentTransactionsQuery
                        .Where(x => x.AFM == afm);
                }

                if (!string.IsNullOrEmpty(id))
                {
                    dbPaymentTransactionsQuery = dbPaymentTransactionsQuery
                        .Where(x => x.Id == id);
                }

                var dbPaymentTransactions = await dbPaymentTransactionsQuery.ToListAsync();

                List<PaymentTransactions> paymentTransactions = new List<PaymentTransactions>();

                foreach (NEE_PaymentTransactions dbPaymentTransaction in dbPaymentTransactions)
                {
                    paymentTransactions.Add(dbPaymentTransaction.Map());
                }

                return paymentTransactions;
            }

        }

        public async Task<List<PaymentsWebView>> GetPaymentTransactionsMasterView(string afm = null, string id = null)
        {
            using (var db = CreateDb("ApplicationRepository.GetPaymentTransactionsMasterView"))
            {
                var dbPaymentTransactionsQuery = db.NEE_PaymentsWebView
                    //temp fix
                    .Where(a => a.AFM != null);
                    //.Include(x => x.PaymentTransactions);

                if (!string.IsNullOrEmpty(afm))
                {
                    dbPaymentTransactionsQuery = dbPaymentTransactionsQuery
                        .Where(x => x.AFM == afm);
                }

                if (!string.IsNullOrEmpty(id))
                {
                    dbPaymentTransactionsQuery = dbPaymentTransactionsQuery
                        .Where(x => x.Id == id);
                }

                var dbPaymentTransactionsMasterView = await dbPaymentTransactionsQuery.ToListAsync();

                List<PaymentsWebView> paymentTransactionsMaster = new List<PaymentsWebView>();

                foreach (NEE_PaymentsWebView dbPaymentTransactionasterView in dbPaymentTransactionsMasterView)
                {
                    paymentTransactionsMaster.Add(dbPaymentTransactionasterView.Map());
                }

                return paymentTransactionsMaster;
            }
        }

        #region Admin Search
        public async Task<SearchApplicationsResponse> Search(SearchApplicationsRequest req)
        {
            SearchApplicationsResponse res = new SearchApplicationsResponse();

            var decisiveCrtiteria
                = (!string.IsNullOrWhiteSpace(req.Id))
                | (!string.IsNullOrWhiteSpace(req.AMKA))
                | (!string.IsNullOrWhiteSpace(req.AFM))
                ;

            try
            {
                using (var db = CreateDb("ApplicationRepository.Search"))
                {
                    var crit = new List<string>();
                    object[] parameters = new object[] { };


                    if (!string.IsNullOrWhiteSpace(req.Id))
                    {
                        if (req.SearchInAppPerson)
                            crit.Add(@"(p.""Id""   = :p_Id)");
                        else
                            crit.Add(@"(t.""Id""   = :p_Id)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.Id;

                    }

                    if (!string.IsNullOrWhiteSpace(req.AMKA))
                    {
                        if (req.SearchInAppPerson)
                            crit.Add(@"(p.""AMKA"" = :p_AMKA)");
                        else
                            crit.Add(@"(t.""AMKA"" = :p_AMKA)");

                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.AMKA;

                    }

                    if (!string.IsNullOrWhiteSpace(req.AFM))
                    {
                        if (req.SearchInAppPerson)
                            crit.Add(@"(p.""AFM""  = :p_AFM)");
                        else
                            crit.Add(@"(t.""AFM""  = :p_AFM)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.AFM;

                    }

                    if (!string.IsNullOrWhiteSpace(req.LastName))
                    {
                        if (req.SearchInAppPerson)
                            crit.Add(@"(Upper(p.""LastName"") like Upper(:p_LastName||'%'))");
                        else
                            crit.Add(@"(Upper(t.""LastName"") like Upper(:p_LastName||'%'))");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.LastName;

                    }

                    if (!string.IsNullOrWhiteSpace(req.FirstName))
                    {
                        if (req.SearchInAppPerson)
                            crit.Add(@"(Upper(p.""FirstName"") like Upper(:p_FirstName||'%'))");
                        else
                            crit.Add(@"(Upper(t.""FirstName"") like Upper(:p_FirstName||'%'))");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.FirstName;

                    }

                    if (!string.IsNullOrWhiteSpace(req.City))
                    {
                        crit.Add(@"(Upper(a.""City"") like Upper('%'||:p_City||'%'))");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.City;

                    }

                    if (!string.IsNullOrWhiteSpace(req.Zip))
                    {
                        crit.Add(@"(p.""Zip"" = :p_Zip)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.Zip;

                    }

                    if (!string.IsNullOrWhiteSpace(req.Email))
                    {
                        crit.Add(@"(a.""Email"" = :p_Email)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.Email;

                    }

                    if (!string.IsNullOrWhiteSpace(req.IBAN))
                    {
                        crit.Add(@"(a.""IBAN"" = :p_IBAN)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.IBAN;
                    }

                    if (!string.IsNullOrWhiteSpace(req.MobilePhone))
                    {
                        crit.Add(@"(a.""MobilePhone"" = :p_MobilePhone)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.MobilePhone;
                    }

                    if (!string.IsNullOrWhiteSpace(req.HomePhone))
                    {
                        crit.Add(@"(a.""HomePhone"" = :p_HomePhone)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.HomePhone;
                    }

                    if (!req.SubmittedByKK.Equals(null))
                    {
                        crit.Add(@"(a.""IsModifiedByKK"" = :p_SubmittedByKK)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.SubmittedByKK.Equals(true) ? '1' : '0';

                        crit.Add(@"(d.""DistrictId"" = :p_DistrictId)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.DistrictId;                        
                    }

                    if (req.StateId != -2)
                    {
                        crit.Add(@"(a.""State"" = :p_SelState)");
                        Array.Resize(ref parameters, parameters.Length + 1);
                        parameters[parameters.Length - 1] = req.StateId;
                    }
                    var where_clause = $@"Where a.""State"" <> -1";
                    var criteria = (crit.Count == 0) ? "" : " And " + string.Join("\r\n  And  ", crit);
                    where_clause = where_clause + criteria;
                    //var ora_parameters = para.ToArray();

                    var order_clause = "";
                    if (req.SubmittedByKK.Equals(null))
                        order_clause = $@"Order By a.""Id"" Desc, t.""PersonId"" DESC";
                    else
                        order_clause = $@"Order By a.""ModifiedAt""";


                    var from_clause = "";
                    from_clause =
                        $@"From    ""NEE_App"" a
                           Inner Join  ""NEE_AppPerson""     t On (t.""Id""=a.""Id"") And (t.""PersonId"" = 0)
                           Inner Join  ""NEE_AppDistrict""   d On (a.""Id"" = d.""AppId"")";
                    if (req.SearchInAppPerson)
                    {
                        from_clause =
                        $@"From    ""NEE_App"" a
                           Inner Join  ""NEE_AppPerson""     p On (p.""Id""=a.""Id"") AND (p.""MemberState"" <> 1)
                           Inner Join  ""NEE_AppPerson""     t On (t.""Id""=a.""Id"") And (t.""PersonId"" = 0)";
                    }


                    var sqlTotal =
                        $@"Select  Count(*) ""Total""
                        {from_clause}
                        {where_clause}";

                    var sqlSelect = "";

                    sqlSelect =
                    $@"Select  a.""Id"",
                                   a.""State"",
                                   t.""LastName""    ""Applicant_LastName"",
                                   t.""FirstName""   ""Applicant_FirstName"",
                                   t.""LastNameEN""    ""Applicant_LastNameEN"",
                                   t.""FirstNameEN""   ""Applicant_FirstNameEN"",
								   t.""CitizenCountry""   ""Applicant_CitizenCountry"",
                                   t.""Zip"",
                                   t.""Municipality"",
                                   t.""PersonId"",
                                   t.""AMKA"",
                                   t.""AFM"",
                                   t.""LastName"",
                                   t.""FirstName"",
                                   t.""LastNameEN"",
                                   t.""FirstNameEN"",
								   t.""CitizenCountry"",
                                   a.""IBAN"",
                                   a.""Email"",
                                   a.""MobilePhone"",
                                   a.""HomePhone"",
                                   d.""DistrictId""
                        {from_clause}
                        {where_clause}
                        {order_clause}";

                    if (req.SearchInAppPerson)
                    {
                        sqlSelect =
                        $@"Select  a.""Id"",
                                   a.""State"",
                                   t.""LastName""    ""Applicant_LastName"",
                                   t.""FirstName""   ""Applicant_FirstName"",
                                   t.""LastNameEN""    ""Applicant_LastNameEN"",
                                   t.""FirstNameEN""   ""Applicant_FirstNameEN"",
								   t.""CitizenCountry""   ""Applicant_CitizenCountry"",
                                   t.""Zip"",
                                   t.""Municipality"",
                                   p.""PersonId"",
                                   p.""AMKA"",
                                   p.""AFM"",
                                   p.""LastName"",
                                   p.""FirstName"",
                                   p.""LastNameEN"",
                                   p.""FirstNameEN"",
								   p.""CitizenCountry"",
                                   a.""IBAN"",
                                   a.""Email"",
                                   a.""MobilePhone"",
                                   a.""HomePhone""
                        {from_clause}
                        {where_clause}
                        {order_clause}";
                    }


                    var qryTotal = db.Database.SqlQuery<int>(sqlTotal, parameters);
                    var qrySelect = db.Database.SqlQuery<SearchApplication>(sqlSelect, parameters);

                    var total = await qryTotal.FirstAsync();

                    var skip = Math.Max(0, req.Skip);

                    int take;
                    if (!decisiveCrtiteria)
                    {
                        take = Math.Max(0, Math.Min(SearchApplicationsRequest.MAX_Take, req.Take));
                        if (take <= 0) take = SearchApplicationsRequest.DEF_Take;
                    }
                    else
                    {
                        take = Math.Max(0, Math.Min(SearchApplicationsRequest.MAX_Take_IfDecisiveCrtiteria, req.Take_IfDecisiveCrtiteria));
                        if (take <= 0) take = SearchApplicationsRequest.DEF_Take_IfDecisiveCrtiteria;
                    }

                    var lst = qrySelect.Skip(skip).Take(take).ToList();

                    res.ApplicationSearchResults = lst;

                    res.Total = total;
                    res.Skip = skip;
                    res.Take = take;

                    res._IsSuccessful = true;
                }
            }
            catch (Exception)
            {
                res = null;
            }

            return res;
        }
        #endregion
    }
}
