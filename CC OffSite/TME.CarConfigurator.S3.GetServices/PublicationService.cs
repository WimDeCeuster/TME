using System;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class PublicationService : IPublicationService
    {
        private readonly ISerialiser _serialiser;
        private readonly IService _service;
        private readonly IKeyManager _keyManager;

        public PublicationService(ISerialiser serialiser, IService service, IKeyManager keyManager)
        {
            if (serialiser == null) throw new ArgumentNullException("serialiser");
            if (service == null) throw new ArgumentNullException("service");
            if (keyManager == null) throw new ArgumentNullException("keyManager");

            _serialiser = serialiser;
            _service = service;
            _keyManager = keyManager;
        }

        public Publication GetPublication(Guid publicationId, Context context)
        {
            var key = _keyManager.GetPublicationFileKey(publicationId);
            var serializedPublication = _service.GetObject(context.Brand, context.Country, key);
            return _serialiser.Deserialise<Publication>(serializedPublication);
        }
    }
}