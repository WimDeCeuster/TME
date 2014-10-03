using System;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3Service
    {
        void PutObject(String key, String item);
        Languages GetModelsOverviewPerLanguage(string brand, string country);
        Result PutModelsOverviewPerLanguage(string brand, string country, Languages languages);
    }
}