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
using XServices.CitizenRegistry.CitizerRegistryServiceReference;
using System.Linq;
using System.Globalization;
using System.Text;

namespace XServices.CitizenRegistry
{
    public class CitizenRegistryService : XServiceBase
    {
        public readonly string ServiceName = "ΚΕΔ Μητρώο Πολιτών";

        private WebServiceConnectionString _kedWsConStr;           // GSIS : "uid|pwd|url"

        private string _audit_transaction_Reason = "Επίδομα ΚΑΑΥ Αλβανίας";

        private readonly NEEDbContextFactory dbContextFactory;
        private readonly INEECurrentUserContext _currentUserContext;

        public CitizenRegistryService(NEEDbContextFactory dbContextFactory,
        INEECurrentUserContext currentUserContext,
        string kedWsConStr)
        {
            _kedWsConStr = new WebServiceConnectionString(kedWsConStr);
            this.dbContextFactory = dbContextFactory;
            _currentUserContext = currentUserContext;
        }


        public static CitizenRegistryService CreateDefault()
        {

            var ret = new CitizenRegistryService
            (
                null, null,
               //kedWsConStr: "T00010332T01207THCD7NZDKWD42FC4E74S|Mcert1f#|https://test.gsis.gr/esbpilot/citizenRegistryService"
               kedWsConStr: "L00010332L01207TWKCZ86XHXWKLZFPBTHX|Mcert2f#Pol|https://www1.gsis.gr/esb/citizenRegistryService"

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

        public citizenRegistryClient CreateKedClient() => this.CreateKedClient(_kedWsConStr.Url, _kedWsConStr.Uid, _kedWsConStr.Pwd);
        private citizenRegistryClient CreateKedClient(string url, string uid, string pwd)
        {

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            binding.Name = "getMaritalStatusCertificateInterfaceSoapBinding";

            binding.UseDefaultWebProxy = true;

            // create the client
            var client = new citizenRegistryClient(binding, new EndpointAddress(url));

            client.ClientCredentials.UserName.UserName = uid;
            client.ClientCredentials.UserName.Password = pwd;

            var elements = client.Endpoint.Binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().EnableUnsecuredResponse = true;
            client.Endpoint.Binding = new CustomBinding(elements);

            return client;
        }

        public async Task<GetMaritalStatusResponse> GetMaritalStatusCertificateAsync(GetMaritalStatusRequest req)
        {
            var res = new GetMaritalStatusResponse();
            var dbLog = CreateAadeLogEntry("Get marital status info", req.Afm, req.Amka, req.ApplicationId);
            await AddKEDLog(dbLog, res, false);
            var sw = Stopwatch.StartNew();
            var client = CreateKedClient();

            Guid id = Guid.NewGuid();
            DateTime requestCallTimestamp = DateTime.Now;
            getMaritalStatusCertificateRequest reqWS = null;

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
                var recordRequest = new getMaritalStatusCertificateInput
                {
                    firstname = req.FirstName,
                    lastname = req.LastName,
                    birthdate = req.BirthDate,
                    username = (_currentUserContext != null) ? _currentUserContext.UserName : "VIRTUAL_USER"
                };

                reqWS = new getMaritalStatusCertificateRequest
                {
                    auditRecord = audit,
                    getMaritalStatusCertificateInputRecord = recordRequest
                };
                var jsonReqWS = JsonHelper.Serialize(reqWS, true);

                if (jsonReqWS.Length > NEEConstants.AADE_Log_ReqJson_Length)
                    jsonReqWS = JsonHelper.Serialize(reqWS, false);

                jsonReqWS = jsonReqWS.Truncate(NEEConstants.AADE_Log_ReqJson_Length);
                dbLog.ReqJson = jsonReqWS;

                //call the service

                var resWS = (await client.getMaritalStatusCertificateAsync(reqWS)).getMaritalStatusCertificateResponse;

                RaiseCallReturnedEvent(new XServiceCallReturnedEventArgs()
                {
                    Id = id.ToString(),
                    ResponseCallTimestamp = DateTime.Now,
                    ResponseJson = JsonHelper.Serialize(resWS, false),
                    RequestJson = JsonHelper.Serialize(reqWS, false),
                    RequestCallTimestamp = requestCallTimestamp,
                    MethodCall = nameof(client.getMaritalStatusCertificateAsync),
                    ServiceName = nameof(CitizenRegistryService),
                });

                dbLog.ElapsedMS = (int)sw.ElapsedMilliseconds;
                dbLog.CallSeqId = (long)resWS.callSequenceId;

                var jsonResWS = JsonHelper.Serialize(resWS, true);                                                              // serialize indented
                if (jsonResWS.Length > NEEConstants.AADE_Log_ResJson_Length) jsonResWS = JsonHelper.Serialize(resWS, false);     // if too long, serialize non-indented    // register length in DB
                dbLog.ResLen = jsonResWS.Length;                                                                                // register length in DB
                jsonResWS = jsonResWS.Truncate(NEEConstants.AADE_Log_ResJson_Length);                                            // truncate as needed
                dbLog.ResJson = jsonResWS;                                                                                      // truncate as needed              // register json in DB
                var resRecords = resWS.getMaritalStatusCertificateOutputRecord?.list;

                if (resRecords != null && resRecords?.Length > 0)
                {
                    foreach (var record in resRecords)
                    {
                        var birthdate = DateTime.ParseExact(record.birthdate, "dd/MM/yyyy-hh:mm:ss", CultureInfo.InvariantCulture);
                        if (RemoveDiacritics(record.surname.ToLowerInvariant()).Equals(RemoveDiacritics(req.LastName.ToLowerInvariant())) &&
                            RemoveDiacritics(record.firstname.ToLowerInvariant()).Equals(RemoveDiacritics(req.FirstName.ToLowerInvariant())) &&
                            birthdate.ToString("dd/MM/yyyy").Equals(req.BirthDate))
                        {
                            if (resRecords.Any(r => r.membertype.Contains("σύζυγος")) && !record.membertype.Contains("τέκνο")) //Married or Widowed
                            {
                                if (resRecords.Any(r => r.deletereasonPriv.Contains("Θάνατος")))
                                {
                                    res.MaritalStatus = MaritalStatus.Widoed;
                                }
                                else
                                {
                                    res.MaritalStatus = MaritalStatus.Married;
                                }
                            }
                            else if (resRecords.Any(s => s.membertype.Contains("Συμβιών")) && !record.membertype.Contains("τέκνο")) //civilUnion or widowed
                            {
                                if (resRecords.Any(r => r.deletereasonPriv.Contains("Θάνατος")))
                                {
                                    res.MaritalStatus = MaritalStatus.CivilUnionWidoed;
                                }
                                else
                                {
                                    res.MaritalStatus = MaritalStatus.CivilUnion;
                                }
                            }
                            else if (record.marriagerank.Contains("γάμος")) //there was a marriage, but no active husband/spouse
                            {
                                res.MaritalStatus = MaritalStatus.Divorsed;
                            }
                            else if (!record.agreementactno.Equals("___") && 
                                     !record.agreementactno.Equals(""))  //there was an agreement, but no active husband/spouse
                            {
                                res.MaritalStatus = MaritalStatus.CivilUnionBreak;
                            }
                            else if (record.marriagerank.Equals("___") && record.agreementrank.Equals("___"))
                            {
                                res.MaritalStatus = MaritalStatus.Single;
                            }
                            else if (record.membertype.Contains("τέκνο"))
                            {
                                res.MaritalStatus = MaritalStatus.Single;
                            }
                            else
                                res.MaritalStatus = MaritalStatus.Unknown;

                            return res;
                        }
                    }
                }
                res.MaritalStatus = MaritalStatus.Unknown;
                return res;
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
                    MethodCall = nameof(client.getMaritalStatusCertificateAsync),
                    ServiceName = nameof(CitizenRegistryService),
                });
            }
            finally
            {
                client.Close();
            }

            await AddKEDLog(dbLog, res, true);

            return res;
        }
        static string RemoveDiacritics(string text)
        {
            string formD = text.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
                if (ch == 'ς')
                    sb.Replace(ch, 'σ');
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
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