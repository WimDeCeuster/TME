using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public class CarConfiguratorPublisher : ICarConfiguratorPublisher
    {
        readonly IPublisherFactory _publisherFactoryFacade;
        readonly IMapper _mapper;

        public CarConfiguratorPublisher(IPublisherFactory publisherFactoryFacade, IMapper mapper)
        {
            if (publisherFactoryFacade == null) throw new ArgumentNullException("publisherFactoryFacade");
            if (mapper == null) throw new ArgumentNullException("mapper");

            _publisherFactoryFacade = publisherFactoryFacade;
            _mapper = mapper;
        }

        public async Task<Result> PublishAsync(Guid generationID, String environment, String target, String brand, String country, PublicationDataSubset dataSubset)
        {
            if (String.IsNullOrWhiteSpace(environment)) throw new ArgumentNullException("environment");
            if (String.IsNullOrWhiteSpace(target)) throw new ArgumentNullException("target");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            
            var context = new Context(brand, country, generationID, dataSubset);

            await _mapper.MapAsync(context);

            var publisher = _publisherFactoryFacade.GetPublisher(target, environment, dataSubset);

            return await publisher.PublishAsync(context);
        }
    }
}
