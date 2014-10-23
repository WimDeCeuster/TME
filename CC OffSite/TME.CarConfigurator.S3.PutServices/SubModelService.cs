using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class SubModelService : ISubModelService
    {
        private readonly IService _service;
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keyManager;

        public SubModelService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keyManager == null) throw new ArgumentNullException("keyManager");

            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> PutTimeFrameGenerationSubModelsAsync(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<SubModel> subModels)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (subModels == null) throw new ArgumentNullException("subModels");

            var path = _keyManager.GetSubModelsKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(subModels);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}