using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Progress;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarConfiguratorPublisher
    {
        Task PublishAsync(Guid generationId, string environment, string target, string brand, string country, PublicationDataSubset dataSubset, string publishedBy, IProgress<PublishProgress> progress);
    }
}