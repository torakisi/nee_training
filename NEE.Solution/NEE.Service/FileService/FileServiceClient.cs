using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NEE.Service.FileService
{
    public class FileServiceClient
    {


        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _password;
        private HttpClient httpClient;

        public FileServiceClient(string baseUrl, string username, string password)
        {
            _baseUrl = baseUrl;
            _username = username;
            _password = password;
        }

        public FileServiceClient(string username, string password)
    : this("http://applsrv10:7780/IdikaFileService/", username, password)
        { }


        public IFileServiceApi Api
        {
            get
            {
                if (httpClient == null)
                {
                    httpClient = new HttpClient(
                        new AuthenticatedHttpClientHandler(
                            GetBasicAuthenticationBase64Value()))
                    {
                        BaseAddress = new Uri(_baseUrl)
                    };
                }
                var refitSettings = new RefitSettings
                {
                    UrlParameterFormatter = new DefaultUrlParameterFormatter()
                };
                return RestService.For<IFileServiceApi>(httpClient, refitSettings);
            }
        }

        public async Task<FileInformation> HelperUploadAllowIdentical(StreamPart stream)
        {
            var response = await this.Api.UploadFilesAllowIdenticalSP(stream);

            return (response.Count == 1) ? response[0] : null;
        }

        public async Task<FileInformation> HelperUploadAllowIdentical(ByteArrayPart bytes)
        {
            var response = await this.Api.UploadFilesAllowIdenticalBAP(bytes);

            return (response.Count == 1) ? response[0] : null;
        }

        public async Task<FileInformation> HelperUploadAllowIdentical(FileInfoPart file)
        {
            var response = await this.Api.UploadFilesAllowIdenticalFIP(file);

            return (response.Count == 1) ? response[0] : null;
        }

        public async Task<FileInformation> HelperUploadUpdateIdentical(StreamPart stream)
        {
            var response = await this.Api.UploadFilesUpdateIdenticalSP(stream);

            return (response.Count == 1) ? response[0] : null;
        }

        public async Task<FileInformation> HelperUploadUpdateIdentical(ByteArrayPart bytes)
        {
            var response = await this.Api.UploadFilesUpdateIdenticalBAP(bytes);

            return (response.Count == 1) ? response[0] : null;
        }

        public async Task<FileInformation> HelperUploadUpdateIdentical(FileInfoPart file)
        {
            var response = await this.Api.UploadFilesUpdateIdenticalFIP(file);

            return (response.Count == 1) ? response[0] : null;
        }

        public async Task<FileInformation> HelperUpdate(string id, StreamPart stream)
        {
            return await this.Api.UpdateFileSP(id, stream);
        }

        public async Task<FileInformation> HelperUpdate(string id, ByteArrayPart bytes)
        {
            return await this.Api.UpdateFileBAP(id, bytes);
        }

        public async Task<FileInformation> HelperUpdate(string id, FileInfoPart file)
        {
            return await this.Api.UpdateFileFIP(id, file);
        }

        private string GetBasicAuthenticationBase64Value()
        {
            var byteArray = Encoding.ASCII.GetBytes($"{_username}:{_password}");
            return Convert.ToBase64String(byteArray);
        }
    }
}
