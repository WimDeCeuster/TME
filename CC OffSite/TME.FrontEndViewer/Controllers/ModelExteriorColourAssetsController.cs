using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;
using System.Collections.Generic;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelExteriorColourAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid exteriorColourID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, exteriorColourID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, exteriorColourID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid exteriorColourID)
        {
            var start = DateTime.Now;
            var list = new CarConfigurator.LegacyAdapter.Model(
                        TMME.CarConfigurator.Model.GetModel(oldContext, modelID))
                        .ColourCombinations
                        .Select(cc => cc.ExteriorColour)
                        .First(x => x.ID == exteriorColourID)
                        .Assets.ToList();

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid exteriorColourID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .ColourCombinations
                .Select(cc => cc.ExteriorColour)
                .First(x=> x.ID == exteriorColourID)
                .Assets.ToList();

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
