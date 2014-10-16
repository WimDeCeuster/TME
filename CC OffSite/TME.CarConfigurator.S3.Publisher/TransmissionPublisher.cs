using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.S3.Publisher
{
    public class TransmissionPublisher : ITransmissionPublisher
    {
        ITransmissionService _transmissionService;

        public TransmissionPublisher(ITransmissionService transmissionService)
        {
            _transmissionService = transmissionService;
        }

        public async Task<IEnumerable<Result>> PublishGenerationTransmissions(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutTimeFramesGenerationTransmissions(context.Brand, context.Country, timeFrames, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutTimeFramesGenerationTransmissions(String brand, String country, IEnumerable<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var transmissions = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => data.Transmissions.Where(transmission => timeFrame.Cars.Any(car => car.Transmission.ID == transmission.ID))
                                                               .OrderBy(transmission => transmission.SortIndex)
                                                               .ThenBy(transmission => transmission.Name)
                                                               .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var transmission in transmissions)
                tasks.Add(_transmissionService.PutTimeFrameGenerationTransmissions(brand, country, publication.ID, transmission.Key.ID, transmission.Value));

            return await Task.WhenAll(tasks);
        }
    }
}
