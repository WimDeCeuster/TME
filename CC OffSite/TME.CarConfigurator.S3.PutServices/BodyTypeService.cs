using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class BodyTypeService : IBodyTypeService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public BodyTypeService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> PutTimeFrameGenerationBodyTypes(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<BodyType> bodyTypes)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (bodyTypes == null) throw new ArgumentNullException("bodyTypes");

            var path = _keyManager.GetGenerationBodyTypesKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(bodyTypes);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}
