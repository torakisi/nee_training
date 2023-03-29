using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEE.Core.Helpers;
using System;
using System.Threading.Tasks;
using XServices.Edto;
using XServices.Ergani;

namespace XServices.Tests
{
    [TestClass]
    public class ErganiServiceTests
    {
        private ErganiService client;

        [TestInitialize]
        public void Initialize()
        {
            client = ErganiService.CreateDefault();
        }

        [DataTestMethod]
        [DataRow("2301-3011-2886-7715", "161182011", "17046325145", "12/12/2022")]
        [DataRow("2301-3011-2886-7715", "059577620", "28086602092", "12/12/2022")]
        [DataRow("2301-3011-2886-7715", "163121688", "08129501873", "12/12/2022")]
        public async Task GetEmploymentDetails(string applicationId, string afm, string amka, string refDate)
        {
            var res = await client.GetEmploymentDetailsAsync(new GetEmploymentDetailsRequest
            {
                ApplicationId = applicationId,
                Afm = afm,
                Amka = amka,
                RefDate = refDate
            });

            res._IsSuccessful.Should().BeTrue(res._ErrorsFormatted);
        }
    }    
}