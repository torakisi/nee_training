using NEE.Core.Contracts.Enumerations;
using NEE.Core.Helpers;
using NEE.Core.Security;
using NEE.Database.Entities;
using NEE.Database.Entities_Extra;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace NEE.Database
{
    public class NEEDbContext : DbContext
    {
        static NEEDbContext()
        {
            System.Data.Entity.Database.SetInitializer<NEEDbContext>(null);
        }

        private string _defaultSchema;

        public NEEDbContext(string nameOrConnectionString, string defaultSchema) : base(nameOrConnectionString)
        {
            _defaultSchema = defaultSchema;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var schema = DefaultDbSchemaHelper.ResolveDbSchema(_defaultSchema, this.Database.Connection.ConnectionString);
            if (!string.IsNullOrWhiteSpace(schema))
                modelBuilder.HasDefaultSchema(schema);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //IdikaDbNamingConventions.Configure(modelBuilder, false);

            // every property of type DateTime should have a column type of "timestamp" (Oracle):
            modelBuilder.Properties<DateTime>().Configure(property => property.HasColumnType("timestamp"));

            modelBuilder.Properties<string>().Configure(property => property.HasColumnType("varchar2"));

            modelBuilder.Entity<NEE_App>()
                .HasMany(x => x.Members)
                .WithRequired()
                .HasForeignKey(x => x.Id);

            modelBuilder.Entity<NEE_App>()
                .HasMany(x => x.Remarks)
                .WithOptional()
                .HasForeignKey(x => x.Id);

            modelBuilder.Entity<NEE_App>()
                .HasMany(x => x.ApplicationStateChange)
                .WithOptional()
                .HasForeignKey(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<NEE_App> NEE_App { get; set; }

        public DbSet<NEE_AppPerson> NEE_AppPerson { get; set; }
        public DbSet<NEE_AppRemark> NEE_AppRemark { get; set; }
        public DbSet<NEE_AppDistrict> NEE_AppDistrict { get; set; }
        public DbSet<NEE_AppFile> NEE_AppFile { get; set; }
        public DbSet<NEE_ChangeLog> NEE_ChangeLog { get; set; }
        public DbSet<NEE_AppSettings> NEE_AppSettings { get; set; }
        public DbSet<NEE_ErrorLog> NEE_ErrorLog { get; set; }
        public DbSet<DIC_ZipCodes> DIC_ZipCodes { get; set; }
        public DbSet<NEE_AppStateChange> NEE_AppStateChange { get; set; }
        public DbSet<GMI_UserDetails> GMI_UserDetails { get; set; }
        public DbSet<AADE_Log> AADE_Log { get; set; }
        public DbSet<KED_Log> KED_Log { get; set; }
        public DbSet<SUP_Faq> SUP_Faq { get; set; }
        public DbSet<NEE_PaymentTransactions> NEE_PaymentTransactions { get; set; }
        public DbSet<NEE_PaymentsWebView> NEE_PaymentsWebView { get; set; }
        public bool UpdateCreatedAndModifiedState(INEECurrentUserContext user, DateTime dt)
        {
            var appRevisionMustChangeBecauseOfRelatedEntities = false;
            this.ChangeTracker.Entries<IProvideCreatedAndModified>().ToList().ForEach(x =>
            {
                if (x.Entity is NEE_AppPerson)
                {
                    appRevisionMustChangeBecauseOfRelatedEntities = true;
                }

                if (x.State == EntityState.Added)
                {
                    if (x.Entity is NEE_App || x.Entity is NEE_AppPerson)
                    {
                        x.Entity.CreatedAt = dt;
                        x.Entity.CreatedBy = user?.UserName;
                        x.Entity.ModifiedAt = dt;
                        x.Entity.ModifiedBy = user?.UserName;
                        x.Entity.Revision = 1;

                        if (x.Entity is NEE_App)
                        {
                            var version = ConfigurationManager.AppSettings["version"] ?? "1";
                            var application = (x.Entity as NEE_App);
                            application.Version = int.Parse(version);

                            if (!application.IsModifiedByKK && user != null)
                            {
                                application.IsModifiedByKK = user.IsNormalUser;
                            }
                        }

                        CreateChangeLog(user?.UserName, dt, x, ChangeLogTypes.Insert);
                    }
                }

                if (x.State == EntityState.Modified)
                {
                    if (x.Entity is NEE_App || x.Entity is NEE_AppPerson)
                    {
                        bool valueHasChange = false;

                        foreach (string opn in x.OriginalValues.PropertyNames)
                        {
                            if (x.OriginalValues[opn] == null && x.CurrentValues[opn] != null)
                            {
                                CreateChangeLog(user?.UserName, dt, x, ChangeLogTypes.Update, opn);
                                valueHasChange = true;
                            }
                            else if (x.OriginalValues[opn] != null && !x.OriginalValues[opn].Equals(x.CurrentValues[opn]))
                            {
                                CreateChangeLog(user?.UserName, dt, x, ChangeLogTypes.Update, opn);
                                valueHasChange = true;
                            }
                        }
                        if (valueHasChange)
                        {
                            x.Entity.ModifiedAt = dt;
                            x.Entity.ModifiedBy = user?.UserName;
                            x.Entity.Revision++;

                            if (x.Entity is NEE_App)
                            {
                                var application = (x.Entity as NEE_App);

                                if (!application.IsModifiedByKK && user != null)
                                {
                                    application.IsModifiedByKK = user.IsNormalUser;
                                }
                            }
                        }
                    }
                }

                if (x.State == EntityState.Deleted)
                {
                    if (x.Entity is NEE_App || x.Entity is NEE_AppPerson)
                    {
                        CreateChangeLog(user?.UserName, dt, x, ChangeLogTypes.Delete);
                    }
                }
            });

            return appRevisionMustChangeBecauseOfRelatedEntities;
        }

        private void CreateChangeLog(string user, DateTime dt, DbEntityEntry<IProvideCreatedAndModified> x, ChangeLogTypes type, string opn = null)
        {
            int? revision = null;
            if (x.Entity is NEE_App)
            {
                revision = (x.Entity as NEE_App).Revision;
            }

            NEE_ChangeLog changeLog = new NEE_ChangeLog()
            {
                ChangeLogId = Guid.NewGuid().ToString(),
                Id = x.Entity.Id,
                User = user,
                ModifiedAt = dt,
                ChangedField = opn,
                EntityType = x.Entity.GetType().Name,
                Revision = x.Entity.Revision,
                EntityId = x.Entity.EntityId,
                Type = type
            };

            if (opn != null)
            {
                changeLog.OriginalValue = x.OriginalValues[opn]?.ToString();
                changeLog.CurrentValue = x.CurrentValues[opn]?.ToString();
            }

            NEE_ChangeLog.Add(changeLog);
        }
    }
}
