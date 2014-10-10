using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Factories
{
    public class PublisherFacadeFactory : IPublisherFacadeFactory
    {
        public IPublisherFacade GetFacade(String target)
        {
            return (IPublisherFacade)ContextRegistry.GetContext().GetObject(String.Format("{0}PublisherFacade", target));
        }
    }
}
