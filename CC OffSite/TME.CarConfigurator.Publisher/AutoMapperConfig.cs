using AutoMapper;
using System;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects;
using CarConfiguratorVersion = TME.CarConfigurator.Repository.Objects.CarConfiguratorVersion;

namespace TME.CarConfigurator.Publisher
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            AutoMapper.Mapper.CreateMap<Brand, String>().ConvertUsing(brand => brand.Name);

            AutoMapper.Mapper.CreateMap<Administration.Model, Repository.Objects.Model>();

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