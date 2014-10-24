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
    public class GradeEquipmentService : IGradeEquipmentService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public GradeEquipmentService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task<Result> Put(String brand, String country, Guid publicationID, Guid timeFrameID, Guid gradeID, GradeEquipment gradeEquipment)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (gradeEquipment == null) throw new ArgumentNullException("gradeEquipment");

            var path = _keyManager.GetGradeEquipmentsKey(publicationID, timeFrameID, gradeID);
            var value = _serialiser.Serialise(gradeEquipment);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}