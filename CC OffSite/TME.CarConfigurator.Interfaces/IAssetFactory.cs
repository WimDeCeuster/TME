using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Interfaces
{
    public interface IAssetFactory
    {
        IEnumerable<IAsset> CreateAssets(IEnumerable<Asset> repoAssets);
    }
}