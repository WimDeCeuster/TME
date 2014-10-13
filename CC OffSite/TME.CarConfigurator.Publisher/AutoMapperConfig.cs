using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;
using AutoMapper;
using System.Linq.Expressions;

namespace TME.CarConfigurator.Publisher
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            ConfigureBase();

            ConfigureBodyType();
            ConfigureEngine();
            ConfigureAssets();
            ConfigureModel();
            ConfigureGeneration();
            ConfigureCar();

            //AutoMapper.Mapper.AssertConfigurationIsValid();
            AutoMapper.Mapper.Configuration.Seal();
        }

        public static void ConfigureBase()
        {
            AutoMapper.Mapper.CreateMap<Administration.Brand, String>().ConvertUsing(brand => brand.Name);
            AutoMapper.Mapper.CreateMap<Administration.Translations.Label, Label>()

                .ForMember(label => label.Code,
                           opt => opt.MapFrom(label => label.Definition.Code));
        }

        public static void ConfigureModel()
        {
            AutoMapper.Mapper.CreateMap<Administration.Model, Model>()
                .Ignore(model => model.Publications)
                .Localize(model => model.Name)
                .Sort();
        }

        public static void ConfigureGeneration()
        {
            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationCarConfiguratorVersion, CarConfiguratorVersion>();

            AutoMapper.Mapper.CreateMap<Administration.ModelGeneration, Generation>()
                .For(gen => gen.CarConfiguratorVersion, gen => gen.ActiveCarConfiguratorVersion)
                .Ignore(gen => gen.SortIndex)
                .Ignore(gen => gen.Links)
                .ForMember(gen => gen.Assets,
                           opt => opt.MapFrom(modelGeneration => modelGeneration.Assets))
                .ForMember(generation => generation.SSN,
                           opt => opt.MapFrom(modelGeneration =>
                                              modelGeneration.FactoryGenerations.First().SSN))
                .Localize(modelGeneration => modelGeneration.Name);
        }

        public static void ConfigureBodyType()
        {
            AutoMapper.Mapper.CreateMap<Administration.BodyType, BodyType>()
                .Localize(bodyType => bodyType.Name)
                .Ignore(bodyType => bodyType.VisibleIn)
                .Ignore(bodyType => bodyType.NumberOfSeats)
                .Ignore(bodyType => bodyType.NumberOfDoors)
                .Ignore(bodyType => bodyType.SortIndex);

            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationBodyType, BodyType>()
                .ForMember(bodyType => bodyType.VisibleIn,
                           opt => opt.MapFrom(bodyType => bodyType.AssetSet.GetVisibleInList()))
                .IgnoreLocalized()
                .Translate(bodyType => bodyType.Name)
                .Sort();
        }

        public static void ConfigureEngine()
        {
            AutoMapper.Mapper.CreateMap<Administration.Engine, Engine>()
                .Ignore(engine => engine.KeyFeature)
                .Ignore(engine => engine.Brochure)
                .Ignore(engine => engine.SortIndex)
                .Ignore(engine => engine.VisibleIn)
                .Ignore(engine => engine.Type)
                .Ignore(engine => engine.Category)
                .Localize(engine => engine.Name);

            AutoMapper.Mapper.CreateMap<Administration.ModelGenerationEngine, Engine>()
                .ForMember(engine => engine.VisibleIn,
                            opt => opt.MapFrom(engine => engine.AssetSet.GetVisibleInList()))
                .Ignore(engine => engine.Category)
                .IgnoreLocalized()
                .Translate(engine => engine.Name)
                .Sort();

            AutoMapper.Mapper.CreateMap<Administration.EngineCategory, EngineCategory>()
                .Translate(engine => engine.Name)
                .For(engine => engine.InternalCode, engine => engine.Code)
                .ForMember(engine => engine.LocalCode, opt => opt.UseValue(String.Empty))
                .Sort();
            
            AutoMapper.Mapper.CreateMap<Administration.EngineTypeInfo, EngineType>()
                .Ignore(engineType => engineType.FuelType);

            AutoMapper.Mapper.CreateMap<Administration.FuelType, FuelType>()
                .Localize(fuelType => fuelType.Name)
                .For(fuelType => fuelType.Hybrid, fuelType => fuelType.Code.ToUpper(System.Globalization.CultureInfo.InvariantCulture).StartsWith("H"))
                .ForMember(fuelType => fuelType.SortIndex, opt => opt.UseValue(0));
        }

        public static void ConfigureCar()
        {
            AutoMapper.Mapper.CreateMap<Administration.Car, Car>()
                .ForMember(car => car.Engine, opt => opt.Ignore())
                .ForMember(car => car.BodyType, opt => opt.Ignore())
                .ForMember(car => car.Transmission, opt => opt.Ignore());
        }

        public static void ConfigureAssets()
        {
            AutoMapper.Mapper.CreateMap<Administration.Assets.Enums.AssetScope, Repository.Objects.Assets.Enums.Scope>().ConvertUsing(source => {
                switch(source)
                {
                    case Administration.Assets.Enums.AssetScope.Internal:
                        return Repository.Objects.Assets.Enums.Scope.Internal;
                    case Administration.Assets.Enums.AssetScope.Public:
                        return Repository.Objects.Assets.Enums.Scope.Public;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            AutoMapper.Mapper.CreateMap<Administration.FileType, FileType>();

            AutoMapper.Mapper.CreateMap<Administration.Assets.AssetType, AssetType>()
                .ForMember(assetType => assetType.Scope, opt => opt.MapFrom(assetType => assetType.Group.Scope))
                .ForMember(assetType => assetType.Mode, opt => opt.MapFrom(assetType => assetType.Details.Mode))
                .ForMember(assetType => assetType.Side, opt => opt.MapFrom(assetType => assetType.Details.Side))
                .ForMember(assetType => assetType.View,opt => opt.MapFrom(assetType => assetType.Details.View))
                .ForMember(assetType => assetType.Type,opt => opt.MapFrom(assetType => assetType.Details.Type));

            AutoMapper.Mapper.CreateMap<Administration.Assets.DetailedAssetInfo, Asset>()
                .Ignore(asset => asset.ShortID);

            AutoMapper.Mapper.CreateMap<Administration.Assets.LinkedAsset, Asset>()
                .Ignore(asset => asset.Height)
                .Ignore(asset => asset.Width)
                .Ignore(asset => asset.PositionX)
                .Ignore(asset => asset.PositionY);
        }

        static AutoMapper.IMappingExpression<TSource, TDestination> Localize<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping, Func<TSource, String> name
            )
            where TSource : Administration.BaseObjects.LocalizeableBusinessBase
            where TDestination : Repository.Objects.Core.BaseObject
        {
            return mapping
                .Translate(name)
                .ForMember(destination => destination.InternalCode, opt => opt.MapFrom(source => source.BaseCode))
                .ForMember(destination => destination.LocalCode, opt => opt.MapFrom(source => String.IsNullOrWhiteSpace(source.LocalCode) ? source.BaseCode : source.LocalCode));
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

        static AutoMapper.IMappingExpression<TSource, TDestination> IgnoreLocalized<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping)
            where TDestination : Repository.Objects.Core.BaseObject
        {
            return mapping
                .Ignore(source => source.LocalCode)
                .Ignore(source => source.InternalCode);
        }

        static AutoMapper.IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, Object>> property)
        {
            return mapping.ForMember(property, opt => opt.Ignore());
        }

        static AutoMapper.IMappingExpression<TSource, TDestination> For<TSource, TDestination>(
            this AutoMapper.IMappingExpression<TSource, TDestination> mapping,
            Expression<Func<TDestination, Object>> destinationProperty, Expression<Func<TSource, Object>> sourceProperty)
        {
            return mapping.ForMember(destinationProperty, opt => opt.MapFrom(sourceProperty));
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
                                  .Where(info => !(String.IsNullOrWhiteSpace(info.Mode) && String.IsNullOrWhiteSpace(info.View)))
                                  .ToList();
        }
    }
}