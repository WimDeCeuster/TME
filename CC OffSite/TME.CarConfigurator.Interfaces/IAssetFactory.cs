using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Interfaces
{
    public interface IAssetFactory
    {
        IEnumerable<IAsset> GetAssets(Guid objectId);
    }
}