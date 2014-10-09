using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Enums;

namespace TME.CarConfigurator.Publisher
{
    public class PublicationService : IPublicationService
    {
        IContextFactory _contextFactory;
        IPublisherFactory _publisherFactory;
        IMapper _mapper;
        ICarDbModelGenerationFinder _generationFinder;

        public PublicationService(IContextFactory contextFactory, IPublisherFactory publisherFactory, IMapper mapper, ICarDbModelGenerationFinder generationFinder)
        {
            if (contextFactory == null) throw new ArgumentNullException("contextFactory");
            if (publisherFactory == null) throw new ArgumentNullException("publisherFactory");
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (generationFinder == null) throw new ArgumentNullException("generationFinder");

            _contextFactory = contextFactory;
            _publisherFactory = publisherFactory;
            _mapper = mapper;
            _generationFinder = generationFinder;
        }

        public Task<Result> Publish(Guid generationID, String target, String brand, String country, PublicationDataSubset dataSubset)
        {
            var context = _contextFactory.Get(brand, country, generationID, dataSubset);

            _mapper.Map(brand, country, generationID, _generationFinder, context);
            
            var publisher = _publisherFactory.Get(target);

            return publisher.Publish(context);
        }
    }
}
