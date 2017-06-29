using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Autofac.Test.Issues
{
    public abstract class Issue847
    {
        public interface IThing<T> { }

        public interface IThingFactory { }

        public class Thing<T> : IThing<T>
        {
            public Thing(IThingFactory factory)
            {
                this.factory = factory;
            }

            private IThingFactory factory;
        }

        public class ThingFactory : IThingFactory { }

        private ILifetimeScope _scope;
        
        protected void SetScope(ILifetimeScope scope)
        {
            _scope = scope;
        }

        protected void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<ThingFactory>().As<IThingFactory>().SingleInstance();

            // Logger<T> takes an ILoggerFactory in as a constructor parameter
            builder.RegisterGeneric(typeof(Thing<>)).As(typeof(IThing<>)).SingleInstance();
            // builder.RegisterGeneric(typeof(Thing<>)).As(typeof(IThing<>)).InstancePerLifetimeScope();
        }

        [Fact]
        public void CanResolveGeneric()
        {
            // _scope.Resolve<IThingFactory>();

            // This blows up with an exception saying it can't resolve the ILoggerFactory type.
            _scope.Resolve<IThing<object>>();
        }
    }

    public class Issue847ViaContainer : Issue847
    {
        public Issue847ViaContainer()
        {
            var builder = new ContainerBuilder();
            Configure(builder);

            SetScope(builder.Build());
        }
    }

    public class Issue847ViaScope : Issue847
    {
        public Issue847ViaScope()
        {
            var root = new ContainerBuilder().Build();

            SetScope(root.BeginLifetimeScope(Configure));
        }
    }
}
