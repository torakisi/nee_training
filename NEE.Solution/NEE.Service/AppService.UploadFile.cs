using NEE.Core.Contracts.Enumerations;
using NEE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEE.Service.Core;
using NEE.Database;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;

namespace NEE.Service
{
    public partial class AppService
    {

        public async Task<UploadFileResponse> UploadFileAsync(UploadFileRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.UploadFiles,
                ReferencedApplicationId = req.AppId,
                InitialRequest = null
            };

            SetServiceContext(context);

            UploadFileResponse resp = new UploadFileResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                if (req == null)
                    throw new ArgumentNullException(nameof(req));
                if (string.IsNullOrWhiteSpace(req.AppId))
                    throw new ArgumentNullException(nameof(req.AppId));

                if (req.Files != null && req.Files.Any())
                {                    
                    foreach (var reqFile in req.Files)
                    {                        
                        // post at IDIKA file service before submitting:
                        await UploadFile(reqFile);
                        bool fileExists = await repository.FileExists(reqFile);
                        if (fileExists)
                        {
                            resp._IsSuccessful = false;
                                                        
                            resp._Errors.Add(new Error { Category = ErrorCategory.UIDisplayed, Message = "Προσπαθείτε να επισυνάψετε το ίδιο αρχείο σε 2 διαφορετικά πεδία. \nΒεβαιωθείτε ότι καταχωρείτε το σωστό αρχείο στο κάθε πεδίο." });
                            return resp;
                        }

                        await repository.SaveFile(reqFile);
                        resp.Url = reqFile.Url;
                        await repository.UpdateCurrentFile(reqFile);
                    }
                }
            }
            catch (Exception ex)
            {
                resp.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return resp;
        }

        public async Task<SaveFileResponse> SaveFileAsync(SaveFileRequest req)
        {
            ServiceContext context = new ServiceContext()
            {
                ServiceAction = ServiceAction.UploadFiles,
                ReferencedApplicationId = req.AppId,
                InitialRequest = null
            };

            SetServiceContext(context);

            SaveFileResponse resp = new SaveFileResponse(_errorLogger, _currentUserContext.UserName);

            try
            {
                if (req == null)
                    throw new ArgumentNullException(nameof(req));
                if (string.IsNullOrWhiteSpace(req.AppId))
                    throw new ArgumentNullException(nameof(req.AppId));

                if (req.FileUrl != null)
                {
                    var reqFile = new AppFileSave() { ApplicationId = req.AppId, Url = req.FileUrl };
                    await repository.UpdateFile(reqFile, req.RejectionReason, req.IsApproved);
                }
            }
            catch (Exception ex)
            {
                resp.AddError(ErrorCategory.Unhandled, null, ex);
            }

            return resp;
        }        

        private async Task UploadFile(AppFileSave reqFile)
        {            
            using (var content = new System.Net.Http.MultipartFormDataContent())
            {
                var fileContent = new System.Net.Http.StreamContent(reqFile.ContentStream);
                fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(reqFile.ContentType);
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    Name = reqFile.FileName,
                    FileName = reqFile.FileName
                };
                content.Add(fileContent);
                // send using HttpClient:
                using (var client = new System.Net.Http.HttpClient())
                {
                    // Authorization Settings
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", 
                        ConfigurationManager.AppSettings["FilesService:Username"], 
                        ConfigurationManager.AppSettings["FilesService:Password"]));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var uri = string.Format("{0}{1}", ConfigurationManager.AppSettings["FilesService:BaseUrl"], "api/File/UploadFilesUpdateIdentical");
                    var clientResponse = await client.PostAsync(uri, content);
                    // if success, insert in database, else mark failure:
                    if (clientResponse.IsSuccessStatusCode)
                    {
                        string json = await clientResponse.Content.ReadAsStringAsync();
                        List<FileInformation> servicePostedFiles = JsonConvert.DeserializeObject<List<FileInformation>>(json);
                        FileInformation servicePostedFile = servicePostedFiles.FirstOrDefault();
                        reqFile.FileName = servicePostedFile.Filename;
                        reqFile.FileSize = servicePostedFile.Length.ToString();
                        reqFile.FileType = servicePostedFile.ContentType;
                        reqFile.Url = servicePostedFile.Id;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to communicate with IdikaFileService!");
                    }
                }
            }
        } 
    }

    public class UploadFileRequest : ApplicationIdentityRequest
    {
        public string AppId { get; set; }

        /// <summary>
        /// Sets any new files to insert or files to modify.
        /// </summary>
        public AppFileSave[] Files { get; set; }

        /// <summary>
        /// The Ids of files to delete.
        /// </summary>
        public string[] DeleteFileIds { get; set; }
    }

    public class SaveFileRequest : ApplicationIdentityRequest
    {
        public string AppId { get; set; }
        public string FileUrl { get; set; }
        public string RejectionReason { get; set; }
        public bool IsApproved { get; set; }
    }

    public class UploadFileResponse : NEEServiceResponseBase
    {
        public UploadFileResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }
        public string AppId { get; set; }
        public string Url { get; set; }

        public NEE_AppFile[] Files { get; set; }
    }

    public class SaveFileResponse : NEEServiceResponseBase
    {
        public SaveFileResponse(IErrorLogger errorLogger = null, string userName = null) : base(errorLogger, userName)
        {
        }
    }

    public partial class AppFileSave
    {
        public string Id { get; set; }
        public string ApplicationId { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string Url { get; set; }
        public string UploadedFromIP { get; set; }
        public string ContentType { get; set; }
        public Stream ContentStream { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DocumentCategory DocumentCategory { get; set; }
    }

    public class FileInformation
    {
        public string Id { get; set; }
        public string Filename { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public string Md5 { get; set; }
        public string Sha256 { get; set; }
    }
}
