using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

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

        public async Task<Result> PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid objectID, string mode, string view, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetAssetsKey(publicationID, objectID, view, mode);

            return await PutAssets(brand, country, path, assets);
        }

        public async Task<Result> PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid carID, Guid objectID, string mode, string view, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetAssetsKey(publicationID, carID, objectID, view, mode);

            return await PutAssets(brand, country, path, assets);
        }

        public async Task<Result> PutDefaultAssets(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultAssetsKey(publicationID, objectID);
            return await PutAssets(brand, country, path, assets);
        }

        public async Task<Result> PutDefaultAssets(string brand, string country, Guid publicationID, Guid carID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultAssetsKey(publicationID, carID, objectID);
            return await PutAssets(brand, country, path, assets);
        }

        private async Task<Result> PutAssets(string brand, string country, string path, IEnumerable<Asset> assets)
        {
            var value = _serialiser.Serialise(assets);
            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}