using NEE.Database;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XServices.Idika
{
    public class OtherBenefitsGateway
    {
        private readonly NEEDbContextFactory dbFactory;

        public OtherBenefitsGateway(NEEDbContextFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }
        private NEEDbContext CreateDb(string methodName) => dbFactory.Create(methodName);

        public Task<List<OtherBenefit>> GetOtherBenefits(string amka, DateTime yearMonth)
        {
            using (var db = CreateDb("OtherBenefitsGateway.GetOtherBenefits"))
            {
                string sql = $@"
					select ""BenefitCategoryCode"", ""Amount"" 
                    from ""ΝΕΕBenefitView""@GMI_REFDB_T 
                    where ""AMKA"" = :p_AMKA
                    and   ""YearMonth"" = :p_YearMonth";
                var p1 = new OracleParameter("p_AMKA", OracleDbType.Varchar2);
                p1.Value = amka;
                var p2 = new OracleParameter("p_YearMonth", OracleDbType.Date);
                p2.Value = yearMonth;
                var qry = db.Database.SqlQuery<OtherBenefit>(sql, p1, p2);
                return qry.ToListAsync();
            }
        }
    }
}
