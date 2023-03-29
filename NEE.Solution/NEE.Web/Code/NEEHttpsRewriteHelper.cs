using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NEE.Web.Code
{
    public static class NEEHttpsRewriteHelper
    {
        public static bool ForceHttpsMode
        {
            get
            {
                bool forceHttps = false;
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["opeka:force-https"], out forceHttps);
                return forceHttps;
            }
        }

        public static string RewriteUrl(string url)
        {
            var uri = new UriBuilder(url);
            if (uri.Host.ToLower().EndsWith(".gr"))
            {
                uri.Scheme = "https";
                uri.Port = -1;
            }
            return url = uri.ToString();
        }
    }
}