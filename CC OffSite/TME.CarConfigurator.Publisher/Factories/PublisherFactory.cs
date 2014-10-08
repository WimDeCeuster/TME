using Spring.Context;
using Spring.Context.Support;
using System;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Factories
{
    public class PublisherFactory : IPublisherFactory
    {
        public IPublisher Get(String target)
        {
            var springContext = ContextRegistry.GetContext();
            switch (target)
            {
                case "S3":
                    return (IPublisher)springContext.GetObject("S3Publisher");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
