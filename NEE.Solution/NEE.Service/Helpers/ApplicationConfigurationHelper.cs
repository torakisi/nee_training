using System.Configuration;

namespace NEE.Service.Helpers
{
    public class ApplicationConfigurationHelper
    {
        public static bool IsDbLoggingEnabled
        {
            get
            {
                return ConfigurationManager.AppSettings["EnableDbLogging"] == "true" ? true : false;
            }
        }
    }
}
