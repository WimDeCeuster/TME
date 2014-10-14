using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.CommandServices
{
    public interface IAssetService
    {
        Task<Result> PutGenerationsAsset(String brand, String country,Guid publicationID,Guid objectID,Dictionary<Guid,IEnumerable<Asset>> assetsPerObject );
    }
}