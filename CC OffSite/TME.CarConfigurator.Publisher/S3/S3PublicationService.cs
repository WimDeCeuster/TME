using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared;
using IContext = TME.CarConfigurator.Publisher.Interfaces.IContext;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3PublicationService : IS3PublicationService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public S3PublicationService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service ?? new Service(null);
            _serialiser = serialiser ?? new Serialiser();
            _keyManager = keyManager ?? new KeyManager();
        }

        public async Task<IEnumerable<Result>> PutPublications(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<Result>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;

                tasks.Add(PutPublication(context.Brand, context.Country, data.Publication));
            }

            return await Task.WhenAll(tasks);
        }

        async Task<Result> PutPublication(String brand, String country, Publication publication)
        {
            var path = _keyManager.GetPublicationKey(publication.ID);
            var value = _serialiser.Serialise(publication);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}
