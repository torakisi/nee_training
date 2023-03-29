using NEE.Core;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Core.Validation;
using NEE.Database;
using System.Diagnostics;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using XServices.Edto.PolicePersonalDetailsEDTOService;
using System.Linq;
using System.Collections.Generic;

namespace XServices.Edto
{
    public class KEDService : XServiceBase
    {
        public readonly string ServiceName = "ΚΕΔ ΕΔΤΟ";

        private WebServiceConnectionString _kedWsConStr;           // GSIS : "uid|pwd|url"

        private string _audit_transaction_Reason = "Επίδομα ΚΑΑΥ Αλβανίας";

        private readonly NEEDbContextFactory dbContextFactory;
        private readonly INEECurrentUserContext _currentUserContext;

        public KEDService(NEEDbContextFactory dbContextFactory,
        INEECurrentUserContext currentUserContext,
        string kedWsConStr)
        {
            _kedWsConStr = new WebServiceConnectionString(kedWsConStr);
            this.dbContextFactory = dbContextFactory;
            _currentUserContext = currentUserContext;
        }


        public static KEDService CreateDefault()
        {

            var ret = new KEDService
            (
                null, null,
               //kedWsConStr: "T00010332T01383V2PFM6WNDCCKKULKRP2S|h2AWuS1r5zb4w6wueocC@|https://test.gsis.gr/esbpilot/policePersonalDetailsEDTOService"
               kedWsConStr: "L00010332L01383H3DM36BXBHM9BC8DXBBM|ProtecTci9#|https://ked.gsis.gr/esb/policePersonalDetailsEDTOService"

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
        public policePersonalDetailsEDTOInterfaceClient CreateKedClient() => this.CreateKedClient(_kedWsConStr.Url, _kedWsConStr.Uid, _kedWsConStr.Pwd);
        private policePersonalDetailsEDTOInterfaceClient CreateKedClient(string url, string uid, string pwd)
        {

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Name = "policePersonalDetailsEDTOInterfaceSoapBinding";

            binding.UseDefaultWebProxy = true;

            // create the client
            var client = new policePersonalDetailsEDTOInterfaceClient(binding, new EndpointAddress(url));

            client.ClientCredentials.UserName.UserName = uid;
            client.ClientCredentials.UserName.Password = pwd;

            var elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            client.Endpoint.Binding = new CustomBinding(elements);

            return client;
        }

        public async Task<GetPolicePersonDetailsResponse> GetPolicePersonDetailsAsync(GetPolicePersonDetailsRequest req)
        {
            var res = new GetPolicePersonDetailsResponse();
            var dbLog = CreateAadeLogEntry("Get Edto info", req.Afm, req.Amka, req.ApplicationId);
            await AddKEDLog(dbLog, res, false);
            var sw = Stopwatch.StartNew();
            var client = CreateKedClient();

            Guid id = Guid.NewGuid();
            DateTime requestCallTimestamp = DateTime.Now;
            getPersonDetailsEDTORequest reqWS = null;

            try
            {
                //create request

                var audit = new auditRecord
                {
                    auditUnit = dbLog.AuditUnit,
                    auditUserId = dbLog.AuditUserId,
                    auditUserIp = dbLog.AuditUserIp,
                    auditProtocol = dbLog.AuditProtocol,
                    auditTransactionId = dbLog.AuditTransactionId,
                    auditTransactionDate = dbLog.AuditTransactionDate
                };
                var recordRequest = new getPersonDetailsEDTOInput
                {
                    lastName = req.LastName,
                    firstName = req.FirstName,
                    fathersName = req.FathersName,
                    mothersName = req.MothersName,
                    birthDate = req.BirthDate
                };
                reqWS = new getPersonDetailsEDTORequest
                {
                    auditRecord = audit,
                    getPersonDetailsEDTOInputRecord = recordRequest
                };
                var jsonReqWS = JsonHelper.Serialize(reqWS, true);

                if (jsonReqWS.Length > NEEConstants.AADE_Log_ReqJson_Length)
                    jsonReqWS = JsonHelper.Serialize(reqWS, false);

                jsonReqWS = jsonReqWS.Truncate(NEEConstants.AADE_Log_ReqJson_Length);
                dbLog.ReqJson = jsonReqWS;

                
                //call the service

                var resWS = (await client.getPersonDetailsEDTOAsync(reqWS)).getPersonDetailsEDTOResponse;

                RaiseCallReturnedEvent(new XServiceCallReturnedEventArgs()
                {
                    Id = id.ToString(),
                    ResponseCallTimestamp = DateTime.Now,
                    ResponseJson = JsonHelper.Serialize(resWS, false),
                    RequestJson = JsonHelper.Serialize(reqWS, false),
                    RequestCallTimestamp = requestCallTimestamp,
                    MethodCall = nameof(client.getPersonDetailsEDTOAsync),
                    ServiceName = nameof(KEDService),
                });

                dbLog.ElapsedMS = (int)sw.ElapsedMilliseconds;
                dbLog.CallSeqId = (long)resWS.callSequenceId;

                var jsonResWS = JsonHelper.Serialize(resWS, true);                                                              // serialize indented
                if (jsonResWS.Length > NEEConstants.AADE_Log_ResJson_Length) jsonResWS = JsonHelper.Serialize(resWS, false);     // if too long, serialize non-indented    // register length in DB
                dbLog.ResLen = jsonResWS.Length;                                                                                // register length in DB
                jsonResWS = jsonResWS.Truncate(NEEConstants.AADE_Log_ResJson_Length);                                            // truncate as needed
                dbLog.ResJson = jsonResWS;                                                                                      // truncate as needed              // register json in DB


                if (resWS.getPersonDetailsEDTOOutputRecord != null)
                {
                    if (resWS.getPersonDetailsEDTOOutputRecord.returns.Length > 0)
                    {
                        var records = resWS.getPersonDetailsEDTOOutputRecord.returns;
                        if (records.Length > 0)
                        {
                            var items = new List<EdtoItem>();
                            foreach (var record in records)
                            {
                                items.Add(new EdtoItem() { 
                                    ProtocolDate = DateTime.ParseExact(record.efkaPermitCaseDTOS.FirstOrDefault().protocolDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                                    PermitNumber = record.efkaPermitCaseDTOS.FirstOrDefault().permitNumber
                                });
                            }
                            var recentCase = items.OrderByDescending(i => i.ProtocolDate).FirstOrDefault();
                            res.PermitNumber = recentCase.PermitNumber;
                            res.AdministrationDate = recentCase.ProtocolDate;
                        }
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
                    MethodCall = nameof(client.getPersonDetailsEDTOAsync),
                    ServiceName = nameof(KEDService),
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
                    //return res;
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
    public class EdtoItem
    {
        public DateTime? ProtocolDate { get; set; }
        public string PermitNumber { get; set; }
    }
}
