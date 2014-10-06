using Spring.Context.Support;
using System;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Factories
{
    public class PublisherFactory : IPublisherFactory
    {
        public IPublisher Get(IService service)
        {
            var springContext = ContextRegistry.GetContext();
            return (IPublisher)springContext.CreateObject("Publisher", typeof(IPublisher), new object[] { service });
        }
    }
}
