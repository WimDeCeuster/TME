﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;

using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class CarService : ICarService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public CarService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task PutTimeFrameGenerationCars(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Car> cars)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (cars == null) throw new ArgumentNullException("cars");

            var path = _keyManager.GetCarsKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(cars);

            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}
