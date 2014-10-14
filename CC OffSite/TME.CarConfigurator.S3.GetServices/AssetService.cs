using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class AssetService : IAssetService
    {
        public IEnumerable<Asset> GetAssets(Guid publicationId, Guid publicationTimeFrameId, Guid objectId, Context context)
        {
            throw new NotImplementedException();
        }
    }
}