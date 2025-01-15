

using Autofac;

namespace VendersCloud.Business
{
    public class IoC
    {
        private static IContainer _container;

        public IoC(Func<IContainer> func, IServiceProvider serviceProvider = null)
        {
            _container = func();
            ServiceProvider = serviceProvider;
        }

        public static IServiceProvider ServiceProvider
        {
            get;
            set;
        }

        public static IContainer Container
        {
            get
            {
                return _container;
            }
        }

    }
}
