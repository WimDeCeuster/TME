using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetPublisher
    {
        Task<IEnumerable<Result>> PublishAssets(IContext context);
    }
}