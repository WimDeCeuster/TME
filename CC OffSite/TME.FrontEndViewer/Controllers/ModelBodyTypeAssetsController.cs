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
    public class ModelBodyTypeAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid bodyTypeID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IAsset>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, bodyTypeID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, bodyTypeID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid bodyTypeID)
        {
            var start = DateTime.Now;
            var list =
                new CarConfigurator.LegacyAdapter.BodyType(
                    TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                    .BodyTypes.Cast<TMME.CarConfigurator.BodyType>()
                    .First(x => x.ID == bodyTypeID)
                ).Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid bodyTypeID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .BodyTypes.First(x=> x.ID == bodyTypeID)
                .Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
