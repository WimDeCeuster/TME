using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher
{
    public enum PublicationTarget
    {
        S3,
        SQL
    }

    public enum PublicationEnvironment
    {
        Production,
        Acceptance,
        Development
    }

    public enum PublicationDataSubset
    {
        Live,
        Preview
    }

    public class PublicationService
    {
        IContextFactory _contextFactory;
        IPublisherFactory _publisherFactory;
        IMapper _mapper;

        public PublicationService(IContextFactory contextFactory, IPublisherFactory publisherFactory, IMapper mapper)
        {
            if (contextFactory == null) throw new ArgumentNullException("contextFactory");
            if (publisherFactory == null) throw new ArgumentNullException("publisherFactory");
            if (mapper == null) throw new ArgumentNullException("mapper");

            _contextFactory = contextFactory;
            _publisherFactory = publisherFactory;
            _mapper = mapper;
        }

        public void Publish(Guid generationId, PublicationTarget target, PublicationEnvironment environment, PublicationDataSubset dataSubset)
        {
            var context = _contextFactory.Get(generationId, dataSubset);

            //_mapper.Map(context);

            var publisher = _publisherFactory.Get(target, environment);

            publisher.Publish(context);
        }
    }
}
