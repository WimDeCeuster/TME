using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher
{

    public interface IPublisherFactory
    {
        IPublisher Get(PublicationTarget target, PublicationEnvironment environment);
    }

    public class PublisherFactory : IPublisherFactory
    {
        public IPublisher Get(PublicationTarget target, PublicationEnvironment environment)
        {
            throw new NotImplementedException();
        }
    }
}
