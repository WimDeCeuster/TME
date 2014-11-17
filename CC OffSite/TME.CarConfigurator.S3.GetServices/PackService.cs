using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class PackService : IPackService
    {
        private readonly ISerialiser _serializer;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public PackService(ISerialiser serializer, IService service, IKeyManager keyManager)
        {
            _serializer = serializer;
            _service = service;
            _keyManager = keyManager;
        }

        public IEnumerable<GradePack> GetGradePacks(Guid publicationId, Guid publicationTimeFrameId, Guid gradeId, Context context)
        {
            return GetGradePacks(context, _keyManager.GetGradePacksKey(publicationId, publicationTimeFrameId, gradeId));
        }

        public IEnumerable<GradePack> GetSubModelGradePacks(Guid publicationId, Guid publicationTimeFrameId, Guid gradeId, Guid subModelId, Context context)
        {
            return GetGradePacks(context, _keyManager.GetSubModelGradePacksKey(publicationId, publicationTimeFrameId, gradeId, subModelId));
        }

        private IEnumerable<GradePack> GetGradePacks(Context context, string key)
        {
            var serializedPacks = _service.GetObject(context.Brand, context.Country, key);

            return _serializer.Deserialise<IEnumerable<GradePack>>(serializedPacks);
        }

        public IEnumerable<CarPack> GetCarPacks(Guid publicationId, Guid carId, Context context)
        {
            var key = _keyManager.GetCarPacksKey(publicationId, carId);
            var serializedPacks = _service.GetObject(context.Brand, context.Country, key);

            return _serializer.Deserialise<IEnumerable<CarPack>>(serializedPacks);
        }
    }
}