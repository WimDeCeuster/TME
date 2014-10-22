using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class SubModelPublisher : ISubModelPublisher
    {
        private readonly ISubModelService _subModelService;

        public SubModelPublisher(ISubModelService subModelService)
        {
            if (subModelService == null) throw new ArgumentNullException("subModelService");

            _subModelService = subModelService;
        }

        public async Task<IEnumerable<Result>> PublishGenerationSubModelsAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<IEnumerable<Result>>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;
                var timeFrames = context.TimeFrames[language];

                tasks.Add(PutGenerationSubModelsPerTimeFrameAsync(context.Brand,context.Country,timeFrames,data));
            }

            var result = await Task.WhenAll(tasks);
            return result.SelectMany(xs => xs);
        }

        private async Task<IEnumerable<Result>> PutGenerationSubModelsPerTimeFrameAsync(String brand, String country, IEnumerable<TimeFrame> timeFrames, ContextData data)
        {
            var publication = data.Publication;

            var subModels = timeFrames.ToDictionary(
                timeFrame =>
                    data.Publication.TimeFrames.Single(publicationTimeFrame => publicationTimeFrame.ID == timeFrame.ID),
                timeFrame =>
                    data.SubModels.Where(subModel => timeFrame.Cars.Any(car => car.SubModel.ID == subModel.ID))
                        .OrderBy(subModel => subModel.SortIndex)
                        .ThenBy(subModel => subModel.Name)
                        .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in subModels)
                tasks.Add(_subModelService.PutTimeFrameGenerationSubModelsAsync(brand, country, publication.ID, entry.Key.ID, entry.Value));

            return await Task.WhenAll(tasks);
        }
    }
}