using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Publisher
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            MapAssets();


            AutoMapper.Mapper.CreateMap<BaseObject, Model>();
            AutoMapper.Mapper.CreateMap<Administration.Brand, String>().ConvertUsing(brand => brand.Name);
            AutoMapper.Mapper.CreateMap<Administration.Translations.Label, Label>()
                .ForMember(label => label.Code,
                           opt => opt.MapFrom(label => label.Definition.Code));
            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationCarConfiguratorVersion, CarConfiguratorVersion>();
            AutoMapper.Mapper.CreateMap<Administration.Model, Model>()
                .Translate(model => model.Name);
            AutoMapper.Mapper.CreateMap<Administration.ModelGeneration, Generation>()
                .ForMember(gen => gen.Links,
                    opt => opt.Ignore())
                .ForMember(gen => gen.Assets, 
                           opt => opt.MapFrom(modelGeneration => modelGeneration.Assets))
                .ForMember(generation => generation.SSN,
                           opt => opt.MapFrom(modelGeneration =>
                                              modelGeneration.FactoryGenerations.Select(factoryGeneration => factoryGeneration.SSN).First()))
                .Translate(modelGeneration => modelGeneration.Name);


            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationBodyType, BodyType>()
                .ForMember(bodyType => bodyType.VisibleIn,
                           opt => opt.MapFrom(bodyType => bodyType.GetVisibleInList()))
                .Translate(
                    bodyType => bodyType.Name);
            
            AutoMapper.Mapper.CreateMap<Administration.Car, Car>();

            AutoMapper.Mapper.CreateMap<Administration.EngineInfo, Engine>();
            AutoMapper.Mapper.CreateMap<Administration.EngineTypeInfo, EngineType>();
            AutoMapper.Mapper.CreateMap<Administration.FuelTypeInfo, FuelType>();
            //AutoMapper.Mapper.CreateMap<Administration.EngineCategory, EngineCategory>();
            AutoMapper.Mapper.CreateMap<Administration.BodyTypeInfo, BodyType>();
        }

        private static void MapAssets()
        {
            AutoMapper.Mapper.CreateMap<Administration.FileType, FileType>();
            AutoMapper.Mapper.CreateMap<Administration.Assets.AssetType, AssetType>()
                .ForMember(assetType => assetType.Mode,opt => opt.MapFrom(assetType => assetType.Details.Mode))
                .ForMember(assetType => assetType.Side,opt => opt.MapFrom(assetType => assetType.Details.Side))
                .ForMember(assetType => assetType.View,opt => opt.MapFrom(assetType => assetType.Details.View))
                .ForMember(assetType => assetType.Type,opt => opt.MapFrom(assetType => assetType.Details.Type));
            AutoMapper.Mapper.CreateMap<Administration.Assets.LinkedAsset, Asset>();
        }

        static AutoMapper.IMappingExpression<TSource, TDestination> Translate<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping, Func<TSource, String> name
            )
            where TSource : Administration.BaseObjects.TranslateableBusinessBase
            where TDestination : Repository.Objects.Core.BaseObject
        {
            Func<TSource, String> empty = source => "";

            mapping
                .ForMember(destination => destination.Name, opt => opt.MapFrom(source => source.Translation.Name.DefaultIfEmpty(name(source))))
                .ForMember(destination => destination.Description, opt => opt.MapFrom(source => source.Translation.Description))
                .ForMember(destination => destination.FootNote, opt => opt.MapFrom(source => source.Translation.FootNote))
                .ForMember(destination => destination.ToolTip, opt => opt.MapFrom(source => source.Translation.ToolTip))
                .ForMember(destination => destination.Labels, opt => opt.MapFrom(source => source.Translation.Labels));

            return mapping;
        }

        static String DefaultIfEmpty(this String str, String defaultStr)
        {
            return String.IsNullOrWhiteSpace(str) ? defaultStr : str; 
        }

        static List<VisibleInModeAndView> GetVisibleInList(this Administration.ModelGenerationBodyType bodyType)
        {
            return bodyType.AssetSet.Assets.Select(asset => Tuple.Create(asset.AssetType.Details.Mode, asset.AssetType.Details.View))
                                           .Distinct()
                                           .Select(info => new VisibleInModeAndView { Mode = info.Item1, View = info.Item2 })
                                           .ToList();
        }
    }
}