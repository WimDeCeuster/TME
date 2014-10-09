using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using AutoMapper;

namespace TME.CarConfigurator.Publisher
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            ConfigureBase();
            ConfigureAssets();
            ConfigureModel();
            ConfigureGeneration();
            ConfigureBodyType();
            ConfigureEngine();
            ConfigureCar();
        }

        static void ConfigureBase()
        {
            AutoMapper.Mapper.CreateMap<Administration.Brand, String>().ConvertUsing(brand => brand.Name);
            AutoMapper.Mapper.CreateMap<Administration.Translations.Label, Label>()
                .ForMember(label => label.Code,
                           opt => opt.MapFrom(label => label.Definition.Code));
        }

        static void ConfigureModel()
        {
            AutoMapper.Mapper.CreateMap<Administration.Model, Model>()
                .Translate(model => model.Name);
        }

        static void ConfigureGeneration()
        {
            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationCarConfiguratorVersion, CarConfiguratorVersion>();

            AutoMapper.Mapper.CreateMap<Administration.ModelGeneration, Generation>()
                .ForMember(gen => gen.Links,
                    opt => opt.Ignore())
                .ForMember(gen => gen.Assets,
                           opt => opt.MapFrom(modelGeneration => modelGeneration.Assets))
                .ForMember(generation => generation.SSN,
                           opt => opt.MapFrom(modelGeneration =>
                                              modelGeneration.FactoryGenerations.Select(factoryGeneration => factoryGeneration.SSN).First()))
                .Translate(modelGeneration => modelGeneration.Name);
        }

        public class CrossModelObjectTypeConverter<TSource, TCrossModelType, TDestination> : ITypeConverter<Tuple<TSource, TCrossModelType>, TDestination>
        {
            public TDestination Convert(AutoMapper.ResolutionContext context)
            {
                var values = (Tuple<TSource, TCrossModelType>)context.SourceValue;
                var bodyType = AutoMapper.Mapper.Map(values.Item2, (TDestination)context.DestinationValue);
                return AutoMapper.Mapper.Map(values.Item1, bodyType);
            }
        }

        public class BodyTypeConverter : CrossModelObjectTypeConverter<Administration.ModelGenerationBodyType, Administration.BodyType, BodyType>
        { }

        static void ConfigureBodyType()
        {
            AutoMapper.Mapper.CreateMap<Tuple<Administration.ModelGenerationBodyType, Administration.BodyType>, BodyType>()
                .ConvertUsing<CrossModelObjectTypeConverter<Administration.ModelGenerationBodyType, Administration.BodyType, BodyType>>();

            AutoMapper.Mapper.CreateMap<Administration.BodyType, BodyType>()
                .ForMember(bodyType => bodyType.InternalCode,
                           opt => opt.MapFrom(bodyType => bodyType.BaseCode))
                .Localize(bodyType => bodyType.Name);

            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationBodyType, BodyType>()
                .ForMember(bodyType => bodyType.VisibleIn,
                           opt => opt.MapFrom(bodyType => bodyType.AssetSet.GetVisibleInList()))
                .Translate(bodyType => bodyType.Name)
                .Sort();
            
            AutoMapper.Mapper.CreateMap<Administration.BodyTypeInfo, BodyType>();
        }

        static void ConfigureEngine()
        {
            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationEngine, Engine>()
                .ForMember(engine => engine.VisibleIn,
                           opt => opt.MapFrom(engine => engine.AssetSet.GetVisibleInList()))
                .ForMember(engine => engine.Category,
                           opt => opt.Ignore())
                .Translate(engine => engine.Name);

            AutoMapper.Mapper.CreateMap<Administration.EngineTypeInfo, EngineType>();

            AutoMapper.Mapper.CreateMap<Administration.FuelTypeInfo, FuelType>()
                .ForMember(fuelType => fuelType.Hybrid, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<Administration.EngineInfo, Engine>();
            
            AutoMapper.Mapper.CreateMap<Administration.EngineCategory, EngineCategory>();   
        }

        static void ConfigureCar()
        {
            AutoMapper.Mapper.CreateMap<Administration.Car, Car>()
                .ForMember(car => car.Transmission, opt => opt.Ignore());
        }

        static void ConfigureAssets()
        {
            AutoMapper.Mapper.CreateMap<Administration.FileType, FileType>();
            AutoMapper.Mapper.CreateMap<Administration.Assets.AssetType, AssetType>()
                .ForMember(assetType => assetType.Mode,opt => opt.MapFrom(assetType => assetType.Details.Mode))
                .ForMember(assetType => assetType.Side,opt => opt.MapFrom(assetType => assetType.Details.Side))
                .ForMember(assetType => assetType.View,opt => opt.MapFrom(assetType => assetType.Details.View))
                .ForMember(assetType => assetType.Type,opt => opt.MapFrom(assetType => assetType.Details.Type));
            AutoMapper.Mapper.CreateMap<Administration.Assets.LinkedAsset, Asset>();
        }

        static AutoMapper.IMappingExpression<TSource, TDestination> Localize<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping, Func<TSource, String> name
            )
            where TSource : Administration.BaseObjects.LocalizeableBusinessBase
            where TDestination : Repository.Objects.Core.BaseObject
        {
            return mapping
                .Translate(name)
                .ForMember(destination => destination.InternalCode, opt => opt.MapFrom(source => source.BaseCode));
        }

        static AutoMapper.IMappingExpression<TSource, TDestination> Translate<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping, Func<TSource, String> name
            )
            where TSource : Administration.BaseObjects.TranslateableBusinessBase
            where TDestination : Repository.Objects.Core.BaseObject
        {
            Func<TSource, String> empty = source => "";

            return mapping
                .ForMember(destination => destination.Name, opt => opt.MapFrom(source => source.Translation.Name.DefaultIfEmpty(name(source))))
                .ForMember(destination => destination.Description, opt => opt.MapFrom(source => source.Translation.Description))
                .ForMember(destination => destination.FootNote, opt => opt.MapFrom(source => source.Translation.FootNote))
                .ForMember(destination => destination.ToolTip, opt => opt.MapFrom(source => source.Translation.ToolTip))
                .ForMember(destination => destination.Labels, opt => opt.MapFrom(source => source.Translation.Labels));
        }

        static AutoMapper.IMappingExpression<TSource, TDestination> Sort<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping
            )
            where TSource : Administration.BaseObjects.ISortedIndex
            where TDestination : Repository.Objects.Core.BaseObject
        {
            return mapping
                .ForMember(destination => destination.SortIndex, opt => opt.MapFrom(source => source.Index));
        }

        static String DefaultIfEmpty(this String str, String defaultStr)
        {
            return String.IsNullOrWhiteSpace(str) ? defaultStr : str; 
        }

        static List<VisibleInModeAndView> GetVisibleInList(this Administration.Assets.AssetSet assetSet)
        {
            return assetSet.Assets.Select(asset => Tuple.Create(asset.AssetType.Details.Mode, asset.AssetType.Details.View))
                                  .Distinct()
                                  .Select(info => new VisibleInModeAndView { Mode = info.Item1, View = info.Item2 })
                                  .ToList();
        }
    }
}