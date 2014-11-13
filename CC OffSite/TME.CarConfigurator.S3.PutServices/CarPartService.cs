using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class CarPartService : ICarPartService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public CarPartService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task PutCarCarParts(String brand, String country, Guid publicationID, Guid carID, IEnumerable<CarPart> carParts)
        {
            if (carParts == null) throw new ArgumentNullException("carParts");
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");

            var path = _keyManager.GetCarPartsKey(publicationID, carID);
            var value = _serialiser.Serialise(carParts);

            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}