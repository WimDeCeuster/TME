using System;
using Spring.Context.Support;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.DI.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.DI
{
    public class PublisherFactory : IPublisherFactory
    {
        public IPublisher GetPublisher(string target, string environment, PublicationDataSubset dataSubset)
        {
            var targetPublisherFactory = (ITargetPublisherFactory)ContextRegistry.GetContext().GetObject(String.Format("{0}PublisherFactory", target));

            return targetPublisherFactory.GetPublisher(environment, dataSubset);
        }
    }
}
