using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelTransmissionsController : Controller
    {

        public ActionResult Index(Guid modelID)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;

            var model = new CompareView<IReadOnlyList<ITransmission>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID)
            };

            return View(model);
        }
        private static ModelWithMetrics<IReadOnlyList<ITransmission>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                            .Transmissions
                            .Cast<TMME.CarConfigurator.Transmission>()
                            .Select(x => new CarConfigurator.LegacyAdapter.Transmission(x))
                            .Cast<ITransmission>()
                            .ToList();

            return new ModelWithMetrics<IReadOnlyList<ITransmission>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<ITransmission>> GetNewReaderModelWithMetrics(Context context, Guid modelID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).Transmissions.ToList();

            return new ModelWithMetrics<IReadOnlyList<ITransmission>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
