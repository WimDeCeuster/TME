using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.CommandServices
{
    public interface IAssetService
    {
        Task PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);
        Task PutCarAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);
        Task PutSubModelAssetsByModeAndView(string brand, string country, Guid publicationID, Guid timeFrameID, Guid subModelID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);
        Task PutDefaultAssets(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets);
        Task PutCarDefaultAssets(string brand, string country, Guid publicationID, Guid carID, Guid objectID, IEnumerable<Asset> assets);
        Task PutSubModelDefaultAssets(string brand, string country, Guid publicationID, Guid timeFrameID, Guid subModelID, Guid objectID, IEnumerable<Asset> assets);
    }
}