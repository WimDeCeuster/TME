using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;



namespace TME.FrontEndViewer.Controllers
{
    public class ModelBodyTypeVisibleInAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid bodyTypeID, string mode, string view)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IAsset>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, bodyTypeID, mode, view),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, bodyTypeID, mode, view)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid bodyTypeID, string mode, string view)
        {
            var start = DateTime.Now;
            var bodyType = new CarConfigurator.LegacyAdapter.BodyType(
                TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                    .BodyTypes.Cast<TMME.CarConfigurator.BodyType>()
                    .First(x => x.ID == bodyTypeID)
                );
            var visibleIn = bodyType.VisibleIn.FirstOrDefault(x => x.Mode == mode && x.View == view);
            var list = visibleIn == null 
                ? new List<IAsset>()
                :  visibleIn.Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid bodyTypeID, string mode, string view)
        {
            var start = DateTime.Now;
            var bodyType = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .BodyTypes.First(x => x.ID == bodyTypeID);
            var visibleIn = bodyType.VisibleIn.FirstOrDefault(x => x.Mode == mode && x.View == view);
            var list = visibleIn == null
                ? new List<IAsset>()
                : visibleIn.Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
