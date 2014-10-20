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
    public class SteeringPublisher : ISteeringPublisher
    {
        ISteeringService _steeringService;

        public SteeringPublisher(ISteeringService steeringService)
        {
            if (steeringService == null) throw new ArgumentNullException("steeringService");

            _steeringService = steeringService;
        }

        public async Task<IEnumerable<Result>> PublishGenerationSteerings(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutTimeFramesGenerationSteerings(context.Brand, context.Country, timeFrames, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutTimeFramesGenerationSteerings(String brand, String country, IEnumerable<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var steerings = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => data.Steerings.Where(steering => timeFrame.Cars.Any(car => car.Steering.ID == steering.ID))
                                                         .OrderBy(steering => steering.SortIndex)
                                                         .ThenBy(steering => steering.Name)
                                                         .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in steerings)
                tasks.Add(_steeringService.PutTimeFrameGenerationSteerings(brand, country, publication.ID, entry.Key.ID, entry.Value));

            return await Task.WhenAll(tasks);
        }
    }
}
