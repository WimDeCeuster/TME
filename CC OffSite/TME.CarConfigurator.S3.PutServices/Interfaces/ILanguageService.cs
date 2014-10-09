using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.PutServices.Interfaces
{
    public interface ILanguageService
    {
        Task<Result> PutModelsOverviewPerLanguage(IContext context, Languages languages);
    }
}
