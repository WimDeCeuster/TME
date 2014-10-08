using System;
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
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");

            _service = service;
            _serialiser = serialiser;
        }

        public async Task<Result> PutPublication(Publication publication)
        {
            if (publication == null) throw new ArgumentNullException("publication");

            var path = String.Format(_publicationPathTemplate, publication.ID);
            var value = _serialiser.Serialise(publication);

            return await _service.PutObjectAsync(path, value);
        }
    }
}
