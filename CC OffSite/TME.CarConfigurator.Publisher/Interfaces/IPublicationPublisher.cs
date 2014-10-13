using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPublicationPublisher
    {
        Task<IEnumerable<Result>> PublishPublications(IContext context);
    }
}
