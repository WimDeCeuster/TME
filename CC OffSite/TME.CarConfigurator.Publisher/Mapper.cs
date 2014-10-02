using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using ModelGeneration = TME.CarConfigurator.Administration.ModelGeneration;

namespace TME.CarConfigurator.Publisher
{
    public interface IMapper
    {
        IContextData Map(ModelGeneration modelGeneration, IContextData contextData);
    }

    public class Mapper : IMapper
    {
        public IContextData Map(ModelGeneration modelGeneration, IContextData contextData)
        {
            // fill contextData
            
            contextData.Generations.Add(AutoMapper.Mapper.Map<Generation>(modelGeneration));

            return contextData;
        }
    }

    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            AutoMapper.Mapper.CreateMap<TME.CarConfigurator.Administration.ModelGenerationCarConfiguratorVersion, CarConfiguratorVersion>();

            AutoMapper.Mapper.CreateMap<ModelGeneration, Generation>()
                             .ForMember(generation => generation.Links,
                                        opt => opt.Ignore())
                             .ForMember(generation => generation.Assets,
                                        opt => opt.Ignore())
                             .ForMember(generation => generation.SSNs,
                                        opt => opt.MapFrom(modelGeneration =>
                                            modelGeneration.FactoryGenerations.Select(factoryGeneration => factoryGeneration.SSN).ToList()));
        }
    }
}
