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
            _service = service ?? new S3Service(null);
            _serialiser = serialiser ?? new S3Serialiser();
        }

        public async Task<IEnumerable<Result>> PutGenerationBodyTypes(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutTimeFramesGenerationBodyTypes(context.Brand, context.Country, timeFrames, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutTimeFramesGenerationBodyTypes(String brand, String country, IReadOnlyList<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var bodyTypes = timeFrames.ToDictionary(
                                timeFrame => timeFrame,
                                timeFrame => data.GenerationBodyTypes.Where(bodyType =>
                                                                            timeFrame.Cars.Any(car => car.BodyType.ID == bodyType.ID))
                                                                     .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in bodyTypes)
                tasks.Add(PutTimeFrameGenerationBodyTypes(brand, country, publication, entry.Key, entry.Value));

            return await Task.WhenAll(tasks);
        }

        async Task<Result> PutTimeFrameGenerationBodyTypes(String brand, String country, Publication publication, TimeFrame timeFrame, IEnumerable<BodyType> bodyTypes)
        {
            var path = String.Format(_generationBodyTypesTimeFramePath, publication.ID, timeFrame.ID);
            var value = _serialiser.Serialise(bodyTypes);

            return await _service.PutObjectAsync(brand, country, path, value);
        }
    }
}
