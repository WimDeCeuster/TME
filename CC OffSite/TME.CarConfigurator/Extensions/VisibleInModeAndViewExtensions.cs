using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Extensions
{
    internal static class VisibleInModeAndViewExtensions
    {
        public static bool VisibleInExteriorSpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x=> x.Mode == "DAY" && x.View == "EXT");
        }
        public static bool VisibleInInteriorSpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => x.Mode == "DAY" && x.View == "INT");
        }
        public static bool VisibleInXRay4X4Spin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => x.Mode == "XRAY-4X4" && x.View == "EXT");
        }
        public static bool VisibleInXRayHybridSpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => x.Mode == "XRAY-HYBRID" && x.View == "EXT");
        }
        public static bool VisibleInXRaySafetySpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => x.Mode == "XRAY-SAFETY" && x.View == "EXT");
        }
        
    }
}
