using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.QueryServices.Interfaces
{
    public interface IModelService
    {
        IEnumerable<Model> GetModels(Context context);
        Languages GetModelsByLanguage(string brand, string country);
    }
}