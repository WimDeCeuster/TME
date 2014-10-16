using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class SteeringService : ISteeringService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public SteeringService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> PutTimeFrameGenerationSteerings(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Steering> steerings)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (steerings == null) throw new ArgumentNullException("steerings");

            var path = _keyManager.GetSteeringsKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(steerings);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}