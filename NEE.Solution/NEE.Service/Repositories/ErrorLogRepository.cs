using NEE.Core.BO;
using NEE.Core.Contracts;
using NEE.Core;
using NEE.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEE.Database;
using NEE.Core.Security;
using NEE.Database.Entities;

namespace NEE.Service
{
    public class ErrorLogRepository : IErrorLogger
    {
        private readonly NEEDbContextFactory dbFactory;
        private readonly INEECurrentUserContext _currentUserContext;
        private readonly IClock clock;

        public ErrorLogRepository(
            NEEDbContextFactory dbFactory,
            INEECurrentUserContext currentUserContext,
            IClock clock)
        {
            if (currentUserContext == null)
                throw new ArgumentNullException(nameof(currentUserContext));
            if (dbFactory == null)
                throw new ArgumentNullException(nameof(dbFactory));

            this.dbFactory = dbFactory;
            _currentUserContext = currentUserContext;
            this.clock = clock ?? new SystemClock();
        }

        private NEEDbContext CreateDb(string methodName) => dbFactory.Create(methodName, ApplicationConfigurationHelper.IsDbLoggingEnabled);

        public string LogErrorAsync(ErrorLog errorLog)
        {
            using (var db = CreateDb("ErrorLogRepository.LogErrorAsync"))
            {
                var dbError = errorLog.Map();
                db.NEE_ErrorLog.Add(dbError);

                db.SaveChanges();

                return dbError.Id;
            }
        }
    }
}
