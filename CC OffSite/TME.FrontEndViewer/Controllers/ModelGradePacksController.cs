using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Packs;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelGradePacksController : Controller
    {
        public ActionResult Index(Guid modelID, Guid gradeID, Guid? subModelID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<IGradePack>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, gradeID, subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, gradeID, subModelID)
            };

            return View(model);
        }

        private static ModelWithMetrics<IReadOnlyList<IGradePack>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid gradeID, Guid? subModelID)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, gradeID, subModelID);

            return new ModelWithMetrics<IReadOnlyList<IGradePack>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IGradePack>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid gradeID, Guid? subModelID)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, gradeID, subModelID);

            return new ModelWithMetrics<IReadOnlyList<IGradePack>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static List<IGradePack> GetList(IModel model, Guid gradeID, Guid? subModelID)
        {
            var grade = (subModelID == null
                ? model.Grades.First(x => x.ID == gradeID)
                : model.SubModels.First(x => x.ID == subModelID.Value).Grades.First(x => x.ID == gradeID));

            return grade.Packs.ToList();
        }

    }
}
