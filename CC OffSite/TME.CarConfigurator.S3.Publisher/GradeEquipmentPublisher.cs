using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class GradeEquipmentPublisher : IGradeEquipmentPublisher
    {
        readonly IGradeEquipmentService _gradeEquipmentService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public GradeEquipmentPublisher(IGradeEquipmentService gradeEquipmentService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (gradeEquipmentService == null) throw new ArgumentNullException("gradeEquipmentService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _gradeEquipmentService = gradeEquipmentService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task<IEnumerable<Result>> PublishAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishObjects(
                context,
                timeFrame => timeFrame.GradeEquipments,
                Publish);
        }

        async Task<IEnumerable<Result>> Publish(string brand, string country, Guid publicationId, Guid timeFrameId, IReadOnlyDictionary<Guid, GradeEquipment> gradeEquipments)
        {
            return await Task.WhenAll(gradeEquipments.Select(entry =>
                _gradeEquipmentService.Put(brand, country, publicationId, timeFrameId, entry.Key, entry.Value)));
        }
    }
}
