using System;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3Service
    {
        void PutObject(String key, String item);
        Languages GetModelsOverview(string brand, string country);
        Result PutModelsOverview(string brand, string country, Languages languages);
    }
}