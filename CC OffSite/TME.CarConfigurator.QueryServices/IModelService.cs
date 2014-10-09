using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryServices
{
    public interface IModelService
    {
        IEnumerable<Model> GetModels(Context context);
        Languages GetModelsByLanguage(string brand, string country);
    }
}