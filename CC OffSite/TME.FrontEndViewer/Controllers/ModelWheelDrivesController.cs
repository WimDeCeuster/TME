using System;
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
    public class ModelWheelDrivesController : Controller
    {

        public ActionResult Index(Guid modelID)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;

            var model = new CompareView<IWheelDrive>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID)
            };

            return View(model);
        }
        private static ModelWithMetrics<IWheelDrive> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                            .WheelDrives
                            .Cast<TMME.CarConfigurator.WheelDrive>()
                            .Select(x => new CarConfigurator.LegacyAdapter.WheelDrive(x))
                            .Cast<IWheelDrive>()
                            .ToList();

            return new ModelWithMetrics<IWheelDrive>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IWheelDrive> GetNewReaderModelWithMetrics(Context context, Guid modelID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).WheelDrives.ToList();

            return new ModelWithMetrics<IWheelDrive>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
