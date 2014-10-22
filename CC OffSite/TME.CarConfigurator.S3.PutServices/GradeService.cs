using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.S3.CommandServices
{
    public class GradeService : IGradeService
    {
        readonly IService _service;
        readonly ISerialiser _serialiser;
        readonly IKeyManager _keyManager;

        public GradeService(IService service, ISerialiser serialiser, IKeyManager keyManager)
        {
            _service = service;
            _serialiser = serialiser;
            _keyManager = keyManager;
        }        

        public async Task<Result> PutTimeFrameGenerationGrades(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<Grade> grades)
        {
            if (String.IsNullOrWhiteSpace(brand)) throw new ArgumentNullException("brand");
            if (String.IsNullOrWhiteSpace(country)) throw new ArgumentNullException("country");
            if (grades == null) throw new ArgumentNullException("grades");

            var path = _keyManager.GetGradesKey(publicationID, timeFrameID);
            var value = _serialiser.Serialise(grades);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}