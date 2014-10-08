using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Tests.Shared;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public abstract class MappingTestBase : TestBase
    {
        Mapper _mapper;
        const String Brand = "Toyota";
        protected String Country;
        protected String Language;
        protected PublicationDataSubset DataSubset;
        protected ICarDbModelGenerationFinder GenerationFinder = new CarDbModelGenerationFinder();
        protected IContext Context;
        protected ModelGeneration Generation;

        protected override void Arrange()
        {
            AutoMapperConfig.Configure();

            _mapper = new Mapper();

            var generationInfo = FindCompatibleGeneration();
            
            Generation = generationInfo.Generation;
            Country = generationInfo.Country;
            Language = generationInfo.Language;

            Context = new Context(Brand, Country, Generation.ID, DataSubset);

        }

        protected override void Act()
        {
            _mapper.Map(Brand, Country, Generation.ID, GenerationFinder, Context);
        }

        GenerationInfo FindCompatibleGeneration()
        {
            MyContext.SetSystemContext(Brand, "ZZ", "en");

            foreach (var country in MyContext.GetContext().Countries)
                foreach (var language in country.Languages)
                {
                    MyContext.SetSystemContext(Brand, country.Code, language.Code);
                    foreach (var model in Models.GetModels())
                        foreach (var generation in model.Generations)
                        {
                            if (Filter(model, generation))
                            {
                                return new GenerationInfo
                                {
                                    Country = country.Code,
                                    Language = language.Code,
                                    Generation = generation,
                                    Model = model
                                };
                            }
                        }
                }

            throw new Exception("No generations with links found.");
        }

        protected abstract Boolean Filter(Model model, ModelGeneration generation);

        protected class GenerationInfo
        {
            public String Country { get; set; }
            public String Language { get; set; }
            public ModelGeneration Generation { get; set; }
            public Model Model { get; set; }
        }
    }
}
