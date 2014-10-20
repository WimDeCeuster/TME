using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;



namespace TME.FrontEndViewer.Controllers
{
    public class ModelEngineAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid engineID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IAsset>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, engineID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, engineID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid engineID)
        {
            var start = DateTime.Now;
            var list =
                    new CarConfigurator.LegacyAdapter.Engine(
                        TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                        .Engines.Cast<TMME.CarConfigurator.Engine>()
                        .First(x => x.ID == engineID)
                    ).Assets.ToList();


            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid engineID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .Engines.First(x => x.ID == engineID)
                .Assets
                .ToList();


            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
