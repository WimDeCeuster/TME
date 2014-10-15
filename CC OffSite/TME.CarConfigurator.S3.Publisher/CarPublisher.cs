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
    public class CarPublisher : ICarPublisher
    {
        ICarService _carService;

        public CarPublisher(ICarService carService)
        {
            _carService = carService;
        }

        public async Task<IEnumerable<Result>> PublishGenerationCars(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutTimeFramesGenerationCars(context.Brand, context.Country, timeFrames, data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PutTimeFramesGenerationCars(String brand, String country, IEnumerable<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var cars = timeFrames.ToDictionary(
                                timeFrame => data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                                timeFrame => timeFrame.Cars.OrderBy(car => car.SortIndex)
                                                           .ThenBy(car => car.Name)
                                                           .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in cars)
                tasks.Add(_carService.PutTimeFrameGenerationCars(brand, country, publication.ID, entry.Key.ID, entry.Value));

            return await Task.WhenAll(tasks);
        }
    }
}
