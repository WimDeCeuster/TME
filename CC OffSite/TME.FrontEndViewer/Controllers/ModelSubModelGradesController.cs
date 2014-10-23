using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelSubModelGradesController : Controller
    {
        public ActionResult Index(Guid modelID, Guid subModelID)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IGrade>()
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, subModelID)
            };

            return View("Grades/Index", model);
        }

        private ModelWithMetrics<IGrade> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid subModelID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .SubModels.First(x => x.ID == subModelID)
                .Grades.ToList();

            return new ModelWithMetrics<IGrade>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private ModelWithMetrics<IGrade> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid subModelID)
        {
            var start = DateTime.Now;
            var list =
                new CarConfigurator.LegacyAdapter.SubModel(
                    TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                    .SubModels.Cast<TMME.CarConfigurator.SubModel>()
                    .First(x => x.ID == subModelID)
                ).Grades.ToList();

            return new ModelWithMetrics<IGrade>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}