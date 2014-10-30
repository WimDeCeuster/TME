using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.Publisher.Extensions;
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

        public async Task<IEnumerable<Result>> PublishSubModelGradeEquipmentAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            return await _timeFramePublishHelper.PublishObjectsPerSubModel(context,timeFrame => timeFrame.SubModels, timeFrame => timeFrame.GradeEquipments,
                    PublishSubModelGradeEquipmentAsync);
        }


        async Task<IEnumerable<Result>> Publish(string brand, string country, Guid publicationId, Guid timeFrameId, IReadOnlyDictionary<Guid, GradeEquipment> gradeEquipments)
        {
            return await Task.WhenAll(gradeEquipments.Select(entry =>
                _gradeEquipmentService.Put(brand, country, publicationId, timeFrameId, entry.Key, SortGradeEquipment(entry.Value))));
        }

        async Task<IEnumerable<Result>> PublishSubModelGradeEquipmentAsync(string brand, string country, Guid publicationId, Guid timeFrameId,Guid subModelID,List<Grade> applicableGrades , IReadOnlyDictionary<Guid, GradeEquipment> gradeEquipments)
        {
            var tasks = new List<Task<Result>>();
            foreach (var entry in gradeEquipments)
            {
                foreach (var applicableGrade in applicableGrades)
                {
                    if (entry.Key == applicableGrade.ID)
                    {
                        tasks.Add(_gradeEquipmentService.PutPerSubModel(brand, country, publicationId, timeFrameId,subModelID,entry.Key, SortGradeEquipment(entry.Value)));
                    }
                }
            }
            return await Task.WhenAll(tasks);
        }

        GradeEquipment SortGradeEquipment(GradeEquipment equipment)
        {
            return new GradeEquipment
            {
                Accessories = equipment.Accessories.OrderEquipment().ToList(),
                Options = equipment.Options.OrderEquipment().ToList()
            };
        }
    }
}
