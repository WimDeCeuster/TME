using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Progress;

namespace TME.CarConfigurator.Publisher
{
    public class CarConfiguratorPublisher : ICarConfiguratorPublisher
    {
        readonly IPublisherFactory _publisherFactory;
        readonly IMapper _mapper;

        public CarConfiguratorPublisher(IPublisherFactory publisherFactory, IMapper mapper)
        {
            if (publisherFactory == null) throw new ArgumentNullException("publisherFactory");
            if (mapper == null) throw new ArgumentNullException("mapper");

            _publisherFactory = publisherFactory;
            _mapper = mapper;
        }

        public async Task PublishAsync(Guid generationID, string environment, string target, string brand, string country, PublicationDataSubset dataSubset, IProgress<PublishProgress> progress)
        {
            if (String.IsNullOrWhiteSpace(environment)) throw new ArgumentNullException("environment");
            if (String.IsNullOrWhiteSpace(target)) throw new ArgumentNullException("target");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");

            // create a progress object when client isn't interested in it, instead of doing nullchecks on progess object in further code
            if (progress == null) progress = new Progress<PublishProgress>();

            var context = new Context(brand, country, generationID, dataSubset);

            await MapAsync(progress, context);

            await PublishAsync(environment, target, dataSubset, context, progress);
        }

        private async Task MapAsync(IProgress<PublishProgress> progress, IContext context)
        {
            var startOfMapping = DateTime.Now;

            progress.Report(new PublishProgress("Mapping started"));

            await _mapper.MapAsync(context, progress);

            progress.Report(new PublishProgress(string.Format("Mapping completed after {0} seconds", DateTime.Now.Subtract(startOfMapping).TotalSeconds)));
        }

        private async Task PublishAsync(string environment, string target, PublicationDataSubset dataSubset, IContext context, IProgress<PublishProgress> progress)
        {
            var publisher = _publisherFactory.GetPublisher(target, environment, dataSubset);

            var startOfPublishing = DateTime.Now;

            progress.Report(new PublishProgress("Publishing started"));

            await publisher.PublishAsync(context);

            progress.Report(new PublishProgress(string.Format("Publishing completed after {0} seconds", DateTime.Now.Subtract(startOfPublishing).TotalSeconds)));
        }
    }
}
