using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class SpecificationsService : ISpecificationsService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public SpecificationsService(ISerialiser serialiser, IService service, IKeyManager keyManager)
        {
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (service == null) throw new ArgumentNullException("service");
            if (keyManager == null) throw new ArgumentNullException("keyManager");
           
            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public IEnumerable<Category> GetCategories(Guid publicationId, Guid timeFrameId, Context context)
        {
            var key = _keyManager.GetSpecificationCategoriesKey(publicationId, timeFrameId);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<IEnumerable<Category>>(serialisedObject);
        }


        public IEnumerable<CarTechnicalSpecification> GetCarTechnicalSpecifications(Guid publicationId, Guid carId, Context context)
        {
            var key = _keyManager.GetCarTechnicalSpecificationsKey(publicationId, carId);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<IEnumerable<CarTechnicalSpecification>>(serialisedObject);
        }
    }
}