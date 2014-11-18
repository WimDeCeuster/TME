using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.CommandServices
{
    public interface IAssetService
    {
        Task PutDefaultAssets(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets);
        Task PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);
        Task PutDefaultCarAssets(string brand, string country, Guid publicationID, Guid carID, Guid objectID, IEnumerable<Asset> assets);
        Task PutCarAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);
        Task PutDefaultSubModelAssets(string brand, string country, Guid publicationID, Guid subModelID, Guid objectID, IEnumerable<Asset> assets);
        Task PutSubModelAssetsByModeAndView(string brand, string country, Guid publicationID, Guid subModelID, Guid objectID, string mode, string view, IEnumerable<Asset> assets);

        Task PutCarPartsAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, string mode, string view, Dictionary<Guid, IList<Asset>> carPartAssets);
        Task PutCarEquipmentAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, string mode, string view, Dictionary<Guid, IList<Asset>> carEquipmentAssets);
        Task PutDefaultCarEquipmentAssets(string brand, string country, Guid publicationID, Guid carID, Dictionary<Guid, IList<Asset>> carEquipmentAssets);
    }
}