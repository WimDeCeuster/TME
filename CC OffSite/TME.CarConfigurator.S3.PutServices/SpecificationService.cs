using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;

using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class SpecificationsService : ISpecificationsService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public SpecificationsService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task PutCategoriesAsync(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Category> categories)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (categories == null) throw new ArgumentNullException("categories");

            var path = _keyManager.GetSpecificationCategoriesKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(categories);

            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}