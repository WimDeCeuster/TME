using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class AssetsService : IAssetService
    {
        private readonly IKeyManager _keyManager;
        private readonly IService _service;
        private readonly ISerialiser _serialiser;

        public AssetsService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> PutDefaultBodyTypeAssets(string brand, string country, Guid publicationID, Guid objectID,
            Dictionary<Guid, IList<Asset>> assetsPerObject)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (publicationID == null) throw new ArgumentNullException("publicationID");
            if (assetsPerObject == null) throw new ArgumentNullException("assetsPerObject");

            var path = _keyManager.GetDefaultAssetsKey(publicationID, objectID);
            var value = _serialiser.Serialise(assetsPerObject);

            return await _service.PutObjectAsync(brand, country, path, value);
        }

        public async Task<Result> PutGenerationsAsset(string brand, string country,Guid publicationID,Guid objectID,Dictionary<Guid,IList<Asset>> assetPerObject)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (publicationID == null) throw new ArgumentNullException("publicationID");
            if (assetPerObject == null) throw new ArgumentNullException("assetPerObject");

            var path = _keyManager.GetDefaultAssetsKey(publicationID, objectID);
            var value = _serialiser.Serialise(assetPerObject);

            return await _service.PutObjectAsync(brand, country, path, value);
        }

        public async Task<Result> PutModeViewBodyTypeAssets(string brand, string country, Guid publicationID, Guid objectID,
            Dictionary<Guid, IList<Asset>> assetsPerObject)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (publicationID == null) throw new ArgumentNullException("publicationID");
            if (assetsPerObject == null) throw new ArgumentNullException("assetsPerObject");

            var path = _keyManager.GetAssetsKey(publicationID, objectID,
                assetsPerObject.First().Value.First().AssetType.View,
                assetsPerObject.First().Value.First().AssetType.Mode);
            var value = _serialiser.Serialise(assetsPerObject);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}