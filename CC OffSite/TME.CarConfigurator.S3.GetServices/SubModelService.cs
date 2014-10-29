using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class SubModelService : ISubModelService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public SubModelService(ISerialiser serialiser,IService service,IKeyManager keyManager)
        {
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (service == null) throw new ArgumentNullException("service");
            if (keyManager == null) throw new ArgumentNullException("keyManager");
           
            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public IEnumerable<SubModel> GetSubModels(Guid publicationId, Guid publicationTimeFrameId, Context context)
        {
            var key = _keyManager.GetSubModelsKey(publicationId, publicationTimeFrameId);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            var deserialisedstuff =  _serialiser.Deserialise<IEnumerable<SubModel>>(serialisedObject);
            return deserialisedstuff;
        }
    }
}