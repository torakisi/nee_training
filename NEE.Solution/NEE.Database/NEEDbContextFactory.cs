namespace NEE.Database
{
    public class NEEDbContextFactory
    {
        private string _nameOrConnectionString;
        private string _defaultSchema;

        /// <summary>
        /// Initializes a new instance of the OpekaDbContextFactory class.
        /// </summary>
        public NEEDbContextFactory(string nameOrConnectionString, string defaultSchema = null)
        {
            _nameOrConnectionString = nameOrConnectionString;
            _defaultSchema = defaultSchema;
        }


        public static NEEDbContextFactory CreateDefault()
        {
            var ret = new NEEDbContextFactory
            (
                nameOrConnectionString: "Name=NEEDataConnection",
                defaultSchema: "(auto)"
            );
            return ret;
        }

        /// <summary>
        /// Creates a NEEDbContext
        /// </summary>
        /// <returns></returns>
        public NEEDbContext Create(string methodName = null, bool enableLogging = false)  // ήταν false. 
        {
            NEEDbContext db = new NEEDbContext(_nameOrConnectionString, _defaultSchema);

            if (enableLogging)
            {
                DbQueryLogger logger = new DbQueryLogger();
                db.Database.Log = s => logger.Log("NEEApp", s, methodName);
            }

            return db;
        }
    }
}
