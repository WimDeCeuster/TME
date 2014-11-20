using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Extensions
{
    internal static class VisibleInModeAndViewExtensions
    {
        internal static bool VisibleInExteriorSpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x=> (x.Mode == "DAY" || !x.HasMode) && x.View == "EXT");
        }
        internal static bool VisibleInInteriorSpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => (x.Mode == "DAY" || !x.HasMode) && x.View == "INT");
        }
        internal static bool VisibleInXRay4X4Spin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => x.Mode == "XRAY-4X4" && x.View == "EXT");
        }
        internal static bool VisibleInXRayHybridSpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => x.Mode == "XRAY-HYBRID" && x.View == "EXT");
        }
        internal static bool VisibleInXRaySafetySpin(this IEnumerable<IVisibleInModeAndView> visibeVisibleInModeAndViews)
        {
            return visibeVisibleInModeAndViews.Any(x => x.Mode == "XRAY-SAFETY" && x.View == "EXT");
        }
        
    }
}
