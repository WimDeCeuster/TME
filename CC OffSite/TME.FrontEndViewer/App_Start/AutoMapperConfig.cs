using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.App_Start
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            //Configure Implementations
            CreateMap<CarConfiguratorVersionInfo,CarConfigurator.CarConfiguratorVersion,TME.CarConfigurator.Interfaces.ICarConfiguratorVersion>();
            CreateMap<Link,CarConfigurator.Link,CarConfigurator.Interfaces.ILink>();
            CreateMap<Asset,CarConfigurator.Asset,CarConfigurator.Interfaces.Assets.IAsset>();
            CreateMap<FileType,CarConfigurator.FileType,CarConfigurator.Interfaces.Assets.IFileType>();
            CreateMap<AssetType,CarConfigurator.AssetType,CarConfigurator.Interfaces.Assets.IAssetType>();
            CreateMap<BodyType,CarConfigurator.BodyType,CarConfigurator.Interfaces.IBodyType>();
            CreateMap<FuelType,CarConfigurator.FuelType,CarConfigurator.Interfaces.IFuelType>();
            CreateMap<Car,CarConfigurator.Car,CarConfigurator.Interfaces.ICar>();
            CreateMap<Engine,CarConfigurator.Engine,CarConfigurator.Interfaces.IEngine>();
            CreateMap<EngineCategory,CarConfigurator.EngineCategory,CarConfigurator.Interfaces.IEngineCategory>();
            CreateMap<FuelType,CarConfigurator.EngineType,CarConfigurator.Interfaces.IEngineType>();

            AutoMapper.Mapper.CreateMap<Model, ModelDTO>()
                .ForMember(dest => dest.CarConfiguratorVersion,
                           opts => opts.MapFrom(src => src.CarConfiguratorVersion));
        }

        private static void CreateMap<TSource, TDestination, TInterface>()
        {
            AutoMapper.Mapper.CreateMap<TSource,TDestination>();
            AutoMapper.Mapper.CreateMap<TSource, TInterface>().As<TDestination>();
        }
    }
}