using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IService
    {
        Languages GetModelsOverviewPerLanguage();
        Task<Result> PutModelsOverviewPerLanguage(Languages languages);
        Task<Result> PutPublication(Publication publication);
    }
}