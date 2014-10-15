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
        private readonly ISerialiser _serializer;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public AssetsService(IService service, ISerialiser serializer, IKeyManager keyManager)
        {
            _serializer = serializer;
            _service = service;
            _keyManager = keyManager;
        }

        public async Task<Result> PutAssetsByModeAndView(string brand, string country, Guid publicationID, Guid objectID, string mode, string view,
            IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetAssetsKey(publicationID, objectID, view, mode);
            var value = _serializer.Serialise(assets);

            return await _service.PutObjectAsync(brand, country, path, value);
        }

        public async Task<Result> PutDefaultAssets(string brand, string country, Guid publicationID, Guid objectID, IEnumerable<Asset> assets)
        {
            var path = _keyManager.GetDefaultAssetsKey(publicationID, objectID);
            var value = _serializer.Serialise(assets);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}