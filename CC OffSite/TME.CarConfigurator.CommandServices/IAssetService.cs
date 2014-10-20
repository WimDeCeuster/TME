using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.CommandServices
{
    public interface IAssetService
    {
        Task<Result> PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);
        Task<Result> PutDefaultAssets(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> defaultAssets);
    }
}