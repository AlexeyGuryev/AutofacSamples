using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;

namespace ScopeSample
{
    public interface ISimple
    {
        void Do(string param);
    }

    public class Singleton
    {
        
    }
    
    public class Simple : ISimple, IDisposable
    {
        private string param;

        public void Do(string param)
        {
            this.param = param;
            Console.WriteLine(string.Format("Simple is doing {0}", param));
        }

        public void Dispose()
        {
            Console.WriteLine(string.Format("Simple is disposing after {0}", param));
        }
    } 
        
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Program started");
            
            var builder = new ContainerBuilder();
            builder.RegisterType<Simple>().As<ISimple>().InstancePerLifetimeScope()
                .WithMetadata("hoho", "yeyey");
            
            builder.RegisterType<Singleton>().AsSelf().SingleInstance()
                .WithMetadata("AllowRootLifetimeScope", true);

            var container = builder.Build();
            container.NoLifetimeResolutionAtRootScope();

            // will work
            var singleton = container.Resolve<Singleton>();
            
            // throw an exception
            var logInRootScope = container.Resolve<ISimple>();
            logInRootScope.Do("sleeping");
 

            using (var scope = container.BeginLifetimeScope())
            {
                var log = scope.Resolve<ISimple>();
                log.Do("waiting");
            }

            //((IDisposable) rootLog).Dispose();
            Console.WriteLine("Program finished");
        }
    }
    
    public static class NoLifetimeResolutionAtRootScopeExtensions
    {
        /// <summary>
        /// Prevents instances that are lifetime registration from being resolved in the root scope
        /// </summary>
        public static void NoLifetimeResolutionAtRootScope(this IContainer container)
        {
            LifetimeScopeBeginning(null, new LifetimeScopeBeginningEventArgs(container));
        }

        private static void LifetimeScopeBeginning(object sender, LifetimeScopeBeginningEventArgs e)
        {
            e.LifetimeScope.ResolveOperationBeginning += ResolveOperationBeginning;
            e.LifetimeScope.ChildLifetimeScopeBeginning += LifetimeScopeBeginning;
        }

        private static void ResolveOperationBeginning(object sender, ResolveOperationBeginningEventArgs e)
        {
            e.ResolveOperation.InstanceLookupBeginning += InstanceLookupBeginning;
        }

        private static void InstanceLookupBeginning(object sender, InstanceLookupBeginningEventArgs e)
        {
            var registration = e.InstanceLookup.ComponentRegistration;
            var activationScope = e.InstanceLookup.ActivationScope;

            object value;
            var preventChecking = registration.Target.Metadata.TryGetValue("AllowRootLifetimeScope", out value) && (bool)value;
            if (preventChecking)
            {
                return;
            }

            if (registration.Ownership != InstanceOwnership.ExternallyOwned
                && registration.Sharing == InstanceSharing.Shared
                && !(registration.Lifetime is RootScopeLifetime)
                && activationScope.Tag.Equals("root"))
            {
                //would be really nice to be able to get a resolution stack here
                throw new DependencyResolutionException(string.Format(
                    "Cannot resolve a lifetime instance of {0} at the root scope.", registration.Target));
            }
        }
    }
}