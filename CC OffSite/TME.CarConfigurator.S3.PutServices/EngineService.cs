using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class EngineService : IEngineService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public EngineService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }        

        public async Task<Result> PutTimeFrameGenerationEngines(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Engine> engines)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (engines == null) throw new ArgumentNullException("engines");

            var path = _keyManager.GetEnginesKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(engines);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}