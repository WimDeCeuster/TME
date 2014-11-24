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
    public class ModelColourCombinationVisibleInAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid exteriorColourID, Guid upholsteryID, Guid? carID, string mode, string view)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            if (string.IsNullOrEmpty(mode)) mode = string.Empty;

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, exteriorColourID, upholsteryID, carID, mode, view),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, exteriorColourID, upholsteryID, carID, mode, view)
            };

            return View("Assets/Index", model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid exteriorColourID, Guid upholsteryID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, exteriorColourID, upholsteryID, carID, mode, view);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid exteriorColourID, Guid upholsteryID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, exteriorColourID, upholsteryID, carID, mode, view);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static List<IAsset> GetList(IModel model, Guid exteriorColourID, Guid upholsteryID, Guid? carID, string mode, string view)
        {
            var visibleIn = (carID == null
                ? model.ColourCombinations.First(x => x.ExteriorColour.ID == exteriorColourID  && x.Upholstery.ID == upholsteryID).VisibleIn
                : model.Cars.First(x => x.ID == carID.Value).ColourCombinations.First(x => x.ExteriorColour.ID == exteriorColourID && x.Upholstery.ID == upholsteryID).VisibleIn);

            var visibleInModeAndView = visibleIn.FirstOrDefault(x => x.Mode == mode && x.View == view);
            var list = visibleInModeAndView == null
                ? new List<IAsset>()
                : visibleInModeAndView.Assets.ToList();
            return list;
        }

    }
}