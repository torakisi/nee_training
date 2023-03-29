using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Core.Security;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using NEE.Database;
using NEE.Core.Exceptions;
using XServices.Efka.SalaryEmpInfoServiceReference;
using System.ServiceModel.Channels;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Validation;
using NEE.Core;
using System.Diagnostics;

namespace XServices.Efka
{
    public class EfkaService : XServiceBase
    {
        public readonly string ServiceName = "ΚΕΔ ΕΦΚΑ";

        private WebServiceConnectionString _kedWsConStr;           // GSIS : "uid|pwd|url"

        private string _audit_transaction_Reason = "Επίδομα ΚΑΑΥ Αλβανίας";

        private readonly NEEDbContextFactory dbContextFactory;
        private readonly INEECurrentUserContext _currentUserContext;

        public EfkaService(NEEDbContextFactory dbContextFactory,
        INEECurrentUserContext currentUserContext,
        string kedWsConStr)
        {
            _kedWsConStr = new WebServiceConnectionString(kedWsConStr);
            this.dbContextFactory = dbContextFactory;
            _currentUserContext = currentUserContext;
        }


        public static EfkaService CreateDefault()
        {

            var ret = new EfkaService
            (
                null, null,
               //kedWsConStr: "T00010332T01302VXULPN6NCHRPX5ULYULH|EfkaPEN9#|https://test.gsis.gr/esbpilot/salaryEmpInfoService"
               kedWsConStr: "L00010332L01302HZX2MAHH734ALLBKEK6P|EfkaPEN8#|https://ked.gsis.gr/esb/salaryEmpInfoService"

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
        
        public salaryEmpInfoInterfaceClient CreateKedClient() => this.CreateKedClient(_kedWsConStr.Url, _kedWsConStr.Uid, _kedWsConStr.Pwd);
        private salaryEmpInfoInterfaceClient CreateKedClient(string url, string uid, string pwd)
        {

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Name = "getSalaryEmpInfoInterfaceSoapBinding";

            binding.UseDefaultWebProxy = true;

            // create the client
            var client = new salaryEmpInfoInterfaceClient(binding, new EndpointAddress(url));

            client.ClientCredentials.UserName.UserName = uid;
            client.ClientCredentials.UserName.Password = pwd;

            var elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            client.Endpoint.Binding = new CustomBinding(elements);

            return client;
        }


        public async Task<PensionsOpekaResponse> GetPensionsOpekaInfoAsync(PensionsOpekaRequest req)
        {
            var res = new PensionsOpekaResponse();
            var dbLog = CreateAadeLogEntry("Get pensions opeka info", req.Afm, req.Amka, req.ApplicationId);
            await AddKEDLog(dbLog, res, false);
            var sw = Stopwatch.StartNew();
            var client = CreateKedClient();

            Guid id = Guid.NewGuid();
            DateTime requestCallTimestamp = DateTime.Now;
            requestPensionsOpekaRequest reqWS = null;

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
                var recordRequest = new requestPensionsOpekaInput
                {
                    amkaId = req.Amka,
                    dateFrom = req.DateFrom,
                    dateTo = req.DateTo
                };

                reqWS = new requestPensionsOpekaRequest
                {
                    auditRecord = audit,
                    requestPensionsOpekaInputRecord = recordRequest
                };
                var jsonReqWS = JsonHelper.Serialize(reqWS, true);

                if (jsonReqWS.Length > NEEConstants.AADE_Log_ReqJson_Length)
                    jsonReqWS = JsonHelper.Serialize(reqWS, false);

                jsonReqWS = jsonReqWS.Truncate(NEEConstants.AADE_Log_ReqJson_Length);
                dbLog.ReqJson = jsonReqWS;

                //call the service

                var resWS = (await client.requestPensionsOpekaAsync(reqWS)).requestPensionsOpekaResponse;

                RaiseCallReturnedEvent(new XServiceCallReturnedEventArgs()
                {
                    Id = id.ToString(),
                    ResponseCallTimestamp = DateTime.Now,
                    ResponseJson = JsonHelper.Serialize(resWS, false),
                    RequestJson = JsonHelper.Serialize(reqWS, false),
                    RequestCallTimestamp = requestCallTimestamp,
                    MethodCall = nameof(client.requestPensionsOpekaAsync),
                    ServiceName = nameof(EfkaService),
                });

                dbLog.ElapsedMS = (int)sw.ElapsedMilliseconds;
                dbLog.CallSeqId = (long)resWS.callSequenceId;

                var jsonResWS = JsonHelper.Serialize(resWS, true);                                                              // serialize indented
                if (jsonResWS.Length > NEEConstants.AADE_Log_ResJson_Length) jsonResWS = JsonHelper.Serialize(resWS, false);     // if too long, serialize non-indented    // register length in DB
                dbLog.ResLen = jsonResWS.Length;                                                                                // register length in DB
                jsonResWS = jsonResWS.Truncate(NEEConstants.AADE_Log_ResJson_Length);                                            // truncate as needed
                dbLog.ResJson = jsonResWS;                                                                                      // truncate as needed              // register json in DB
                var pensionRecords = resWS.requestPensionsOpekaOutputRecord?.pensions;
                res.Pensions = new System.Collections.Generic.List<Pension>();

                if (pensionRecords?.Length > 0)
                {
                    var currentPensions = pensionRecords.Where(p => p.year == DateTime.Now.Year && p.month == DateTime.Now.Month).ToList();

                    foreach (var pension in currentPensions)
                    {
                        res.Pensions.Add(new Pension
                        {
                            Amka = pension.amkaId,
                            GrossAmountBasic = pension.grossAmntBasicSpecified ? (decimal?)pension.grossAmntBasic : null,
                            GrossAmountAdditional = pension.grossAmntEpikSpecified ? (decimal?)pension.grossAmntEpik : null,
                            Year = pension.yearSpecified ? (decimal?)pension.year : null,
                            Month = pension.monthSpecified ? (decimal?)pension.month : null
                        });
                    }
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
                    MethodCall = nameof(client.requestPensionsOpekaAsync),
                    ServiceName = nameof(EfkaService),
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
