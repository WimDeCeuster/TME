using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelGradeEquipmentController : Controller
    {
        public ActionResult Index(Guid modelID, Guid gradeID, Guid? subModelID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<GradeEquipmentModel>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, gradeID, subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, gradeID, subModelID)
            };

            return View(model);
        }

        private static ModelWithMetrics<GradeEquipmentModel> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid gradeID, Guid? subModelID)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var gradeEquipmentModel = GetGradeEquipment(model, gradeID, subModelID);

            return new ModelWithMetrics<GradeEquipmentModel>()
            {
                Model = gradeEquipmentModel,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<GradeEquipmentModel> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid gradeID, Guid? subModelID)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var gradeEquipmentModel = GetGradeEquipment(model, gradeID, subModelID);

            return new ModelWithMetrics<GradeEquipmentModel>()
            {
                Model = gradeEquipmentModel,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static GradeEquipmentModel GetGradeEquipment(IModel model, Guid gradeID, Guid? subModelID)
        {
            var grade = (subModelID == null
                ? model.Grades.First(x => x.ID == gradeID)
                : model.SubModels.First(x => x.ID == subModelID.Value).Grades.First(x => x.ID == gradeID));

            return new GradeEquipmentModel
            {
                Accessories = grade.Equipment.OfType<IGradeAccessory>().ToList(),
                Options = grade.Equipment.OfType<IGradeOption>().ToList(),
                Rest = grade.Equipment.Where(item => !(item is IGradeAccessory) && !(item is IGradeOption)).ToList()
            };
        }

    }
}
