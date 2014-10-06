using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public class PublicationService : IPublicationService
    {
        IContextFactory _contextFactory;
        IPublisherFactory _publisherFactory;
        IMapper _mapper;
        ICarDbModelGenerationFinder _generationFinder;
        IServiceFactory _serviceFactory;

        public PublicationService(IContextFactory contextFactory, IPublisherFactory publisherFactory, IServiceFactory serviceFactory,
                                  IMapper mapper, ICarDbModelGenerationFinder generationFinder)
        {
            if (contextFactory == null) throw new ArgumentNullException("contextFactory");
            if (publisherFactory == null) throw new ArgumentNullException("publisherFactory");
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (generationFinder == null) throw new ArgumentNullException("generationFinder");
            if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");

            _contextFactory = contextFactory;
            _publisherFactory = publisherFactory;
            _mapper = mapper;
            _generationFinder = generationFinder;
            _serviceFactory = serviceFactory;
        }

        public Task<Result> Publish(Guid generationID, String target, String brand, String country, PublicationDataSubset dataSubset)
        {
            var context = _contextFactory.Get(brand, country, generationID, dataSubset);
            var service = _serviceFactory.Get(target, brand, country);

            _mapper.Map(brand, country, generationID, _generationFinder, context);

            var publisher = _publisherFactory.Get(service);

            return publisher.Publish(context);
        }
    }
}
