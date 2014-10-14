using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.QueryServices
{
    public interface IAssetService
    {
        IEnumerable<Asset> GetAssets(Guid publicationId, Guid objectId, Context context);
    }
}