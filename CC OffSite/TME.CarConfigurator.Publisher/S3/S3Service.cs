using System;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Service : IS3Service
    {
        public void PutObject(string key, string item)
        {
            throw new NotImplementedException();
        }

        public Models GetModelsOverview(string brand, string country, string language)
        {
            throw new NotImplementedException();
        }

        public Result PutModelsOverview(string brand, string country, string language, Models models)
        {
            throw new NotImplementedException();
        }
    }
}
