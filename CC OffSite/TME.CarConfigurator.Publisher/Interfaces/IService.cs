using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IService
    {
        Languages GetModelsOverviewPerLanguage(String brand, String country);
        Result PutModelsOverviewPerLanguage(String brand, String country, Languages languages);
        Task<Result> PutPublication(String language, Publication publication);
    }
}