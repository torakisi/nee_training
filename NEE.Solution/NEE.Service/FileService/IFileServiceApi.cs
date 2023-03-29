
using Refit;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NEE.Service.FileService
{

    [Headers("User-Agent: IDIKA File Service Client Library (1.0)")]
    public interface IFileServiceApi
    {
        [Get("/api/File/Download/{id}")]
        Task<HttpResponseMessage> Download(string id);

        [Get("/api/File/Details/{id}")]
        Task<FileInformation> Details(string id);

        [Delete("/api/File/Remove/{id}")]
        Task Remove(string id);

        [Multipart]
        [Post("/api/File/UploadFilesAllowIdentical")]
        Task<List<FileInformation>> UploadFilesAllowIdenticalSP([AliasAs("fileInput")] StreamPart stream);

        [Multipart]
        [Post("/api/File/UploadFilesAllowIdentical")]
        Task<List<FileInformation>> UploadFilesAllowIdenticalBAP([AliasAs("fileInput")] ByteArrayPart bytes);

        [Multipart]
        [Post("/api/File/UploadFilesAllowIdentical")]
        Task<List<FileInformation>> UploadFilesAllowIdenticalFIP([AliasAs("fileInput")] FileInfoPart file);

        [Multipart]
        [Post("/api/File/UploadFilesUpdateIdentical")]
        Task<List<FileInformation>> UploadFilesUpdateIdenticalSP([AliasAs("fileInput")] StreamPart stream);

        [Multipart]
        [Post("/api/File/UploadFilesUpdateIdentical")]
        Task<List<FileInformation>> UploadFilesUpdateIdenticalBAP([AliasAs("fileInput")] ByteArrayPart bytes);

        [Multipart]
        [Post("/api/File/UploadFilesUpdateIdentical")]
        Task<List<FileInformation>> UploadFilesUpdateIdenticalFIP([AliasAs("fileInput")] FileInfoPart file);

        [Multipart]
        [Put("/api/File/UpdateFile/{id}")]
        Task<FileInformation> UpdateFileSP(string id, [AliasAs("fileInput")] StreamPart stream);

        [Multipart]
        [Put("/api/File/UpdateFile/{id}")]
        Task<FileInformation> UpdateFileBAP(string id, [AliasAs("fileInput")] ByteArrayPart bytes);

        [Multipart]
        [Put("/api/File/UpdateFile/{id}")]
        Task<FileInformation> UpdateFileFIP(string id, [AliasAs("fileInput")] FileInfoPart file);
    }
}
