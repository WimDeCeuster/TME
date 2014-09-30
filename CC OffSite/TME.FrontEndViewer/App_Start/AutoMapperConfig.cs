using System.Web.UI;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.App_Start
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<Model, ModelDTO>()
                .ForMember(dest => dest.CarConfiguratorVersion,
                           opts => opts.MapFrom(src => src.CarConfiguratorVersion));
        }
    }
}