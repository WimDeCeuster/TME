﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelSteeringsController : Controller
    {

        public ActionResult Index(Guid modelID)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;

            var model = new CompareView<IReadOnlyList<ISteering>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID)
            };

            return View(model);
        }
        private static ModelWithMetrics<IReadOnlyList<ISteering>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                            .Cars.Cast<TMME.CarConfigurator.Car>()
                            .Select(car => car.Steering)
                            .Distinct()
                            .Select(x => new CarConfigurator.LegacyAdapter.Steering(x))
                            .Cast<ISteering>()
                            .ToList();

            return new ModelWithMetrics<IReadOnlyList<ISteering>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<ISteering>> GetNewReaderModelWithMetrics(Context context, Guid modelID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).Steerings.ToList();

            return new ModelWithMetrics<IReadOnlyList<ISteering>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
