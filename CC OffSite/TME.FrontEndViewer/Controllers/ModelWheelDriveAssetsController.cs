﻿using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;



namespace TME.FrontEndViewer.Controllers
{
    public class ModelWheelDriveAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid wheelDriveID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IAsset>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, wheelDriveID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, wheelDriveID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid wheelDriveID)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                            .WheelDrives[wheelDriveID]
                            .Assets
                            .Cast<TMME.CarConfigurator.Asset>()
                            .Select(x => new CarConfigurator.LegacyAdapter.Asset(x))
                            .ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid wheelDriveID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .WheelDrives.First(x => x.ID == wheelDriveID)
                .Assets;

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
