using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Enums.Result;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPublicationService
    {
        Task<Result> Publish(Guid generationId, String target, String brand, String country, PublicationDataSubset dataSubset);
    }
}