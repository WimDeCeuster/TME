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
            throw new NotImplementedException();
        }
    }
}