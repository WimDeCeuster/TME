using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class EquipmentService : IEquipmentService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public EquipmentService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task Put(String brand, String country, Guid publicationID, Guid timeFrameID, Guid gradeID, GradeEquipment gradeEquipment)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (gradeEquipment == null) throw new ArgumentNullException("gradeEquipment");

            var path = _keyManager.GetGradeEquipmentsKey(publicationID, timeFrameID, gradeID);
            var value = _serialiser.Serialise(gradeEquipment);

            await _service.PutObjectAsync(brand, country, path, value);
        }
        
        public async Task PutCategoriesAsync(String brand, String country, Guid publicationID, IEnumerable<Category> categories)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (categories == null) throw new ArgumentNullException("categories");

            var path = _keyManager.GetEquipmentCategoriesKey(publicationID);
            var value = _serialiser.Serialise(categories);

            await _service.PutObjectAsync(brand, country, path, value);
        }

        public async Task PutPerSubModel(String brand, String country, Guid publicationID, Guid timeFrameID, Guid subModelID, Guid gradeID, GradeEquipment gradeEquipment)
        {
            if (brand == null) throw new ArgumentNullException("brand");
            if (country == null) throw new ArgumentNullException("country");
            if (gradeEquipment == null) throw new ArgumentNullException("gradeEquipment");

            var path = _keyManager.GetSubModelGradeEquipmentsKey(publicationID, timeFrameID, gradeID, subModelID);
            var value = _serialiser.Serialise(gradeEquipment);

            await _service.PutObjectAsync(brand, country, path, value);
        }

        public async Task PutCarEquipment(string brand, string country, Guid publicationID, Guid carID, CarEquipment carEquipment)
        {
            if (carEquipment == null) throw new ArgumentNullException("carEquipment");
            if (String.IsNullOrEmpty(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrEmpty(country)) throw new ArgumentNullException("country");

            var path = _keyManager.GetCarEquipmentKey(publicationID, carID);
            var value = _serialiser.Serialise(carEquipment);

            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}