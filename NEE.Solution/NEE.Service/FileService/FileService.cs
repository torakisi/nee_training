using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Security;
using NEE.Service.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Service.FileService
{
    public class FileAccessService: NEEServiceBase
    {
        #region Security Related: OPS

        private static string OPS_API_USER = "ops.ws@kentrakoinotitas.gr";
        private static string OPS_API_PASS = "#2v55frQ6Lp$";    // this is the fallback password, you can use appSettings as in: <appSettings><add key= "ws-api-credentials:ops.ws@kentrakoinotitas.gr" value="#2v55frQ6Lp$"/>...


        private static WsApiCredentials OpsCredentials = new WsApiCredentials(OPS_API_USER, OPS_API_PASS);

        public FileAccessService(INEECurrentUserContext currentUserContext, IErrorLogger errorLogger) : base(currentUserContext, errorLogger)
        {
        }

        #endregion
        public async Task<GetFileByIdResponse> GetFileByIdAsync(GetFileByIdRequest req)
        {

            var res = new GetFileByIdResponse();
            var fileUsername = string.Empty;
            var filePassword = string.Empty;

            // If not logged in with ASP.NET Identity and not in the permitted Role, then use manual API Credentials...
            var userName = _currentUserContext?.UserName;   // let's start with this (we may change it bellow to api:user)
            var isStdUser = _currentUserContext?.IsInRole("DEV_Team") ?? false;
            if (!isStdUser)
            {
                if (!OpsCredentials.CheckApiCredentials(OpsCredentials.ApiUser, OpsCredentials.ApiPass))
                {
                    res.AddError(ErrorCategory.Error, "Invalid API Credentials, Access Denied!");
                    return res;
                }
                userName = "api:" + req.ApiUser;    // OK, change to: api:user now...                                                    
            }
            //Fake Login in order to have access to file
            fileUsername = ConfigurationManager.AppSettings["NEE:GetFile:FileServiceApiUser"];
            filePassword = ConfigurationManager.AppSettings["NEE:GetFile:FileServiceApiUserPass"];


            try
            {
                var idikaClient = new FileServiceClient(fileUsername, filePassword);
                var download = new HttpResponseMessage();
                download = await idikaClient.Api.Download(req.UploadedFileId);
                byte[] x = await download.Content.ReadAsByteArrayAsync();

                if (download.IsSuccessStatusCode == true)
                {
                    string filename = !string.IsNullOrWhiteSpace(download.Content.Headers.ContentDisposition.FileName) ? download.Content.Headers.ContentDisposition.FileName : string.Empty;
                    string contentType = !string.IsNullOrEmpty(download.Content.Headers.ContentType.ToString()) ? download.Content.Headers.ContentType.ToString() : string.Empty;
                    res.File = x;
                    res.FileName = filename;
                    res.FileContentType = contentType;
                }
                res._IsSuccessful = download.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                var log1 = new ErrorLog()
                {
                    CreatedAt = DateTime.Now,
                    ErrorLogSource = ErrorLogSource.Service,
                    Exception = ex.ToString(),
                    User = "TEST",
                    StackTrace = "log1",
                    InnerException = ex.InnerException?.ToString()
                };
                _errorLogger.LogErrorAsync(log1);
                res.AddError(ErrorCategory.Unhandled, ex.ToString());
            }

            return res;
        }

    }

    public class GetFileByIdRequest
    {
        // Credentials
        public string ApiUser { get; set; }
        public string ApiPass { get; set; }

        // Criteria
        public string ApplicationId { get; set; }
        public string UploadedFileId { get; set; }
    }

    public class GetFileByIdResponse : NEEServiceResponseBase
    {
        public class RequestCriteria
        {
            public string ApplicationId { get; set; }
        }

        public byte[] File { get; set; }

        public string FileName { get; set; }

        public string FileContentType { get; set; }
    }

    public class WsApiCredentials
    {

        public string ApiUser { get; private set; }
        public string ApiPass { get; private set; }

        public WsApiCredentials(string apiUser, string apiPass, bool tryLoadFromConfig = true)
        {
            this.ApiUser = apiUser;
            this.ApiPass = apiPass;

            if (tryLoadFromConfig)
            {
                var appKey = "ws-api-credentials:" + this.ApiUser;
                var value = ConfigurationManager.AppSettings[appKey];
                if (!string.IsNullOrWhiteSpace(value))
                    this.ApiPass = value;
            }
        }

        public bool CheckApiCredentials(string apiUser, string apiPass) => ((apiUser == this.ApiUser) || (apiPass == this.ApiPass));

    }
}
