﻿using System;
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
    public class ModelTransmissionVisibleInAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid? transmissionID, Guid? carID, string mode, string view)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            if (string.IsNullOrEmpty(mode)) mode = string.Empty;

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, transmissionID, carID, mode, view),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, transmissionID, carID, mode, view)
            };

            return View("Assets/Index", model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid? transmissionID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, transmissionID, carID, mode, view);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? transmissionID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, transmissionID, carID, mode, view);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static List<IAsset> GetList(IModel model, Guid? transmissionID, Guid? carID, string mode, string view)
        {
            var transmission = (carID == null)
                ? model.Transmissions.First(x => x.ID == transmissionID.Value)
                : model.Cars.First(x => x.ID == carID.Value).Transmission;

            var visibleIn = transmission.VisibleIn.FirstOrDefault(x => x.Mode == mode && x.View == view);
            var list = visibleIn == null
                ? new List<IAsset>()
                : visibleIn.Assets.ToList();
            return list;
        }

    }
}
