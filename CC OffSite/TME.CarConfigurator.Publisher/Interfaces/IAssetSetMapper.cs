using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetSetMapper
    {
        IEnumerable<VisibleInModeAndView> GetVisibility(Administration.Assets.AssetSet assetSet);
    }
}
