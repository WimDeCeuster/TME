﻿using System;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;

using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class PublicationService : IPublicationService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public PublicationService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }

        public async Task PutPublication(String brand, String country, Publication publication)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (publication == null) throw new ArgumentNullException("publication");

            var path = _keyManager.GetPublicationFileKey(publication.ID);
            var value = _serialiser.Serialise(publication);

            await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}
