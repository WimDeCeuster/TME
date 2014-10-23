using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class GradeAccessoryService : IGradeAccessoryService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public GradeAccessoryService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> Put(String brand, String country, Guid publicationID, Guid timeFrameID, Guid gradeID, IEnumerable<GradeAccessory> gradeAccessories)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (gradeAccessories == null) throw new ArgumentNullException("gradeAccessories");

            var path = _keyManager.GetGradeAccessoriesKey(publicationID, timeFrameID, gradeID);
            var value = _serialiser.Serialise(gradeAccessories);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}