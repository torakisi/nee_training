using NEE.Core.Contracts;
using NEE.Core.Contracts.Enumerations;
using System;
using System.Threading.Tasks;

namespace NEE.Service
{
    partial class AppService
    {
        public async Task<int?> GetOpekaDistrict()
        {
            var userName = _currentUserContext.User.Identity.Name;
            return await repository.GetOpekaDistrict(userName);
        }
    }    
}
