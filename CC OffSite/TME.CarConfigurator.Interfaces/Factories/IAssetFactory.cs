using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IAssetFactory
    {
        IReadOnlyList<IAsset> GetAssets(Publication publication, Guid objectId, Context context);
        IReadOnlyList<IAsset> GetAssets(Publication publication, Guid objectId, Context context, string view, string mode);
        IReadOnlyList<IAsset> GetCarAssets(Publication publication, Guid carId, Guid objectId, Context context);
        IReadOnlyList<IAsset> GetCarAssets(Publication publication, Guid carId, Guid objectId, Context context, string view, string mode);
        IReadOnlyList<IAsset> GetCarEquipmentAssets(Publication publication, Guid carID, Guid objectId, Context context);
        IReadOnlyList<IAsset> GetCarEquipmentAssets(Publication publication, Guid carID, Guid objectID, Context context, string view, string mode);
        IReadOnlyList<IAsset> GetCarPartAssets(Publication publication, Guid carID, Guid objectID, Context context, string view, string mode);
        IReadOnlyList<IAsset> GetSubModelAssets(Publication publication, Guid subModelID, Guid objectID, Context context);
        IReadOnlyList<IAsset> GetSubModelAssets(Publication publication, Guid subModelID, Guid objectID, Context context, string view, string mode);
    }
}