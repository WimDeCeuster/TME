using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.Shared.Exceptions;
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

        public IEnumerable<Asset> GetSubModelAssets(Guid publicationID, Guid subModelID, Guid objectID, Context context)
        {
            var key = _keyManager.GetDefaultSubModelAssetsKey(publicationID, subModelID, objectID);

            return GetAssets(context, key);
        }

        public IEnumerable<Asset> GetSubModelAssets(Guid publicationID, Guid subModelID, Guid objectID, Context context, string view, string mode)
        {
            var key = _keyManager.GetSubModelAssetsKey(publicationID, subModelID, objectID, view, mode);

            return GetAssets(context, key);
        }

        public IEnumerable<Asset> GetCarAssets(Guid publicationId, Guid carId, Guid objectId, Context context, String view, string mode)
        {
            var key = _keyManager.GetCarAssetsKey(publicationId, carId, objectId, view, mode);

            return GetAssets(context, key);
        }


        public Dictionary<Guid, List<Asset>> GetCarPartsAssets(Guid publicationID, Guid carID, Context context, string view, string mode)
        {
            var key = _keyManager.GetCarPartAssetsKey(publicationID, carID, view, mode);
            return GetDictionaryAssets(context, key);
        }

        public Dictionary<Guid, List<Asset>> GetCarEquipmentAssets(Guid publicationID, Guid carID, Context context)
        {
            var key = _keyManager.GetDefaultCarEquipmentAssetsKey(publicationID, carID);
            return GetDictionaryAssets(context, key);
        }

        public Dictionary<Guid, List<Asset>> GetCarEquipmentAssets(Guid publicationID, Guid carID, Context context, string view, string mode)
        {
            var key = _keyManager.GetCarEquipmentAssetsKey(publicationID, carID, view, mode);
            return GetDictionaryAssets(context, key);
        }

        private IEnumerable<Asset> GetAssets(Context context, String key)
        {
            try { 
                var serializedObject = _service.GetObject(context.Brand, context.Country, key);
                return _serialiser.Deserialise<IEnumerable<Asset>>(serializedObject);
            }
            catch (ObjectNotFoundException ex)
            {
                if (!String.Equals(ex.Path, key, StringComparison.InvariantCultureIgnoreCase))
                    throw;

                return new Asset[] { };
            }
        }

        private Dictionary<Guid, List<Asset>> GetDictionaryAssets(Context context, String key)
        {
            try
            {
                var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
                return _serialiser.Deserialise<Dictionary<Guid, List<Asset>>>(serialisedObject);
            }
            catch(ObjectNotFoundException ex)
            {
                if (!String.Equals(ex.Path, key, StringComparison.InvariantCultureIgnoreCase))
                    throw;

                return new Dictionary<Guid,List<Asset>>();
            }
        }
    }
}