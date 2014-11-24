using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;



namespace TME.FrontEndViewer.Controllers
{
    public class ModelAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid? subModelID = null)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID,subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID,subModelID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid? subModelID)
        {
            List<IAsset> list;
            var start = DateTime.Now;
            if(subModelID == null)
                list = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID))
                         .Assets.ToList();
            else
                list = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID))
                        .SubModels.First(x => x.ID == subModelID)
                        .Assets.ToList();

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? subModelID)
        {
            List<IAsset> list;
            var start = DateTime.Now;
            if(subModelID == null)
                list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).Assets.ToList();
            else
                list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .SubModels.First(x => x.ID == subModelID)
                .Assets.ToList();

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
