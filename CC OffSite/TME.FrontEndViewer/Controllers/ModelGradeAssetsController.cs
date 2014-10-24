using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;



namespace TME.FrontEndViewer.Controllers
{
    public class ModelGradeAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid gradeID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IAsset>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, gradeID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, gradeID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid gradeID)
        {
            var start = DateTime.Now;
            var list =
                new CarConfigurator.LegacyAdapter.Grade(
                    TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                    .Grades.Cast<TMME.CarConfigurator.Grade>()
                    .First(x => x.ID == gradeID)
                ).Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid gradeID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .Grades.First(x=> x.ID == gradeID)
                .Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
