using System;
using System.Collections.Generic;

namespace TME.CarConfigurator.QueryServices
{
    public interface IAssetService
    {
        IEnumerable<Repository.Objects.Assets.Asset> GetAssets(Guid objectId);
    }
}