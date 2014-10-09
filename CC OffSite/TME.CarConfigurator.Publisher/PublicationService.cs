using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public class PublicationService : IPublicationService
    {
        readonly IContextFactory _contextFactory;
        readonly IPublisherFacadeFactory _publisherFacadeFactory;
        readonly IMapper _mapper;
        readonly ICarDbModelGenerationFinder _generationFinder;

        public PublicationService(IContextFactory contextFactory, IPublisherFacadeFactory publisherFacadeFactory, IMapper mapper, ICarDbModelGenerationFinder generationFinder)
        {
            if (contextFactory == null) throw new ArgumentNullException("contextFactory");
            if (publisherFacadeFactory == null) throw new ArgumentNullException("publisherFactory");
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (generationFinder == null) throw new ArgumentNullException("generationFinder");

            _contextFactory = contextFactory;
            _publisherFacadeFactory = publisherFacadeFactory;
            _mapper = mapper;
            _generationFinder = generationFinder;
        }

        public Task<Result> Publish(Guid generationID, String target, String brand, String country, PublicationDataSubset dataSubset)
        {
            var context = _contextFactory.Get(brand, country, generationID, dataSubset);

            _mapper.Map(brand, country, generationID, _generationFinder, context);
            
            var publisher = _publisherFacadeFactory.GetFacade(target).GetPublisher("Development", dataSubset);

            return publisher.Publish(context);
        }
    }
}
