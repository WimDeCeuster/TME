using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IModelPublisher
    {
        Task<Result> PublishModelsByLanguage(IContext context, Languages languages);
    }
}
