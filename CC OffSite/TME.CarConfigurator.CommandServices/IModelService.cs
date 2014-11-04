using System;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface IModelService
    {
        Task PutModelsByLanguage(String brand, String country, Languages languages);
    }
}
