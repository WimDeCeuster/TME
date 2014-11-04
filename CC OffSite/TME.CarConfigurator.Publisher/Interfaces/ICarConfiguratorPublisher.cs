using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarConfiguratorPublisher
    {
        Task PublishAsync(Guid generationId, String environment, String target, String brand, String country, PublicationDataSubset dataSubset);
    }
}