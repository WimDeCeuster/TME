using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class WheelDriveService : IWheelDriveService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public WheelDriveService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }        

        public async Task<Result> PutTimeFrameGenerationWheelDrives(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<WheelDrive> wheelDrives)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (wheelDrives == null) throw new ArgumentNullException("wheelDrives");

            var path = _keyManager.GetWheelDrivesKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(wheelDrives);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}