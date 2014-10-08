using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3PublicationService : IS3PublicationService
    {
        readonly IS3Service _service;
        readonly IS3Serialiser _serialiser;
        readonly String _publicationPathTemplate = "publication/{0}";

        public S3PublicationService(IS3Service service, IS3Serialiser serialiser)
        {
            _service = service ?? new S3Service(null);
            _serialiser = serialiser ?? new S3Serialiser();
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
            var path = String.Format(_publicationPathTemplate, publication.ID);
            var value = _serialiser.Serialise(publication);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}
