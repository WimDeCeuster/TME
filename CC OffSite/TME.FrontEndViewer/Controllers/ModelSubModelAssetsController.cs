using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelSubModelAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid? subModelID, Guid? carID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, subModelID, carID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, subModelID, carID)
            };

            return View("Assets/Index", model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid? subModelID, Guid? carID)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, subModelID, carID);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? subModelID, Guid? carID)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, subModelID, carID);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static IReadOnlyList<IAsset> GetList(IModel model, Guid? subModelID, Guid? carID)
        {
            var subModel = (carID == null
                ? model.SubModels.First(x => x.ID == subModelID.Value)
                : model.Cars.First(x => x.ID == carID.Value).SubModel);

            return subModel.Assets.ToList();
        }
    }
}