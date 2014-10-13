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
        private readonly IKeyManager _keyManager;
        private readonly IService _service;
        private readonly ISerialiser _serialiser;

        public AssetsService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> PutGenerationsAsset(string brand, string country,Guid publicationID, IEnumerable<Asset> assetvalue)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (publicationID == null) throw new ArgumentNullException("publicationID");
            if (assetvalue == null) throw new ArgumentNullException("assetvalue");

            var path = _keyManager.GetGenerationAssetKey(publicationID);
            var value = _serialiser.Serialise(assetvalue);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}