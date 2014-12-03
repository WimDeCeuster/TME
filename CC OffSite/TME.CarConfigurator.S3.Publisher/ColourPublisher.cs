using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.Publisher.Extensions;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class ColourPublisher : IColourPublisher
    {
        private readonly IColourService _service;
        private readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public ColourPublisher(IColourService service, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _service = service;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishGenerationColourCombinations(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishList(context, timeFrame => timeFrame.ColourCombinations, _service.PutTimeFrameGenerationColourCombinations);
        }

        public async Task PublishCarColourCombinations(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks =
                context.ContextData.Values.Select(
                    data =>
                        PublishCarColourCombinations(context.Brand, context.Country, data.Publication.ID,
                            data.CarColourCombinations));
            await Task.WhenAll(tasks);
        }

        public async Task PublishCarPackAccentColourCombinations(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            var tasks = context.ContextData.Values.Select(
                data =>
                    PublishCarPackAccentColourCombinations(context.Brand,
                        context.Country,
                        data.Publication.ID,
                        data.CarPackAccentColourCombinations));
            await Task.WhenAll(tasks);
        }

        private async Task PublishCarPackAccentColourCombinations(string brand, string country, Guid publicationID, IEnumerable<KeyValuePair<Guid, IDictionary<Guid, IList<AccentColourCombination>>>> carPackAccentColourCombinations)
        {
            var tasks =
                carPackAccentColourCombinations.Select(
                    entry =>
                        _service.PutCarPackAccentColourCombinations(brand, country, publicationID, entry.Key, Sort(entry.Value)))
                        .ToList();
            await Task.WhenAll(tasks);
        }

        private IDictionary<Guid, List<AccentColourCombination>> Sort(IEnumerable<KeyValuePair<Guid, IList<AccentColourCombination>>> accentColourCombinationsPerPack)
        {
            return accentColourCombinationsPerPack.ToDictionary(x => x.Key, x => x.Value.Sort());
        }

        private async Task PublishCarColourCombinations(string brand, string country, Guid publicationID, IEnumerable<KeyValuePair<Guid, IList<CarColourCombination>>> carColourCombinations)
        {
            var tasks =
                carColourCombinations.Select(
                    entry => _service.PutCarColourCombinations(brand, country, publicationID, entry.Key, entry.Value))
                    .ToList();
            await Task.WhenAll(tasks);
        }
    }
}