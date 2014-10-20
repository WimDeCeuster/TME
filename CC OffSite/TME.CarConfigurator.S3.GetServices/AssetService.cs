using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class AssetService : IAssetService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public AssetService(ISerialiser serialiser, IService service, IKeyManager keyManager)
        {
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (service == null) throw new ArgumentNullException("service");
            if (keyManager == null) throw new ArgumentNullException("keyManager");

            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public IEnumerable<Asset> GetAssets(Guid publicationId, Guid objectId, Context context)
        {
            var key = _keyManager.GetDefaultAssetsKey(publicationId, objectId);

            return GetAssets(context, key);
        }

        public IEnumerable<Asset> GetAssets(Guid publicationId, Guid objectId, Context context, string view, string mode)
        {
            var key = _keyManager.GetAssetsKey(publicationId, objectId, view, mode);

            return GetAssets(context, key);
        }

        public IEnumerable<Asset> GetCarAssets(Guid publicationId, Guid carId, Guid objectId, Context context)
        {
            var key = _keyManager.GetDefaultCarAssetsKey(publicationId, carId, objectId);

            return GetAssets(context, key);
        }

        public IEnumerable<Asset> GetCarAssets(Guid publicationId, Guid carId, Guid objectId, Context context, string view, string mode)
        {
            var key = _keyManager.GetCarAssetsKey(publicationId, carId, objectId, view, mode);

            return GetAssets(context, key);
        }

        private IEnumerable<Asset> GetAssets(Context context, string key)
        {
            var serializedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<IEnumerable<Asset>>(serializedObject);
        }
    }
}