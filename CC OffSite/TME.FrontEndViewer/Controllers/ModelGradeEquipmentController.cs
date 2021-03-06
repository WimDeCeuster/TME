﻿using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.LegacyAdapter;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;
using Model = TME.CarConfigurator.LegacyAdapter.Model;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelGradeEquipmentController : Controller
    {
        public ActionResult Index(Guid modelID, Guid gradeID, Guid? subModelID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IGradeEquipment>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, gradeID, subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, gradeID, subModelID)
            };

            return View(model);
        }

        private static ModelWithMetrics<IGradeEquipment> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid gradeID, Guid? subModelID)
        {
            var start = DateTime.Now;
            var model = new Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var gradeEquipmentModel = GetGradeEquipment(model, gradeID, subModelID);

            return new ModelWithMetrics<IGradeEquipment>
            {
                Model = gradeEquipmentModel,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IGradeEquipment> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid gradeID, Guid? subModelID)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var gradeEquipmentModel = GetGradeEquipment(model, gradeID, subModelID);

            return new ModelWithMetrics<IGradeEquipment>
            {
                Model = gradeEquipmentModel,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static IGradeEquipment GetGradeEquipment(IModel model, Guid gradeID, Guid? subModelID)
        {
            var grade = (subModelID == null
                ? model.Grades.First(x => x.ID == gradeID)
                : model.SubModels.First(x => x.ID == subModelID.Value).Grades.First(x => x.ID == gradeID));

            return grade.Equipment;
        }

    }
}
