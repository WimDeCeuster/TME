using System;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public interface IPublisherFactory
    {
        IPublisher GetPublisher(String target, String environment, PublicationDataSubset dataSubset);
    }
}