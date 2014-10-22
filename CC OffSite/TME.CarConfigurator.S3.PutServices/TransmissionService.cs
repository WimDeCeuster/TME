using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class TransmissionService : ITransmissionService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public TransmissionService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }        

        public async Task<Result> PutTimeFrameGenerationTransmissions(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Transmission> transmissions)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (transmissions == null) throw new ArgumentNullException("transmissions");

            var path = _keyManager.GetTransmissionsKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(transmissions);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}