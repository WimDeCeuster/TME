using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarConfiguratorPublisher
    {
        Task<Result> Publish(Guid generationId, String environment, String target, String brand, String country, PublicationDataSubset dataSubset);
    }
}