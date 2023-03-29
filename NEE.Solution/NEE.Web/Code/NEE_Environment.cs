using System.Configuration;

namespace NEE.Web.Code
{
    public static class NEE_Environment
    {
        public static bool IsDEBUG()
        {
            return ConfigurationManager.AppSettings["ApplicationShowDebugPage"] == "false" ? false : true;
        }

        public static bool IsStaging()
        {
            var environmentTag = ConfigurationManager.AppSettings["AppInsightsEnvironmentTag"];
            return environmentTag == "Staging";
        }
        public static bool IsProduction()
        {
            var environmentTag = ConfigurationManager.AppSettings["AppInsightsEnvironmentTag"];
            return environmentTag == "Production";
        }

        public static bool IsAdmin()
        {
            if (IsDEBUG())
                return true;

            var environmentTag = ConfigurationManager.AppSettings["AppInsightsEnvironmentTag"];
            return IsAdminEnabled() && environmentTag == "Admin";
        }
        public static bool IsAdminEnabled()
        {
            var enableAdminTag = ConfigurationManager.AppSettings["ApplicationForPublic"] ?? "true";
            return enableAdminTag.ToLower() == "false";
        }

        public static bool IsDBDev()
        {
            var dataBaseConnection = ConfigurationManager.ConnectionStrings["NEEDataConnection"];

            if (dataBaseConnection.ConnectionString.Contains("NEE_TEST"))
            {
                return true;
            }

            return false;
        }
    }
}