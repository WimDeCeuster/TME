using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.QueryServices
{
    public interface IAssetService
    {
        IEnumerable<Asset> GetAssets(Guid publicationId, Guid objectId, Context context);
        IEnumerable<Asset> GetAssets(Guid publicationId, Guid objectId, Context context, string view, string mode);
        IEnumerable<Asset> GetCarAssets(Guid publicationId, Guid carId, Guid objectId, Context context, string view, string mode);
        IEnumerable<Asset> GetCarAssets(Guid publicationId, Guid carId, Guid objectId, Context context);
        IEnumerable<Asset> GetSubModelAssets(Guid publicationID, Guid subModelID, Guid objectID, Context context);
        IEnumerable<Asset> GetSubModelAssets(Guid publicationID, Guid subModelID, Guid objectID, Context context, string view, string mode);

        Dictionary<Guid, List<Asset>> GetCarPartsAssets(Guid publicationID, Guid carID, Context context, string view, string mode);
        Dictionary<Guid, List<Asset>> GetCarEquipmentAssets(Guid publicationID, Guid carID, Context context);
        Dictionary<Guid, List<Asset>> GetCarEquipmentAssets(Guid publicationID, Guid carID, Context context, string view, string mode);
    }
}