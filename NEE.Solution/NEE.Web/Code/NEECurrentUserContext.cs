using NEE.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace NEE.Web
{
    public class NEECurrentUserContext : INEECurrentUserContext
    {
        public IPrincipal User => HttpContext.Current.GetOwinContext().Request.User;

        public string UserName
        {
            get
            {
                string userName = HttpContext.Current.GetOwinContext().Request.User?.Identity?.Name;

                List<string> systemUsers = new List<string>()
                {
                    "torakisi@cbs.gr"
                };

                if (systemUsers.Contains(userName))
                {
                    return "System_Audit";
                }

                return userName;
            }
        }

        public bool IsInRole(string role) => HttpContext.Current.GetOwinContext().Request.User.IsInRole(role);
        public string ServerName => System.Net.Dns.GetHostName();
        public string ServerIP => System.Net.Dns.GetHostAddresses(this.ServerName).Where(x => (x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)).FirstOrDefault()?.ToString();
        public string RemoteHostName => HttpContext.Current.Request.UserHostName;
        public string RemoteHostIP => NEECurrentUserContext.GetRemoteHostIPs().First();

        public string Browser => HttpContext.Current.Request.Browser.Type;
        public string Agent => HttpContext.Current.Request.UserAgent;
        public bool IsNormalUser => HttpContext.Current.GetOwinContext().Request.User?.Identity?.GetNEEUserType() == NEEUserHelper.NEEUserType.Normal;
        public bool IsAFMUser => HttpContext.Current.GetOwinContext().Request.User?.Identity?.GetNEEUserType() == NEEUserHelper.NEEUserType.Afm;
        public bool IsKKUser => IsNormalUser && (IsInRole("NEEUsers") || User.IsInRole("OpekaNEEUsers"));
        public bool IsAdministrator => IsNormalUser && IsInRole("Administrators");
        public bool IsManager => IsNormalUser && IsInRole("Managers");
        public bool IsApplicationSupervisor => IsNormalUser && IsInRole("ApplicationSupervisors");
        public bool IsDeav_Team => IsNormalUser && IsInRole("DEV_Team");

        public bool IsReadOnlyUser => IsNormalUser && IsInRole("ReadOnly");
        public bool IsResumeApplicationUsers => IsNormalUser && IsInRole("ResumeApplicationUsers");
        public bool IsPaymentUsers => IsNormalUser && IsInRole("PaymentUsers");


        public static string[] GetRemoteHostIPs()
        {
            var ips = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrWhiteSpace(ips))
                ips = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrWhiteSpace(ips))
                ips = HttpContext.Current.Request.UserHostAddress;

            return ips.Split(',').Reverse().ToArray();
        }

    }
}