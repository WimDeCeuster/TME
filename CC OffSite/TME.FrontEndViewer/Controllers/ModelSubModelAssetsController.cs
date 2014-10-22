using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelSubModelAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid subModelID)
        {
            var context = (Context) Session["context"];
            var oldcontext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IAsset>()
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldcontext, modelID, subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context,modelID,subModelID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid subModelID)
        {
            var start = DateTime.Now;
            var list =
                new CarConfigurator.LegacyAdapter.SubModel(
                    TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                    .SubModels.Cast<TMME.CarConfigurator.SubModel>()
                    .First(x => x.ID == subModelID)
                ).Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid subModelID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .SubModels.First(x => x.ID == subModelID)
                .Assets.ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
