using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.DI.Interfaces
{
    public interface ITargetPublisherFactory
    {
        IPublisher GetPublisher(string environment, PublicationDataSubset dataSubset);
    }
}