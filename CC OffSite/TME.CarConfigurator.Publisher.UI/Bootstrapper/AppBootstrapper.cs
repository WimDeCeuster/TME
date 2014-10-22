using System;
using System.Windows;
using Caliburn.Micro;
using Spring.Context;
using Spring.Context.Support;
using TME.CarConfigurator.Publisher.UI.ViewModels;

namespace TME.CarConfigurator.Publisher.UI.Bootstrapper
{
    public class AppBootstrapper : BootstrapperBase
    {
        private IApplicationContext _springContext = ContextRegistry.GetContext();

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            try
            {
                return _springContext.GetObject(service.ToString());
            }
            catch (Exception)
            {
                return base.GetInstance(service, key);
            }
        }

        ~AppBootstrapper()
        {
            if (_springContext == null) return;

            _springContext.Dispose();

            _springContext = null;
        }
    }
}