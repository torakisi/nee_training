using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using XServices.NotificationCenter;

namespace XServices.Tests
{
    [TestClass]
    public class NotificationCenterServiceTests
    {
        private NotificationCenterService client;

        [TestInitialize]
        public void Initialize()
        {
            client = NotificationCenterService.CreateDefault();
        }

        [DataTestMethod]
        [DataRow("2301-3011-2886-7818", "102752860", "17046325145")]
        [DataRow("2302-3012-2887-7716", "052704062", "17046325145")]
        [DataRow("2303-3013-2888-7717", "076509078", "17046325145")]
        public async Task GetNncIdentityExt(string applicationId, string afm, string amka)
        {
            var res = await client.GetNncIdentityExtAsync(new GetNncIdentityExtRequest
            {
                ApplicationId = applicationId,
                Afm = afm,
                Amka = amka
            });

            res._IsSuccessful.Should().BeTrue(res._ErrorsFormatted);
        }
    }

    
}