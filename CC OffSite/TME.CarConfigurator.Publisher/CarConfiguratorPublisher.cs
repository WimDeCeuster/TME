using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public class CarConfiguratorPublisher : ICarConfiguratorPublisher
    {
        readonly IContextFactory _contextFactory;
        readonly IPublisherFacadeFactory _publisherFacadeFactory;
        readonly IMapper _mapper;
        readonly ICarDbModelGenerationFinder _generationFinder;

        public CarConfiguratorPublisher(IContextFactory contextFactory, IPublisherFacadeFactory publisherFacadeFactory, IMapper mapper, ICarDbModelGenerationFinder generationFinder)
        {
            if (contextFactory == null) throw new ArgumentNullException("contextFactory");
            if (publisherFacadeFactory == null) throw new ArgumentNullException("publisherFacadeFactory");
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (generationFinder == null) throw new ArgumentNullException("generationFinder");

            _contextFactory = contextFactory;
            _publisherFacadeFactory = publisherFacadeFactory;
            _mapper = mapper;
            _generationFinder = generationFinder;
        }

        public Task<Result> Publish(Guid generationID, String environment, String target, String brand, String country, PublicationDataSubset dataSubset)
        {
            if (String.IsNullOrWhiteSpace(environment)) throw new ArgumentNullException("environment");
            if (String.IsNullOrWhiteSpace(target)) throw new ArgumentNullException("target");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            
            var context = _contextFactory.Get(brand, country, generationID, dataSubset);

            _mapper.Map(brand, country, generationID, _generationFinder, context);

            var publisher = _publisherFacadeFactory.GetFacade(target).GetPublisher(environment, dataSubset);

            return publisher.Publish(context);
        }
    }
}
