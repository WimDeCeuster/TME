using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var model = Models.GetModels().Single(mdl => mdl.Generations.Count(gen => gen.ID == generationID) == 1);
                var generation = model.Generations.Single(gen => gen.ID == generationID);
                return Tuple.Create(generation, model);
            });
        }
    }
}
