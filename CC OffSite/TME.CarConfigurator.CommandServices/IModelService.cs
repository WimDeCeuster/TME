using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface IModelService
    {
        Task<Result> PutModelsByLanguage(String brand, String country, Languages languages);
    }
}
