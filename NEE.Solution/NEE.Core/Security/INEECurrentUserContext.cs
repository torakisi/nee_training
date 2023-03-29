using System.Security.Principal;

namespace NEE.Core.Security
{
    public interface INEECurrentUserContext
    {
        IPrincipal User { get; }
        string UserName { get; }

        bool IsInRole(string role);

        string ServerName { get; }
        string ServerIP { get; }

        string RemoteHostName { get; }
        string RemoteHostIP { get; }

        string Browser { get; }
        string Agent { get; }
        bool IsNormalUser { get; }
        bool IsAFMUser { get; }
        bool IsKKUser { get; }
        bool IsAdministrator { get; }
        bool IsManager { get; }
        bool IsApplicationSupervisor { get; }
        bool IsDeav_Team { get; }
        bool IsReadOnlyUser { get; }
        bool IsResumeApplicationUsers { get; }
        bool IsPaymentUsers { get; }

    }

}