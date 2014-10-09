using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.UI.DI.Interfaces;

namespace TME.CarConfigurator.Publisher.UI.DI.Facades
{
    public class S3PublisherFacade : IPublisherFacade
    {
        public S3PublisherFacade(IS3ServiceFactory serviceFactory)
        {

        }

        public IPublisher GetPublisher()
        {
            throw new NotImplementedException();
        }
    }
}
