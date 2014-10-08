using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Service.Interfaces
{
    public interface ILanguageService
    {
        Languages GetLanguages(IContext context);
    }
}