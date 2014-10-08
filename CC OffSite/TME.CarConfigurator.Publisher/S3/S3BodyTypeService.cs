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
    public class S3BodyTypeService : IS3BodyTypeService
    {
        readonly IS3Service _service;
        readonly IS3Serialiser _serialiser;
        readonly String _generationBodyTypesTimeFramePath = "publication/{0}/time-frame/{1}/body-types";

        public S3BodyTypeService(IS3Service service, IS3Serialiser serialiser)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");

            _service = service;
            _serialiser = serialiser;
        }


        public async Task<Result> PutGenerationBodyTypes(Publication publication, TimeFrame timeFrame, IEnumerable<BodyType> bodyTypes)
        {
            if (timeFrame == null) throw new ArgumentNullException("timeFrame");
            if (bodyTypes == null) throw new ArgumentNullException("bodyTypes");

            var path = String.Format(_generationBodyTypesTimeFramePath, publication.ID, timeFrame.ID);
            var value = _serialiser.Serialise(bodyTypes);

            return await _service.PutObjectAsync(path, value);
        }
    }
}
