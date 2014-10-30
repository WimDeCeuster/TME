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
    public class ModelSubModelsController : Controller
    {

        public ActionResult Index(Guid modelID)
        {
            var context = (Context) Session["context"];
            var oldcontext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;

            var model = new CompareView<IReadOnlyList<ISubModel>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldcontext,modelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context,modelID)
            };
            
            return View(model);
        }

        private ModelWithMetrics<IReadOnlyList<ISubModel>> GetOldReaderModelWithMetrics(MyContext oldcontext, Guid modelID)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Model.GetModel(oldcontext, modelID)
                .SubModels
                .Cast<TMME.CarConfigurator.SubModel>()
                .Select(x => new CarConfigurator.LegacyAdapter.SubModel(x))
                .Cast<ISubModel>()
                .ToList();

            return new ModelWithMetrics<IReadOnlyList<ISubModel>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static ModelWithMetrics<IReadOnlyList<ISubModel>> GetNewReaderModelWithMetrics(Context context, Guid modelID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).SubModels.ToList();

            return new ModelWithMetrics<IReadOnlyList<ISubModel>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
