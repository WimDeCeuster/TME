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
                var springContext = ContextRegistry.GetContext();
                return springContext.GetObject(service.ToString());
            }
            catch (Exception e)
            {
                return base.GetInstance(service, key);
            }

        }
    }
}