using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;

using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class AssetsService : IAssetService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public AssetsService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public async Task PutDefaultAssets(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultAssetsKey(publicationID, objectID);
            await PutAssets(brand, country, path, assets);
        }

        public async Task PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid objectID, string mode, string view, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetAssetsKey(publicationID, objectID, view, mode);

            await PutAssets(brand, country, path, assets);
        }

        public async Task PutDefaultCarAssets(string brand, string country, Guid publicationID, Guid carID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultCarAssetsKey(publicationID, carID, objectID);
            await PutAssets(brand, country, path, assets);
        }

        public async Task PutCarAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, Guid objectID, string mode, string view, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetCarAssetsKey(publicationID, carID, objectID, view, mode);

            await PutAssets(brand, country, path, assets);
        }

        public async Task PutDefaultSubModelAssets(string brand, string country, Guid publicationID, Guid subModelID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultSubModelAssetsKey(publicationID, subModelID, objectID);
            await PutAssets(brand, country, path, assets);
        }

        public async Task PutSubModelAssetsByModeAndView(string brand, string country, Guid publicationID, Guid subModelID, Guid objectID, string mode, string view, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetSubModelAssetsKey(publicationID, subModelID, objectID, view, mode);

            await PutAssets(brand, country, path,assets);
        }

        private async Task PutAssets(string brand, string country, string path, IEnumerable<Asset> assets)
        {
            var value = _serialiser.Serialise(assets);
            await _service.PutObjectAsync(brand, country, path, value);
        }

        private async Task PutAssets(string brand, string country, string path, IDictionary<Guid, IList<Asset>> assets)
        {
            var value = _serialiser.Serialise(assets);
            await _service.PutObjectAsync(brand, country, path, value);
        }

        public async Task PutCarPartsAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, string mode, string view, Dictionary<Guid, IList<Asset>> carPartAssets)
        {
            var path = _keyManager.GetCarPartAssetsKey(publicationID, carID, view, mode);
            await PutAssets(brand, country, path, carPartAssets);
        }

        public async Task PutDefaultCarEquipmentAssets(string brand, string country, Guid publicationID, Guid carID, Dictionary<Guid, IList<Asset>> carEquipmentAssets)
        {
            var path = _keyManager.GetDefaultCarEquipmentAssetsKey(publicationID, carID);
            await PutAssets(brand, country, path, carEquipmentAssets);
        }

        public async Task PutCarEquipmentAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, string mode, string view, Dictionary<Guid, IList<Asset>> carEquipmentAssets)
        {
            var path = _keyManager.GetCarEquipmentAssetsKey(publicationID, carID, view, mode);
            await PutAssets(brand, country, path, carEquipmentAssets);
        }

    }
}