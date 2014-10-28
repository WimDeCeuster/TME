using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using Car = TMME.CarConfigurator.Car;
using Model = TMME.CarConfigurator.Model;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelCarsController : Controller
    {

        public ActionResult Index(Guid modelID)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;

            var model = new CompareView<IReadOnlyList<ICar>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID)
            };

            return View(model);
        }
        private static ModelWithMetrics<IReadOnlyList<ICar>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                            .Cars
                            .Cast<TMME.CarConfigurator.Car>()
                            .Select(x => new CarConfigurator.LegacyAdapter.Car(x))
                            .Cast<ICar>()
                            .ToList();

            return new ModelWithMetrics<IReadOnlyList<ICar>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<ICar>> GetNewReaderModelWithMetrics(Context context, Guid modelID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).Cars.ToList();

            return new ModelWithMetrics<IReadOnlyList<ICar>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
