using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Publisher.Extensions;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class EquipmentPublisher : IEquipmentPublisher
    {
        readonly IEquipmentService _equipmentService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public EquipmentPublisher(IEquipmentService equipmentService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (equipmentService == null) throw new ArgumentNullException("equipmentService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _equipmentService = equipmentService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishObjects(
                context,
                timeFrame => timeFrame.GradeEquipments,
                Publish);
        }

        public async Task PublishCategoriesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await Task.WhenAll(context.ContextData.Select(entry =>
                _equipmentService.PutCategoriesAsync(context.Brand, context.Country, entry.Value.Publication.ID, entry.Value.EquipmentCategories)));
        }

        public async Task PublishSubModelGradeEquipmentAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _timeFramePublishHelper.PublishPerParent(
                    context,
                    timeFrame => timeFrame.SubModelGradeEquipments.Keys,
                    (timeFrame, subModelId) => timeFrame.SubModelGradeEquipments[subModelId],
                    PublishSubModelGradeEquipmentAsync);
        }

        public async Task PublishCarEquipmentAsync(IContext context)
        {
            var tasks =
                context.ContextData.Values.Select(
                    contextData =>
                        PublishCarEquipmentAsync(context.Brand, context.Country, contextData.Publication.ID,
                            contextData.CarEquipment));
            await Task.WhenAll(tasks);
        }

        async Task Publish(string brand, string country, Guid publicationId, Guid timeFrameId, IReadOnlyDictionary<Guid, GradeEquipment> gradeEquipments)
        {
            await Task.WhenAll(gradeEquipments.Select(entry =>
                _equipmentService.Put(brand, country, publicationId, timeFrameId, entry.Key, SortGradeEquipment(entry.Value))));
        }

        async Task PublishSubModelGradeEquipmentAsync(string brand, string country, Guid publicationId, Guid timeFrameId, Guid subModelID, IReadOnlyDictionary<Guid, GradeEquipment> gradeEquipments)
        {
            await Task.WhenAll(gradeEquipments.Select(entry =>
                _equipmentService.PutPerSubModel(brand, country, publicationId, timeFrameId, subModelID, entry.Key, SortGradeEquipment(entry.Value))));
        }

        async Task PublishCarEquipmentAsync(string brand, string country, Guid publicationID, IEnumerable<KeyValuePair<Guid, CarEquipment>> carEquipment)
        {
            var tasks =
                carEquipment.Select(
                    entry => _equipmentService.PutCarEquipment(brand, country, publicationID, entry.Key, SortCarEquipment(entry.Value))).ToList();
            await Task.WhenAll(tasks);
        }

        static CarEquipment SortCarEquipment(CarEquipment equipment)
        {
            return new CarEquipment
            {
                Accessories = equipment.Accessories.OrderEquipment().ToList(),
                Options = equipment.Options.OrderEquipment().ToList()
            };
        }

        static GradeEquipment SortGradeEquipment(GradeEquipment equipment)
        {
            return new GradeEquipment
            {
                Accessories = equipment.Accessories.OrderEquipment().ToList(),
                Options = equipment.Options.OrderEquipment().ToList()
            };
        }
    }
}
