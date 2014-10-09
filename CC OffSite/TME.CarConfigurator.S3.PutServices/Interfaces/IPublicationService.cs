using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.CommandServices.Interfaces
{
    public interface IPublicationService
    {
        Task<IEnumerable<Result>> PutPublications(IContext context);
    }
}
