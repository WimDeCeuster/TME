using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IAssetFactory
    {
        IEnumerable<IAsset> GetAssets(Publication publication, Guid objectId, Context context);
    }
}