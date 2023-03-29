using NEE.Database;

namespace XServices.Idika
{
    public class AmkaMaritalStatusGateway
    {
        private readonly NEEDbContextFactory dbFactory;

        public AmkaMaritalStatusGateway(NEEDbContextFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }
        private NEEDbContext CreateDb(string methodName) => dbFactory.Create(methodName);

    }
}
