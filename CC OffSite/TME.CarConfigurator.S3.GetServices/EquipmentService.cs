using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class EquipmentService : IEquipmentService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public EquipmentService(ISerialiser serialiser,IService service,IKeyManager keyManager)
        {
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (service == null) throw new ArgumentNullException("service");
            if (keyManager == null) throw new ArgumentNullException("keyManager");
           
            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public GradeEquipment GetGradeEquipment(Guid publicationId, Guid timeFrameId, Guid gradeId, Context context)
        {
            var key = _keyManager.GetGradeEquipmentsKey(publicationId, timeFrameId, gradeId);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<GradeEquipment>(serialisedObject);
        }

        public IEnumerable<Category> GetCategories(Guid publicationId, Guid timeFrameId, Context context)
        {
            var key = _keyManager.GetEquipmentCategoriesKey(publicationId, timeFrameId);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<IEnumerable<Category>>(serialisedObject);
        }

        public GradeEquipment GetSubModelGradeEquipment(Guid publicationID, Guid timeFrameID, Guid gradeID, Guid subModelID,
            Context context)
        {
            var key = _keyManager.GetSubModelGradeEquipmentsKey(publicationID, timeFrameID, gradeID, subModelID);
            var serialisedObject = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<GradeEquipment>(serialisedObject);
        }
    }
}