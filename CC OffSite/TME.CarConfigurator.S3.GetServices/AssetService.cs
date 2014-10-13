using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class AssetService : IAssetService
    {
        public IEnumerable<Repository.Objects.Assets.Asset> GetAssets(Guid objectId)
        {
            throw new NotImplementedException();
        }
    }
}