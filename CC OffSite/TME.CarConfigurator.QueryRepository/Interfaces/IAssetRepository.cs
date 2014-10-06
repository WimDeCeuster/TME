using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface IAssetRepository
    {
        IEnumerable<Asset> GetAssetsForObject(Repository.Objects.Context.Publication context, Guid objectID);
        IEnumerable<Asset> GetAssetsForObjectAndCar(Repository.Objects.Context.Publication context, Guid objectID);
    }
}
