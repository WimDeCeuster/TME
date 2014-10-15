using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IAssetMapper
    {
        Asset MapLinkedAsset(Administration.Assets.LinkedAsset linkedAsset);
        Asset MapAssetSetAsset(AssetSetAsset assetSetAsset,ModelGeneration modelGeneration);
    }
}
