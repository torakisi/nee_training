using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using NEE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace NEE.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        #region Override Properties: PhoneNumber, PasswordHash, SecurityStamp (set string-lengths,  will cause them to be mapped to NVARCHAR2 instead of NCLOB in Oracle as in the default EF implementation)

        [StringLength(30)]
        public override string PhoneNumber { get; set; }

        [StringLength(256)]
        public override string PasswordHash { get; set; }

        [StringLength(256)]
        public override string SecurityStamp { get; set; }

        #endregion


        #region Extra Properties: Payload: IsActive, LastName, FirstName

        public bool IsActive { get; set; } = true;

        [StringLength(30)]
        public string LastName { get; set; }

        [StringLength(30)]
        public string FirstName { get; set; }

        #endregion

        #region  Extra Properties: Computed: FullName

        public string FullName => $"{this.FirstName} {this.LastName}".Trim();

        #endregion

        #region  Extra Properties: Navigation: NEE_UserDetails (loose coupled entity)

        public virtual NEE.Database.Entities_Extra.GMI_UserDetails GMI_UserDetails { get; set; }

        #endregion
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        static ApplicationDbContext()
        {
            // Disable Automatic Creation and Take manual control
            System.Data.Entity.Database.SetInitializer<ApplicationDbContext>(null);
        }


        private string _defaultSchema;

        public ApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString, throwIfV1Schema: false)
        {
            this.Database.Log = (m) => { System.Diagnostics.Debug.WriteLine(m); };
        }

        public ApplicationDbContext(string nameOrConnectionString, string defaultSchema) : this(nameOrConnectionString)
        {
            _defaultSchema = defaultSchema;
        }

        public static ApplicationDbContext Create()//(string databaseConnection, string defaultSchema)
        {
            return new ApplicationDbContext("GmiDB", "(auto)");
            //return new ApplicationDbContext(databaseConnection, defaultSchema);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var schema = DefaultDbSchemaHelper.ResolveDbSchema(_defaultSchema, this.Database.Connection.ConnectionString);
            if (!string.IsNullOrWhiteSpace(schema))
                modelBuilder.HasDefaultSchema(schema);

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<NEE.Database.Entities_Extra.GMI_UserDetails> GMI_UserDetails { get; set; }

    }
}