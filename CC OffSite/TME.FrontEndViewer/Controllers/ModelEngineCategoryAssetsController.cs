﻿using System;
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
    public class ModelEngineCategoryAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid engineID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, engineID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, engineID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid engineID)
        {
            var start = DateTime.Now;

            var category = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID))
                .Engines.First(x => x.ID == engineID)
                .Category;

            var list = category == null ? new List<IAsset>() : category.Assets.ToList();


            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid engineID)
        {
            var start = DateTime.Now;

            var category = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .Engines.First(x => x.ID == engineID)
                .Category;

            var list = category == null ? new List<IAsset>() : category.Assets.ToList();

            return new ModelWithMetrics<IReadOnlyList<IAsset>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
