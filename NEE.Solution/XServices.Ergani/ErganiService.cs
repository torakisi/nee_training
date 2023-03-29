using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Database;
using System.ServiceModel.Channels;
using System.ServiceModel;
using XServices.Ergani.ErganiServiceReference;
using NEE.Core.Contracts.Enumerations;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using NEE.Core;
using NEE.Core.Validation;
using System.Linq;

namespace XServices.Ergani
{
    public class ErganiService : XServiceBase
    {
        public readonly string ServiceName = "ΚΕΔ Εργάνη";

        private WebServiceConnectionString _kedWsConStr;           // GSIS : "uid|pwd|url"

        private string _audit_transaction_Reason = "Επίδομα ΚΑΑΥ Αλβανίας";

        private readonly NEEDbContextFactory dbContextFactory;
        private readonly INEECurrentUserContext _currentUserContext;

        public ErganiService(NEEDbContextFactory dbContextFactory,
        INEECurrentUserContext currentUserContext,
        string kedWsConStr)
        {
            _kedWsConStr = new WebServiceConnectionString(kedWsConStr);
            this.dbContextFactory = dbContextFactory;
            _currentUserContext = currentUserContext;
        }


        public static ErganiService CreateDefault()
        {

            var ret = new ErganiService
            (
                null, null,
               //kedWsConStr: "T00010332T01189CYMHUDBR2PN48UDWUHTH|GdqCxxUtpl2tYS7nhNMU@|https://test.gsis.gr/esbpilot/erganiService"
               kedWsConStr: "L00010332L01189HMLE8NA7KC6D2NNZYBAP|Ergan#i281!|https://ked.gsis.gr/esb/erganiService"

            );
            return ret;
        }

        protected NEEDbContext CreateContext() =>
            dbContextFactory.Create();

        private bool CanLog
        {
            get
            {
                return (dbContextFactory != null && _currentUserContext != null);
            }
        }

        public erganiInterfaceClient CreateKedClient() => this.CreateKedClient(_kedWsConStr.Url, _kedWsConStr.Uid, _kedWsConStr.Pwd);
        private erganiInterfaceClient CreateKedClient(string url, string uid, string pwd)
        {

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Name = "getEmploymentDetailsInterfaceSoapBinding";

            binding.UseDefaultWebProxy = true;

            // create the client
            var client = new erganiInterfaceClient(binding, new EndpointAddress(url));

            client.ClientCredentials.UserName.UserName = uid;
            client.ClientCredentials.UserName.Password = pwd;

            var elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            client.Endpoint.Binding = new CustomBinding(elements);

            return client;
        }

