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
    public class WheelDrivePublisher : IWheelDrivePublisher
    {
        IWheelDriveService _wheelDriveService;

        public WheelDrivePublisher(IWheelDriveService wheelDriveService)
        {
            if (wheelDriveService == null) throw new ArgumentNullException("wheelDriveService");

            _wheelDriveService = wheelDriveService;
        }

        public async Task<IEnumerable<Result>> PublishGenerationWheelDrives(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutTimeFramesGenerationWheelDrives(context.Brand, context.Country, timeFrames, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutTimeFramesGenerationWheelDrives(String brand, String country, IEnumerable<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var wheelDrives = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => data.WheelDrives.Where(wheelDrive => timeFrame.Cars.Any(car => car.WheelDrive.ID == wheelDrive.ID))
                                                         .OrderBy(wheelDrive => wheelDrive.SortIndex)
                                                         .ThenBy(wheelDrive => wheelDrive.Name)
                                                         .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in wheelDrives)
                tasks.Add(_wheelDriveService.PutTimeFrameGenerationWheelDrives(brand, country, publication.ID, entry.Key.ID, entry.Value));

            return await Task.WhenAll(tasks);
        }
    }
}
