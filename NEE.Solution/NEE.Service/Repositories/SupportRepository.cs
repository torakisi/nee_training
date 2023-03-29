using NEE.Core.Security;
using NEE.Core;
using NEE.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using NEE.Core.BO;
using NEE.Service.Helpers;
using NEE.Database.Entities;

namespace NEE.Service
{
    public class SupportRepository
    {
        private readonly NEEDbContextFactory dbFactory;
        private readonly INEECurrentUserContext _currentUserContext;
        private readonly IClock clock;

        public SupportRepository(
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

        public List<Faq> GetAllFaqs()
        {
            try
            {
                using (var db = CreateDb("SupportRepository.GetAllFaqs"))
                {
                    var faqs = db.SUP_Faq.ToList();

                    List<Faq> ret = new List<Faq>();

                    foreach (SUP_Faq faq in faqs)
                    {
                        ret.Add(faq.Map());
                    }

                    return ret;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
