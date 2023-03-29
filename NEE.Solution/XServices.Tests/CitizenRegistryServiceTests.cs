using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEE.Core.Contracts.Enumerations;
using System.Threading.Tasks;
using XServices.CitizenRegistry;

namespace XServices.Tests
{
    [TestClass]
    public class CitizenRegistryServiceTests
    {
        private CitizenRegistryService client;

        [TestInitialize]
        public void Initialize()
        {
            client = CitizenRegistryService.CreateDefault();
        }

        [DataTestMethod]
        //[DataRow("2305-3014-2889-7823", "019285930", "17046325145", "ΤΟΡΑΚΗΣ", "ΙΩΑΝΝΗΣ", "08/12/1995", MaritalStatus.Single)]
        //[DataRow("2305-3014-2889-7824", "019285930", "17046325145", "ΠΕΠΟΝΑΚΗ", "ΧΡΥΣΑΝΘΗ", "03/02/1965", MaritalStatus.Married)]
        //[DataRow("2305-3014-2889-7825", "019285930", "17046325145", "ΜΑΘΙΟΥΔΑΚΗ", "ΔΕΣΠΟΙΝΑ", "02/12/1964", MaritalStatus.Divorsed)]
        //[DataRow("2305-3014-2889-7826", "019285930", "17046325145", "Πανέρας", "Νικόλαος", "08/08/1984", MaritalStatus.CivilUnion)]
        //[DataRow("2305-3014-2889-7827", "019285930", "17046325145", "Αργύρης", "Ευστάθιος", "03/10/1986", MaritalStatus.CivilUnion)]
        //[DataRow("2305-3014-2889-7828", "019285930", "17046325145", "Καραμπίνη", "Αργυρώ", "20/12/1966", MaritalStatus.CivilUnion)]
        //[DataRow("2305-3014-2889-7829", "019285930", "17046325145", "Βέττας", "Πέτρος", "31/12/1945", MaritalStatus.CivilUnionBreak)]
        //[DataRow("2305-3014-2889-7820", "019285930", "17046325145", "Σαβουϊδάκης", "Ιωάννης", "13/02/1983", MaritalStatus.Widoed)]
        //[DataRow("2305-3014-2889-7821", "019285930", "17046325145", "Γεωργίου", "Γεώργιος", "02/02/1951", MaritalStatus.Widoed)]
        //[DataRow("2305-3014-2889-7823", "019285930", "17046325145", "ΠΟΥΡΣΑΛΙΔΗΣ", "ΕΥΑΓΓΕΛΟΣ", "13/07/1986", MaritalStatus.CivilUnionBreak)]
        //[DataRow("2305-3014-2889-7823", "019285930", "17046325145", "ΜΟΥΣΤΑΚΗ", "ΘΕΥΡΩΝΙΑ", "29/06/1940", MaritalStatus.CivilUnionBreak)]
        [DataRow("2305-3014-2889-7823", "019285930", "17046325145", "ΒΛΑΣΣΗΣ", "ΣΤΕΦΑΝΟΣ", "14/03/1946", MaritalStatus.CivilUnionBreak)]
        //[DataRow("2305-3014-2889-7823", "019285930", "17046325145", "ΝΤΑΒΙΟΣ", "ΘΕΜΙΣΤΟΚΛΗΣ", "03/05/1984", MaritalStatus.Divorsed)]
        //[DataRow("2305-3014-2889-7823", "019285930", "17046325145", "Κονδυλόπουλος", "Διονύσιος", "09/02/1977", MaritalStatus.Divorsed)]
        public async Task GetMaritalStatusCertificate(string applicationId, string afm, string amka, string lastname, string firstname, string birthdate, MaritalStatus maritalStatus)
        {
            var res = await client.GetMaritalStatusCertificateAsync(new GetMaritalStatusRequest
            {
                ApplicationId = applicationId,
                Afm = afm,
                Amka = amka,
                FirstName = firstname,
                LastName = lastname,
                BirthDate = birthdate
            });
            Assert.AreEqual(maritalStatus, res.MaritalStatus);
        }
    }

    
}