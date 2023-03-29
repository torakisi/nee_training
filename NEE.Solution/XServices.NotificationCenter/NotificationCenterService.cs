using NEE.Core.Contracts;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using NEE.Core.Contracts.Enumerations;
using XServices.NotificationCenter.NotificationCenterElements;
using System.Diagnostics;
using NEE.Core;
using NEE.Core.Validation;

namespace XServices.NotificationCenter
{
    public class NotificationCenterService : XServiceBase
    {
        public readonly string ServiceName = "ΚΕΔ Κέντρο επικοινωνίας";

        private WebServiceConnectionString _kedWsConStr;           // GSIS : "uid|pwd|url"

        private string _audit_transaction_Reason = "Επίδομα ΚΑΑΥ Αλβανίας";

        private readonly NEEDbContextFactory dbContextFactory;
        private readonly INEECurrentUserContext _currentUserContext;

        public NotificationCenterService(NEEDbContextFactory dbContextFactory,
        INEECurrentUserContext currentUserContext,
        string kedWsConStr)
        {
            _kedWsConStr = new WebServiceConnectionString(kedWsConStr);
            this.dbContextFactory = dbContextFactory;
            _currentUserContext = currentUserContext;
        }

        public static NotificationCenterService CreateDefault()
        {

            var ret = new NotificationCenterService
            (
                null, null,
               //kedWsConStr: "T00010332T01220MZYB7PCPSFKC4C6TKUYB|GHzUSJFOMsJBSiK48BSM*|https://test.gsis.gr/esbpilot/notificationCenterElementsService"
               kedWsConStr: "L00010332L01220CCMHFWKMAXZFVLBYSLBK|MP17@epik!|https://ked.gsis.gr/esb/notificationCenterElementsService"

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

        public async Task<GetNncIdentityExtResponse> GetNncIdentityExtAsync(GetNncIdentityExtRequest req)
        {
            var res = new GetNncIdentityExtResponse();
            var dbLog = CreateAadeLogEntry("Get NCC Identity info", req.Afm, req.Amka, req.ApplicationId);
            await AddKEDLog(dbLog, res, false);
            var sw = Stopwatch.StartNew();
            var client = CreateKedClient();

            Guid id = Guid.NewGuid();
            DateTime requestCallTimestamp = DateTime.Now;
            getNncIdentityExtRequest reqWS = null;

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
                var recordRequest = new getNncIdentityExtInput
                {
                    afm = req.Afm                    
                };
                reqWS = new getNncIdentityExtRequest
                {
                    auditRecord = audit,
                    getNncIdentityExtInputRecord = recordRequest
                };
                var jsonReqWS = JsonHelper.Serialize(reqWS, true);

                if (jsonReqWS.Length > NEEConstants.AADE_Log_ReqJson_Length)
                    jsonReqWS = JsonHelper.Serialize(reqWS, false);

                jsonReqWS = jsonReqWS.Truncate(NEEConstants.AADE_Log_ReqJson_Length);
                dbLog.ReqJson = jsonReqWS;


                //call the service
                var resWS = (await client.getNncIdentityExtAsync(reqWS)).getNncIdentityExtResponse;

                RaiseCallReturnedEvent(new XServiceCallReturnedEventArgs()
                {
                    Id = id.ToString(),
                    ResponseCallTimestamp = DateTime.Now,
                    ResponseJson = JsonHelper.Serialize(resWS, false),
                    RequestJson = JsonHelper.Serialize(reqWS, false),
                    RequestCallTimestamp = requestCallTimestamp,
                    MethodCall = nameof(client.getNncIdentityExtAsync),
                    ServiceName = nameof(NotificationCenterService),
                });

                dbLog.ElapsedMS = (int)sw.ElapsedMilliseconds;
                dbLog.CallSeqId = (long)resWS.callSequenceId;

                var jsonResWS = JsonHelper.Serialize(resWS, true);                                                              // serialize indented
                if (jsonResWS.Length > NEEConstants.AADE_Log_ResJson_Length) jsonResWS = JsonHelper.Serialize(resWS, false);    // if too long, serialize non-indented    // register length in DB
                dbLog.ResLen = jsonResWS.Length;                                                                                // register length in DB
                jsonResWS = jsonResWS.Truncate(NEEConstants.AADE_Log_ResJson_Length);                                           // truncate as needed
                dbLog.ResJson = jsonResWS;                                                                                      // truncate as needed              // register json in DB
                var record = resWS.getNncIdentityExtOutputRecord;

                if (record != null)
                {
                    res.Email = record.email;
                    res.HomePhone = record.telephone;
                    res.MobilePhone = record.mobile;

                    res.AddressStreet = record.addressStreet;
                    res.AddressNumber = record.addressNumber;
                    res.AddressZip = record.addressZipCode;
                    res.AddressPostalNumber = record.addressNumber2;
                    res.Region = record.region;
                    res.RegionalUnit = record.regionalUnit;
                    res.Municipality= record.municipality;
                    res.MunicipalUnit= record.municipalUnit;
                    res.AddressCity = record.locality;
                    res.Commune = record.commune;
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
                    MethodCall = nameof(client.getNncIdentityExtAsync),
                    ServiceName = nameof(NotificationCenterService),
                });
            }
            finally
            {
                client.Close();
            }

            await AddKEDLog(dbLog, res, true);

            return res;
        }

        public notificationCenterElementsInterfaceClient CreateKedClient() => CreateKedClient(_kedWsConStr.Url, _kedWsConStr.Uid, _kedWsConStr.Pwd);
        private notificationCenterElementsInterfaceClient CreateKedClient(string url, string uid, string pwd)
        {

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Name = "notificationCenterElementsInterfaceSoapBinding";

            binding.UseDefaultWebProxy = true;

            // create the client
            var client = new notificationCenterElementsInterfaceClient(binding, new EndpointAddress(url));

            client.ClientCredentials.UserName.UserName = uid;
            client.ClientCredentials.UserName.Password = pwd;

            var elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            client.Endpoint.Binding = new CustomBinding(elements);

            return client;
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
}
