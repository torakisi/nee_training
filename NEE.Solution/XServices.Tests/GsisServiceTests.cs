using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEE.Core.Helpers;
using System;
using System.Threading.Tasks;
using XServices.Edto;
using XServices.Ergani;
using XServices.Gsis;

namespace XServices.Tests
{
    [TestClass]
    public class GsisServiceTests
    {
        private GsisInfoService infoClient;
        private GsisPropertyService propertyClient;

        [TestInitialize]
        public void Initialize()
        {
            infoClient = GsisInfoService.CreateDefault();
            propertyClient = GsisPropertyService.CreateDefault();
        }

        [DataTestMethod]
        [DataRow("2301-0000-1115-7735", "146144237", "17046325145", 2022)]
        [DataRow("2309-0000-1116-7736", "036897617", "17046325145", 2022)]
        [DataRow("2300-0000-1117-7737", "019536955", "17046325145", 2022)]
        public async Task GetPropertyValueInfo(string applicationId, string afm, string amka, int refYear)
        {
            var res = await propertyClient.GetPropertyValueInfoAsync(new GetPropertyValueE9Request
            {
                ApplicationId = applicationId,
                Afm = afm,
                Amka = amka,
                ReferenceYear = refYear
            });

            res._IsSuccessful.Should().BeTrue(res._ErrorsFormatted);
        }

        [DataTestMethod]
        [DataRow("2301-3011-1111-7144", "032607885", "24075802371", 2021)]
        [DataRow("2301-3011-2222-7144", "027974826", "11015702183", 2021)]
        [DataRow("2301-3011-2886-7716", "000053299", "17046325145", 2021)]
        [DataRow("2302-3012-2886-7717", "000158141", "17046325145", 2021)]
        [DataRow("2303-3013-2886-7718", "000479563", "17046325145", 2021)]
        [DataRow("2304-3014-2886-7719", "000552330", "17046325145", 2021)]
        [DataRow("2305-3015-2886-7720", "018525525", "17046325145", 2021)]
        [DataRow("2306-3016-2886-7721", "018543438", "17046325145", 2021)]
        public async Task GetIncomeMobileValue(string applicationId, string afm, string amka, int refYear)
        {
            var res = await infoClient.GetIncomeMobileValueAsync(new GetIncomeMobileValueRequest
            {
                ApplicationId = applicationId,
                Afm = afm,
                Amka = amka,
                ReferenceYear = refYear
            });

            res._IsSuccessful.Should().BeTrue(res._ErrorsFormatted);
        }
    }    
}