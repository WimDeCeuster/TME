using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPublisherFacade
    {
        IPublisher GetPublisher();
    }
}
