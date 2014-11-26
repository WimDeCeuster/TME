using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.S3.Publisher.Extensions;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class PackPublisher : IPackPublisher
    {
        private readonly IPackService _service;
        private readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public PackPublisher(IPackService service, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _service = service;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishGradePacksAsync(IContext context)
        {
            await _timeFramePublishHelper.PublishObjects(context, timeFrame => timeFrame.GradePacks, Publish);
        }

        public async Task PublishSubModelGradePacksAsync(IContext context)
        {
            await _timeFramePublishHelper.PublishPerParent(context, timeFrame => timeFrame.SubModelGradePacks.Keys, (timeFrame, subModelId) => timeFrame.SubModelGradePacks[subModelId], Publish);
        }

        public async Task PublishCarPacksAsync(IContext context)
        {
            await Task.WhenAll(context.ContextData.Values.Select(contextData => Publish(context.Brand, context.Country, contextData.Publication.ID, contextData.CarPacks)));
        }

        private async Task Publish(string brand, string country, Guid publicationId, Guid timeFrameId, IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>> gradePacks)
        {
            await Task.WhenAll(gradePacks.Select(entry => _service.PutGradePacksAsync(brand, country, publicationId, timeFrameId, entry.Key, entry.Value)));
        }

        private async Task Publish(string brand, string country, Guid publicationId, Guid timeFrameId, Guid subModelId, IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>> gradePacks)
        {
            await Task.WhenAll(gradePacks.Select(entry => _service.PutSubModelGradePacksAsync(brand, country, publicationId, timeFrameId, subModelId, entry.Key, entry.Value)));
        }

        private async Task Publish(string brand, string country, Guid id, IEnumerable<KeyValuePair<Guid, IReadOnlyList<CarPack>>> carPacks)
        {
            await Task.WhenAll(carPacks.Select(entry => _service.PutCarPacksAsync(brand, country, id, entry.Key, entry.Value.OrderEquipment().OrderBy(carPack => carPack.SortIndex))));
        }
    }
}