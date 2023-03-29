using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEE.Core.Helpers;
using System;
using System.Threading.Tasks;
using XServices.Edto;

namespace XServices.Tests
{
    [TestClass]
    public class KEDServiceTests
    {
        private KEDService client;

        [TestInitialize]
        public void Initialize()
        {
            client = KEDService.CreateDefault();
        }

        [DataTestMethod]
        //[DataRow("LICI", "JORGJIA", "KRISTO", "ZAHARULLA", "01/12/1987", "2301-3011-2886-7723", "142818104", "25065002344")]
        [DataRow("MASTORA", "SPIRO", "VASIL", "SOFIA", "24/11/1953", "2301-3011-2886-7723", "142818104", "25065002344")]
        [DataRow("JORGAQI", "KSENOFON", "KRISTAQ", "LEKSO", "21/02/1952", "2301-3011-2886-7723", "142818104", "25065002344")]
        //[DataRow("GJIKA", "MITRO", "ILOS", "XRISANTI", "09/08/1948", "2301-3011-2886-7715", "068933130", "25065002344")]
        //[DataRow("VAJA", "ALEKSANDRA", "STAVRO", "VASILLO", "09/04/1939", "2301-3011-2886-7716", "068933131", "25065002345")]
        public async Task GetEdtoProfile(string lastName, string firstName, string fathersName, string mothersName, string birthDate, string applicationId, string afm, string amka)
        {
            var res = await client.GetPolicePersonDetailsAsync(new GetPolicePersonDetailsRequest
            {
                LastName = lastName,
                FirstName = firstName,
                FathersName = fathersName,
                MothersName = mothersName,
                BirthDate = birthDate,
                ApplicationId = applicationId,
                Afm = afm,
                Amka = amka
            });

            res._IsSuccessful.Should().BeTrue(res._ErrorsFormatted);
        }
    }

    
}