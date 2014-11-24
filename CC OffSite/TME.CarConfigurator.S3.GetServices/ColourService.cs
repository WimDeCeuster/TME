using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class ColourService : IColourService
    {
        private readonly ISerialiser _serializer;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public ColourService(ISerialiser serializer, IService service, IKeyManager keyManager)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (service == null) throw new ArgumentNullException("service");
            if (keyManager == null) throw new ArgumentNullException("keyManager");

            _serializer = serializer;
            _service = service;
            _keyManager = keyManager;
        }

        public IEnumerable<ColourCombination> GetColourCombinations(Guid publicationId, Guid publicationTimeFrameId, Context context)
        {
            var key = _keyManager.GetColourCombinationsKey(publicationId, publicationTimeFrameId);
            var serializedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serializer.Deserialise<IEnumerable<ColourCombination>>(serializedObject);
        }

        public IEnumerable<CarColourCombination> GetCarColourCombinations(Guid publicationID, Context context, Guid carID)
        {
            var key = _keyManager.GetCarColourCombinationsKey(publicationID, carID);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serializer.Deserialise<IEnumerable<CarColourCombination>>(serialisedObject);
        }
    }
}
