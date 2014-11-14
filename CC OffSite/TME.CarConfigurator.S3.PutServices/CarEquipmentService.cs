using System;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class CarEquipmentService : ICarEquipmentService
    {
        private readonly IService _s3Service;
        private readonly ISerialiser _serialiser;
        private readonly IKeyManager _keymanager;

        public CarEquipmentService(IService s3Service, ISerialiser serialiser, IKeyManager keymanager)
        {
            if (s3Service == null) throw new ArgumentNullException("s3Service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (keymanager == null) throw new ArgumentNullException("keymanager");

            _s3Service = s3Service;
            _serialiser = serialiser;
            _keymanager = keymanager;
        }

        public async Task PutCarEquipment(string brand, string country, Guid publicationID, Guid carID, CarEquipment carEquipment)
        {
            if (carEquipment == null) throw new ArgumentNullException("carEquipment");
            if (String.IsNullOrEmpty(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrEmpty(country)) throw new ArgumentNullException("country");

            var path = _keymanager.GetCarEquipmentKey(publicationID, carID);
            var value = _serialiser.Serialise(carEquipment);

            await _s3Service.PutObjectAsync(brand, country, path, value);
        }
    }
}