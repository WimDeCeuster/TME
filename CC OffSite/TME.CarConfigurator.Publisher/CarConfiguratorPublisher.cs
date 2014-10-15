using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public class CarConfiguratorPublisher : ICarConfiguratorPublisher
    {
        readonly String _environment;
        readonly IContextFactory _contextFactory;
        readonly IPublisherFacadeFactory _publisherFacadeFactory;
        readonly IMapper _mapper;
        readonly ICarDbModelGenerationFinder _generationFinder;

        public CarConfiguratorPublisher(String environment, IContextFactory contextFactory, IPublisherFacadeFactory publisherFacadeFactory, IMapper mapper, ICarDbModelGenerationFinder generationFinder)
        {
            if (environment == null) throw new ArgumentNullException("environment");
            if (contextFactory == null) throw new ArgumentNullException("contextFactory");
            if (publisherFacadeFactory == null) throw new ArgumentNullException("publisherFacadeFactory");
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (generationFinder == null) throw new ArgumentNullException("generationFinder");

            if (String.IsNullOrWhiteSpace(environment)) throw new ArgumentException("environment cannot be empty");

            _contextFactory = contextFactory;
            _publisherFacadeFactory = publisherFacadeFactory;
            _mapper = mapper;
            _generationFinder = generationFinder;
            _environment = environment;
        }

        public Task<Result> Publish(Guid generationID, String target, String brand, String country, PublicationDataSubset dataSubset)
        {
            var context = _contextFactory.Get(brand, country, generationID, dataSubset);

            _mapper.Map(brand, country, generationID, _generationFinder, context);

            var publisher = _publisherFacadeFactory.GetFacade(target).GetPublisher(_environment, dataSubset);

            return publisher.Publish(context);
        }
    }
}
