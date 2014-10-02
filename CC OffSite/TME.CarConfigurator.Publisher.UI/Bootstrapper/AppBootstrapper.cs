using System.Windows;
using Caliburn.Micro;
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
    }
}