using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IModelPublisher
    {
        Task PublishModelsByLanguage(IContext context, Languages languages);
    }
}
