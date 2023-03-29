using NEE.Database;
using Oracle.ManagedDataAccess.Client;
using System.Threading.Tasks;

namespace XService.Idika
{
	public class AmkaSnapshotGateway
	{
		private readonly NEEDbContextFactory dbFactory;

		public AmkaSnapshotGateway(NEEDbContextFactory dbFactory)
		{
			this.dbFactory = dbFactory;
		}
		private NEEDbContext CreateDb(string methodName) => dbFactory.Create(methodName);

		public async Task<AmkaResult> GetAmkaRegistryInfoByAmka(string amka)
		{
			using (var db = CreateDb("AmkaSnapshotGateway.GetAmkaRegistryInfoByAmka"))
			{
				string sql = @"select AMKA, AFM, ""LastName"", ""FirstName"", ""FatherName"", ""MotherName"", ""Gender"", DOB, DOD, ""LastNameEN"", ""FirstNameEN"", ""CitizenCountry"", COALESCE(""ModifiedAt"", ""CreatedAt"") ""ModifiedAt"" from ""AmkaSnap"" where AMKA = :p_AMKA or ""Initial_AMKA"" = :p_AMKA1 ";

				var p1 = new OracleParameter("p_AMKA", OracleDbType.Varchar2);
				p1.Value = amka;
				var p2 = new OracleParameter("p_AMKA1", OracleDbType.Varchar2);
				p2.Value = amka;
				var qry = db.Database.SqlQuery<AmkaRow>(sql, p1, p2);
				var rows = await qry.ToListAsync();

				return new AmkaResult(rows);
			}
		}
		public async Task<AmkaResult> GetAmkaRegistryInfoByAfm(string afm)
		{
			using (var db = CreateDb("AmkaSnapshotGateway.GetAmkaRegistryInfoByAfm"))
			{
				string sql = @"select AMKA, AFM, ""LastName"", ""FirstName"", ""FatherName"", ""MotherName"", ""Gender"", DOB, DOD, ""LastNameEN"", ""FirstNameEN"", ""CitizenCountry"", COALESCE(""ModifiedAt"", ""CreatedAt"") ""ModifiedAt"" from ""AmkaSnap"" where AFM = :p_AFM ";

				var p1 = new OracleParameter("p_AFM", OracleDbType.Varchar2);
				p1.Value = afm;
				var qry = db.Database.SqlQuery<AmkaRow>(sql, p1);
				var rows = await qry.ToListAsync();

				return new AmkaResult(rows);
			}
		}
	}
}
