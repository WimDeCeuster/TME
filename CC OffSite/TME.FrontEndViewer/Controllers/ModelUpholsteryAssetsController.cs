using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;
using System.Collections.Generic;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelUpholsteryAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid upholsteryID, Guid? carID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, upholsteryID, carID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, upholsteryID, carID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid upholsteryID, Guid? carID)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, upholsteryID, carID);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid upholsteryID, Guid? carID)
        {
            var start = DateTime.Now;

            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, upholsteryID, carID);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static List<IAsset> GetList(IModel model, Guid upholsteryID, Guid? carID)
        {
            return (carID == null
                ? model.ColourCombinations.First(x => x.Upholstery.ID == upholsteryID).Assets.ToList()
                : model.Cars.First(x => x.ID == carID.Value).ColourCombinations.First(x => x.Upholstery.ID == upholsteryID).Assets.ToList());

        }

    }
}
