using System.Text.RegularExpressions;

namespace NEE.Core.Helpers
{
    public static class DefaultDbSchemaHelper
    {
        private static Regex _regexUserId = new Regex(@"User Id=(?<userId>[^;]*)", (RegexOptions.Compiled | RegexOptions.IgnoreCase));
        private static Regex _regexDataSourcePort = new Regex(@"Data Source=[^;]*:(?<port>\d*)\/.*;*", (RegexOptions.Compiled | RegexOptions.IgnoreCase));

        /// <summary>
        /// Resolves a Db Schema Name from a connectionString if neccecary (using it's "User Id" as default schema)
        /// </summary>
        /// <param name="schemaOrSpecifier">null or schema or "(none)" or "(user)" or "(auto)"</param>
        /// <param name="connectionString">a connection string to extract it's "User Id" and return it as a schema</param>
        /// <returns>the schema passed, the connection's "User Id" or null</returns>
        // TODO: update comments! 
        public static string ResolveDbSchema(string schemaOrSpecifier, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(schemaOrSpecifier)) return null;
            if (schemaOrSpecifier == "(none)") return null;
            if (schemaOrSpecifier == "(auto)")
            {
                var port = _regexDataSourcePort.Match(connectionString).Groups["port"]?.Value;
                if (port == "1521")
                    schemaOrSpecifier = "(user)";       // fall-back to user
            }
            if (schemaOrSpecifier == "(user)")
            {
                schemaOrSpecifier = _regexUserId.Match(connectionString).Groups["userId"]?.Value;
            }
            if (string.IsNullOrWhiteSpace(schemaOrSpecifier)) return null;
            return schemaOrSpecifier.Trim();
        }

    }
}
