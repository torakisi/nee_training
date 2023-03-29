using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEE.Core.Helpers;
using System;
using System.Threading.Tasks;
using XServices.Efka;

namespace XServices.Tests
{
    [TestClass]
    public class EfkaServiceTests
    {
        private EfkaService client;

        [TestInitialize]
        public void Initialize()
        {
            client = EfkaService.CreateDefault();
        }

        [DataTestMethod]
        //[DataRow("2301-3011-2886-7715", "105874230", "10085300936", "12/01/2005", "12/01/2023")]
        [DataRow("2301-3011-2886-7716", "105874231", "24075802371", "12/01/2005", "23/02/2023")]
        public async Task GetPensionsOpeka(string applicationId, string afm, string amka, string dateFrom, string dateTo)
        {
            var res = await client.GetPensionsOpekaInfoAsync(new PensionsOpekaRequest
            {
                ApplicationId= applicationId,
                Afm = afm,
                Amka = amka,
                DateFrom = dateFrom,
                DateTo = dateTo
            });

            res._IsSuccessful.Should().BeTrue(res._ErrorsFormatted);
        }
    }    
}