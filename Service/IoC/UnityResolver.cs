using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Unity;
using Unity.Exceptions;

namespace Service.IoC
{
    public class UnityResolver : IDependencyResolver
    {
        #region Fields

        protected IUnityContainer Container;

        #endregion

        #region Constructors

        public UnityResolver(IUnityContainer container) => Container = container ?? throw new ArgumentNullException(nameof(container));

        ~UnityResolver()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        public IDependencyScope BeginScope()
        {
            var child = Container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return Container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                // log the exception
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IEnumerable<object> services;
            try
            {
                services = Container.ResolveAll(serviceType);

                if (!services.Any())
                {
                    throw new ResolutionFailedException(serviceType, string.Empty, $"Resolution of type {serviceType} failed.", null);
                }
            }
            catch (ResolutionFailedException)
            {
                // log the exception
                return new List<object>();
            }

            return services;
        }

        #endregion

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Container.Dispose();
            }
        }

        #endregion
    }
}