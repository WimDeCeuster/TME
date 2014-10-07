using AutoMapper;
using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using CarConfiguratorVersion = TME.CarConfigurator.Repository.Objects.CarConfiguratorVersion;

namespace TME.CarConfigurator.Publisher
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            AutoMapper.Mapper.CreateMap<Administration.Brand, String>().ConvertUsing(brand => brand.Name);

            AutoMapper.Mapper.CreateMap<Administration.Model, Repository.Objects.Model>();

            AutoMapper.Mapper.CreateMap<TME.CarConfigurator.Administration.ModelGenerationCarConfiguratorVersion, CarConfiguratorVersion>();

            AutoMapper.Mapper.CreateMap<Administration.ModelGeneration, Generation>()
                .ForMember(generation => generation.Links,
                    opt => opt.Ignore())
                .ForMember(generation => generation.Assets,
                    opt => opt.Ignore())
                .ForMember(generation => generation.SSN,
                    opt => opt.MapFrom(modelGeneration =>
                        modelGeneration.FactoryGenerations.Select(factoryGeneration => factoryGeneration.SSN).First()));

            AutoMapper.Mapper.CreateMap<Administration.Car, Car>();

            AutoMapper.Mapper.CreateMap<Administration.EngineInfo, Engine>();
            AutoMapper.Mapper.CreateMap<Administration.EngineTypeInfo, EngineType>();
            AutoMapper.Mapper.CreateMap<Administration.FuelTypeInfo, FuelType>();
            //AutoMapper.Mapper.CreateMap<Administration.EngineCategory, EngineCategory>();
            AutoMapper.Mapper.CreateMap<Administration.BodyTypeInfo, BodyType>();
        }
    }
}