using System;
using TME.CarConfigurator.Publisher.Enums;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPublicationService
    {
        void Publish(Guid generationId, String target, String brand, String country, PublicationDataSubset dataSubset);
    }
}