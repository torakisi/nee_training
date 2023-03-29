using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEE.Core.Contracts;
using NEE.Core.Security;
using NEE.Service.FileService;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace NEE.Service.Tests
{
    [TestClass]
    public class FileServiceTests
    {

        private static FileAccessService _client;

        protected INEECurrentUserContext _currentUserContext;
        protected IErrorLogger _errorLogger;

        [TestInitialize]
        public void Initialize()
        {
            _client = new FileAccessService(_currentUserContext, _errorLogger);
        }

        [TestMethod]
        public async Task GetFileById()
        {
            var req = new GetFileByIdRequest
            {
                UploadedFileId = "090dc440-3608-47f6-bf05-85ee07ee5921",

                ApplicationId = "F49BD044643A7C95E053280FA8C0D0D7"

            };
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            var res = await _client.GetFileByIdAsync(req);

            res._IsSuccessful.Should().BeTrue(res._ErrorsFormatted);

        }
    }
}
