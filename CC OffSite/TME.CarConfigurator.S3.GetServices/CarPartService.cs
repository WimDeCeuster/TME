using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class CarPartService : ICarPartService
    {
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keyManager;
        private readonly IService _service;

        public CarPartService(ISerialiser serialiser, IKeyManager keyManager, IService service)
        {
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keyManager == null) throw new ArgumentNullException("keyManager");
            if (service == null) throw new ArgumentNullException("service");

            _serialiser = serialiser;
            _keyManager = keyManager;
            _service = service;
        }

        public IEnumerable<CarPart> GetCarParts(Guid publicationID, Guid carID, Context context)
        {
            var key = _keyManager.GetCarPartsKey(publicationID, carID);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<IEnumerable<CarPart>>(serialisedObject);
        }
    }
}