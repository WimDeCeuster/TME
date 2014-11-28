using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelCarPackRulesController : Controller
    {
        public ActionResult Index(Guid modelID, Guid carID, Guid packID)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;
            ViewBag.CarID = carID;

            var model = new CompareView<IRuleSets>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, carID, packID),
                //NewReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, carID, packID)
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, carID,  packID)
            };

            return View("ModelRuleSets/Index", model);
        }
        private static ModelWithMetrics<IRuleSets> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid carID, Guid packID)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, carID,  packID);

            return new ModelWithMetrics<IRuleSets>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IRuleSets> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid carID, Guid packID)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, carID, packID);

            return new ModelWithMetrics<IRuleSets>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static IRuleSets GetList(IModel model, Guid carID, Guid packID)
        {
            return model.Cars.First(x => x.ID == carID).Packs.First(x => x.ID == packID).Rules;
        }
    }
}