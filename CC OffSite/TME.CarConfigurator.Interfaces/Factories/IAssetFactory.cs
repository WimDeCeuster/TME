using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IAssetFactory
    {
        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IAsset> GetAssets(Publication publication, Guid objectId, Context context);
        IReadOnlyList<IAsset> GetAssets(Publication publication, Guid objectId, Context context, string view, string mode);
        IReadOnlyList<IAsset> GetCarAssets(Publication publication, Guid carId, Guid objectId, Context context);
        IReadOnlyList<IAsset> GetCarAssets(Publication publication, Guid carId, Guid objectId, Context context, string view, string mode);
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}