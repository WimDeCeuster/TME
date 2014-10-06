using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher
{
    public class CarDbModelGenerationFinder : ICarDbModelGenerationFinder
    {
        public IReadOnlyDictionary<String, Tuple<ModelGeneration, Model>> GetModelGeneration(String brand, String countryCode, Guid generationID)
        {
            // Is ensuring a context can be retrieved by setting to the know global context necessary?
            MyContext.SetSystemContext("Toyota", "ZZ", "en");

            var country = MyContext.GetContext().Countries.Single(ctry => ctry.Code == countryCode);
            return country.Languages.ToDictionary(lang => lang.Code, lang =>
            {
                MyContext.SetSystemContext(brand, countryCode, lang.Code);
                var generation = ModelGeneration.GetModelGeneration(generationID);
                var model = Model.GetModel(generation.Model.ID);
                
                return Tuple.Create(generation, model);
            });
        }
    }
}
