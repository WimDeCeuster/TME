using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;

using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public partial class PackService : IPackService
    {
        private readonly IService _service;
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keyManager;

        public PackService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keyManager == null) throw new ArgumentNullException("keyManager");

            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task PutGradePacksAsync(string brand, string country, Guid publicationId, Guid timeFrameId, Guid gradeId, IEnumerable<GradePack> packs)
        {
            var key = _keyManager.GetGradePacksKey(publicationId, timeFrameId, gradeId);
            var serializedPacks = _serialiser.Serialise(packs);

            await _service.PutObjectAsync(brand, country, key, serializedPacks);
        }

        public async Task PutSubModelGradePacksAsync(string brand, string country, Guid publicationId, Guid timeFrameId, Guid subModelId, Guid gradeId, IEnumerable<GradePack> packs)
        {
            var key = _keyManager.GetSubModelGradePacksKey(publicationId, timeFrameId, gradeId, subModelId);
            var serializedPacks = _serialiser.Serialise(packs);

            await _service.PutObjectAsync(brand, country, key, serializedPacks);
        }

        public async Task PutCarPacksAsync(String brand, String country, Guid publicationID, Guid carID, IEnumerable<CarPack> carPacks)
        {
            if (carPacks == null) throw new ArgumentNullException("carPacks");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");

            var path = _keyManager.GetCarPacksKey(publicationID, carID);
            var value = _serialiser.Serialise(carPacks);

            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}