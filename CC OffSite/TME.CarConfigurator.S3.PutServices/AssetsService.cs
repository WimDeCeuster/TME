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

        public async Task PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid objectID, string mode, string view, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetAssetsKey(publicationID, objectID, view, mode);

            await PutAssets(brand, country, path, assets);
        }

        public async Task PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, Guid objectID, string mode, string view, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetAssetsKey(publicationID, carID, objectID, view, mode);

            await PutAssets(brand, country, path, assets);
        }

        public async Task PutDefaultAssets(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultAssetsKey(publicationID, objectID);
            await PutAssets(brand, country, path, assets);
        }

        public async Task PutDefaultAssets(string brand, string country, Guid publicationID, Guid carID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultAssetsKey(publicationID, carID, objectID);
            await PutAssets(brand, country, path, assets);
        }

        private async Task PutAssets(string brand, string country, string path, IEnumerable<Asset> assets)
        {
            var value = _serialiser.Serialise(assets);
            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}