        public async Task<GetEmploymentDetailsResponse> GetEmploymentDetailsAsync(GetEmploymentDetailsRequest req)
        {
            var res = new GetEmploymentDetailsResponse();
            var dbLog = CreateAadeLogEntry("Get employment info", req.Afm, req.Amka, req.ApplicationId);
            await AddKEDLog(dbLog, res, false);
            var sw = Stopwatch.StartNew();
            var client = CreateKedClient();

            Guid id = Guid.NewGuid();
            DateTime requestCallTimestamp = DateTime.Now;
            getEmploymentRelationshipRequest reqWS = null;

            try
            {
                //create request

                var audit = new auditRecord
                {
                    auditTransactionId = dbLog.AuditTransactionId,
                    auditTransactionDate = dbLog.AuditTransactionDate,
                    auditUnit = dbLog.AuditUnit,
                    auditProtocol = dbLog.AuditProtocol,
                    auditUserId = dbLog.AuditUserId,
                    auditUserIp = dbLog.AuditUserIp
                };
                var recordRequest = new getEmploymentRelationshipInput
                {
                    afm = req.Afm,
                    refdate = req.RefDate
                };

                reqWS = new getEmploymentRelationshipRequest
                {
                    auditRecord = audit,
                    getEmploymentRelationshipInputRecord = recordRequest
                };
                var jsonReqWS = JsonHelper.Serialize(reqWS, true);

                if (jsonReqWS.Length > NEEConstants.AADE_Log_ReqJson_Length)
                    jsonReqWS = JsonHelper.Serialize(reqWS, false);

                jsonReqWS = jsonReqWS.Truncate(NEEConstants.AADE_Log_ReqJson_Length);
                dbLog.ReqJson = jsonReqWS;

                //call the service

                var resWS = (await client.getEmploymentRelationshipAsync(reqWS)).getEmploymentRelationshipResponse;

                RaiseCallReturnedEvent(new XServiceCallReturnedEventArgs()
                {
                    Id = id.ToString(),
                    ResponseCallTimestamp = DateTime.Now,
                    ResponseJson = JsonHelper.Serialize(resWS, false),
                    RequestJson = JsonHelper.Serialize(reqWS, false),
                    RequestCallTimestamp = requestCallTimestamp,
                    MethodCall = nameof(client.getEmploymentRelationshipAsync),
                    ServiceName = nameof(ErganiService),
                });

                dbLog.ElapsedMS = (int)sw.ElapsedMilliseconds;
                dbLog.CallSeqId = (long)resWS.callSequenceId;

                var jsonResWS = JsonHelper.Serialize(resWS, true);                                                              // serialize indented
                if (jsonResWS.Length > NEEConstants.AADE_Log_ResJson_Length) jsonResWS = JsonHelper.Serialize(resWS, false);     // if too long, serialize non-indented    // register length in DB
                dbLog.ResLen = jsonResWS.Length;                                                                                // register length in DB
                jsonResWS = jsonResWS.Truncate(NEEConstants.AADE_Log_ResJson_Length);                                            // truncate as needed
                dbLog.ResJson = jsonResWS;                                                                                      // truncate as needed              // register json in DB
                var resRecord = resWS.getEmploymentRelationshipOutputRecord?.doc;
                if (resRecord != null)
                {
                    var isEmployed = resRecord.Any(r => r.kind != -1);
                    res.IsEmployed = isEmployed;
                }
            }
            catch (Exception ex)
            {
                client.Abort();
                res.AddError(ErrorCategory.Unhandled, null, ex);
                res.AddError(ErrorCategory.UIDisplayedServiceCallFailure, String.Format(ServiceErrorMessages.UnableToCommunicateWithService, ServiceName));
                dbLog.ErrorMessage = res._ErrorsFormatted;

                RaiseCallReturnedEvent(new XServiceCallReturnedEventArgs()
                {
                    Id = id.ToString(),
                    RequestJson = JsonHelper.Serialize(reqWS, false),
                    RequestCallTimestamp = requestCallTimestamp,
                    MethodCall = nameof(client.getEmploymentRelationshipAsync),
                    ServiceName = nameof(ErganiService),
                });
            }
            finally
            {
                client.Close();
            }

            await AddKEDLog(dbLog, res, true);

            return res;
        }

        private async Task AddKEDLog(KED_Log dbLog, XServiceResponseBase res, bool isUpdate = false)
        {
            if (CanLog)
            {
                try
                {
                    if (!isUpdate)
                    {
                        using (var db = CreateContext())
                        {
                            db.KED_Log.Add(dbLog);
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        using (var db = CreateContext())
                        {
                            db.KED_Log.Attach(dbLog);
                            db.Entry(dbLog).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                }
                catch (Exception ex)
                {
                    res.AddError(ErrorCategory.Unhandled, null, ex);
                }
            }
        }

        private KED_Log CreateAadeLogEntry(string reason, string afm, string amka, string applicationId)
        {
            var dtNow = DateTime.Now;
            var auditTransactionId = Guid.NewGuid().ToString();

            string endUserId = "System_Audit";
            if (_currentUserContext != null)
            {
                endUserId = string.IsNullOrEmpty(_currentUserContext.UserName)
                  ? afm
                  : _currentUserContext.UserName;
            }

            var dbLog = new KED_Log
            {
                Id = 1,
                AuditTransactionId = auditTransactionId,
                EntityCode = _kedWsConStr.Uid,
                AuditUnit = "ΟΠΕΚΑ",
                AuditTransactionDate = dtNow,
                TransactionReason = _audit_transaction_Reason,

                ServerHostName = (_currentUserContext != null) ? _currentUserContext.ServerName : "VIRTUAL_HOST",   //  eg: "mincome-dev.idika.gr"
                ServerHostIP = (_currentUserContext != null) ? _currentUserContext.ServerIP : "255.255.255.255",       //  eg: "192.168.17.81"
                AuditUserIp = (_currentUserContext != null) ? _currentUserContext.RemoteHostIP : "255.255.255.255",
                AuditUserId = endUserId,
                Reason = reason,
                AMKA = amka,
                AFM = afm,
                AuditProtocol = applicationId
            };
            return dbLog;
        }
    }
}
