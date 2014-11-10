using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;
using System.Collections.Generic;
using Model = TME.CarConfigurator.LegacyAdapter.Model;


namespace TME.FrontEndViewer.Controllers
{
    public class ModelGradeAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid? gradeID, Guid? carID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, gradeID, carID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, gradeID, carID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid? gradeID, Guid? carID)
        {
            var start = DateTime.Now;
            var model = new Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, gradeID, carID);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? gradeID, Guid? carID)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, gradeID, carID);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static IReadOnlyList<IAsset> GetList(IModel model, Guid? gradeID, Guid? carID)
        {
            var grade = (carID == null 
                ? model.Grades.First(x => x.ID == gradeID.Value) 
                : model.Cars.First(x => x.ID == carID.Value).Grade);

            return grade.Assets.ToList();
        }
    }
}
