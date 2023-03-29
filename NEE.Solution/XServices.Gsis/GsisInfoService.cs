using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Core.Validation;
using NEE.Core;
using System;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Threading.Tasks;
using NEE.Database;
using XServices.Gsis.PropertyValueInfoServiceReference;
using XServices.Gsis.SeniorHouseAssistInfoServiceReference;
using auditRecord = XServices.Gsis.SeniorHouseAssistInfoServiceReference.auditRecord;

namespace XServices.Gsis
{
    public class GsisInfoService : XServiceBase
    {
        public readonly string ServiceName = "ΚΕΔ ΑΑΔΕ";

        private WebServiceConnectionString _kedWsConStr;           // GSIS : "uid|pwd|url"

        private string _audit_transaction_Reason = "Επίδομα ΚΑΑΥ Αλβανίας";

        private readonly NEEDbContextFactory dbContextFactory;
        private readonly INEECurrentUserContext _currentUserContext;

        public GsisInfoService(NEEDbContextFactory dbContextFactory,
        INEECurrentUserContext currentUserContext,
        string kedWsConStr)
        {
            _kedWsConStr = new WebServiceConnectionString(kedWsConStr);
            this.dbContextFactory = dbContextFactory;
            _currentUserContext = currentUserContext;
        }


        public static GsisInfoService CreateDefault()
        {

            var ret = new GsisInfoService
            (
                null, null,                
               //kedWsConStr: "T00010332T01355N6ULL5V79FDYNR38TS9B|IncomeValue!9|https://test.gsis.gr/esbpilot/taxSeniorHouseAssistInfoService"
               kedWsConStr: "L00010332L01355CZV8PWHLRZ8BNMT5N72C|IncomeValue!8|https://ked.gsis.gr/esb/taxSeniorHouseAssistInfoService"

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

        public taxSeniorHouseAssistInfoInterfaceClient CreateKedClient() => this.CreateKedClient(_kedWsConStr.Url, _kedWsConStr.Uid, _kedWsConStr.Pwd);
        private taxSeniorHouseAssistInfoInterfaceClient CreateKedClient(string url, string uid, string pwd)
        {

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Name = "getTaxSeniorHouseAssistInfoInterfaceSoapBinding";

            binding.UseDefaultWebProxy = true;

            // create the client
            var client = new taxSeniorHouseAssistInfoInterfaceClient(binding, new EndpointAddress(url));

            client.ClientCredentials.UserName.UserName = uid;
            client.ClientCredentials.UserName.Password = pwd;

            var elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            client.Endpoint.Binding = new CustomBinding(elements);

            return client;
        }

        public async Task<GetIncomeMobileValueResponse> GetIncomeMobileValueAsync(GetIncomeMobileValueRequest req)
        {
            var res = new GetIncomeMobileValueResponse();
            var dbLog = CreateAadeLogEntry("Get income mobile value info", req.Afm, req.Amka, req.ApplicationId);
            await AddKEDLog(dbLog, res, false);
            var sw = Stopwatch.StartNew();
            var client = CreateKedClient();

            Guid id = Guid.NewGuid();
            DateTime requestCallTimestamp = DateTime.Now;
            getIncomeMobValueExpatShaRequest reqWS = null;

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
                var recordRequest = new getIncomeMobValueExpatShaInput
                {
                    afm = req.Afm,
                    forolEtos = req.ReferenceYear
                };

                reqWS = new getIncomeMobValueExpatShaRequest
                {
                    auditRecord = audit,
                    getIncomeMobValueExpatShaInputRecord = recordRequest
                };
                var jsonReqWS = JsonHelper.Serialize(reqWS, true);

                if (jsonReqWS.Length > NEEConstants.AADE_Log_ReqJson_Length)
                    jsonReqWS = JsonHelper.Serialize(reqWS, false);

                jsonReqWS = jsonReqWS.Truncate(NEEConstants.AADE_Log_ReqJson_Length);
                dbLog.ReqJson = jsonReqWS;

                //call the service

                var resWS = (await client.getIncomeMobValueExpatShaAsync(reqWS)).getIncomeMobValueExpatShaResponse;

                RaiseCallReturnedEvent(new XServiceCallReturnedEventArgs()
                {
                    Id = id.ToString(),
                    ResponseCallTimestamp = DateTime.Now,
                    ResponseJson = JsonHelper.Serialize(resWS, false),
                    RequestJson = JsonHelper.Serialize(reqWS, false),
                    RequestCallTimestamp = requestCallTimestamp,
                    MethodCall = nameof(client.getIncomeMobValueExpatShaAsync),
                    ServiceName = nameof(GsisInfoService),
                });

                dbLog.ElapsedMS = (int)sw.ElapsedMilliseconds;
                dbLog.CallSeqId = (long)resWS.callSequenceId;

                var jsonResWS = JsonHelper.Serialize(resWS, true);    
                if (jsonResWS.Length > NEEConstants.AADE_Log_ResJson_Length) jsonResWS = JsonHelper.Serialize(resWS, false);
                dbLog.ResLen = jsonResWS.Length;                                           
                jsonResWS = jsonResWS.Truncate(NEEConstants.AADE_Log_ResJson_Length);
                dbLog.ResJson = jsonResWS;
                
                var resRec = resWS.getIncomeMobValueExpatShaOutputRecord;
                if (resRec != null)
                {
                    res.SpouseAfm = resRec.coupleOtherAfm;
                    res.SpouseAmka = resRec.amkaSyz;
                    if (resRec.coupleOtherAfm != null && resRec.amkaSyz != null) 
                        res.FamilyIncome = resRec.eisodhmaSpecified ? (decimal?)resRec.eisodhma: null;
                    else
                        res.Income = resRec.eisodhmaSpecified ? (decimal?)resRec.eisodhma : null;
                    res.VehiclesValue = resRec.tekmhrioKinhtwnSpecified ? (decimal?)resRec.tekmhrioKinhtwn: null;
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
                    MethodCall = nameof(client.getIncomeMobValueExpatShaAsync),
                    ServiceName = nameof(GsisInfoService),
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
