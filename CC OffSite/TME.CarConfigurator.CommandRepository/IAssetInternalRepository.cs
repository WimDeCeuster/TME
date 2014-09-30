using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.CommandRepository
{
    public interface IAssetInternalRepository
    {
        Result.Result CreateAssetsOfObject(Repository.Objects.Context.Publication context, Guid objectID, IEnumerable<Asset> assets);
        Result.Result CreateAssetsOfObjectForCar(Repository.Objects.Context.Publication context, Guid objectID, Guid carID, IEnumerable<Asset> assets);
    }
}
