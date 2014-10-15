using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;
using FluentAssertions;

namespace TME.Carconfigurator.Tests.GivenAMapper
{
    public class WhenMappingLinks : TestBase
    {
        Mapper _mapper;
        String _brand;
        String _country;
        String _language;
        ICarDbModelGenerationFinder _generationFinder;
        IContext _context;
        Guid _generationID;
        private int _linkCount;
        
        protected override void Arrange()
        {
            AutoMapperConfig.Configure();

            _brand = "Toyota";

            var generationInfo = FindCompatibleGeneration();
            _generationID = generationInfo.GenerationID;
            _country = generationInfo.Country;
            _language = generationInfo.Language;

            _linkCount = generationInfo.Links.Count;
            
            _context = new Context(_brand, _country, _generationID, PublicationDataSubset.Live);

            _generationFinder = new CarDbModelGenerationFinder();
            _mapper = new Mapper();
        }

        protected override void Act()
        {
            _mapper.Map(_brand, _country, _generationID, _generationFinder, _context);
        }

        [Fact]
        public void ThereShouldBeLinks()
        {
            _context.ContextData[_language].Generations.Single().Links.Count.ShouldBeEquivalentTo(_linkCount);
        }

        GenerationInfo FindCompatibleGeneration()
        {
            MyContext.SetSystemContext(_brand, "ZZ", "en");
            
            foreach (var country in MyContext.GetContext().Countries)
                foreach (var language in country.Languages)
                {
                    MyContext.SetSystemContext(_brand, country.Code, language.Code);
                    foreach (var model in Models.GetModels())
                        foreach (var generation in model.Generations) {
                            var links = model.Links.Where(link => link.Type.CarConfiguratorversionID == generation.ActiveCarConfiguratorVersion.ID ||
                                                                  link.Type.CarConfiguratorversionID == 0)
                                                   .ToList();
                            if (links.Any()) {
                                return new GenerationInfo
                                {
                                    Country = country.Code,
                                    Language = language.Code,
                                    GenerationID = generation.ID,
                                    Model = model,
                                    Links = links
                                };
                            }
                        }
                }
            
            throw new Exception("No generations with links found.");
        }

        class GenerationInfo
        {
            public String Country { get; set; }
            public String Language { get; set; }
            public Guid GenerationID { get; set; }
            public Model Model { get; set; }
            public List<Link> Links { get; set; }
        }
    }
}
