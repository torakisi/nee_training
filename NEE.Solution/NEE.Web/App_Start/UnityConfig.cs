using System;

using Unity;
using Unity.Injection;

namespace NEE.Web
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {

        #region Unity Container
        private static Lazy<UnityConfig> instance = new Lazy<UnityConfig>(() =>
        {
            var config = new UnityConfig();
            config.RegisterTypes();
            return config;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return instance.Value.container;
        }
        #endregion

        private readonly UnityContainer container;
        private UnityConfig()
        {
            container = new UnityContainer();
        }

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        private void RegisterTypes()
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            RegisterType(() => new Models.ApplicationDbContext("Name=GmiUsersConnection", "(auto)"));
        }
        private void RegisterType<T>(Func<T> factory) =>
            container.RegisterType<T>(new InjectionFactory(_ => factory()));

    }
}