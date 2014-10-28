using System;
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
    public class ModelBodyTypeVisibleInAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid? bodyTypeID, Guid? carID, string mode, string view)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            if (string.IsNullOrEmpty(mode)) mode = string.Empty;

            var model = new CompareView<IAsset>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, bodyTypeID, carID, mode, view),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, bodyTypeID, carID, mode, view)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid? bodyTypeID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, bodyTypeID, carID, mode, view);

            return new ModelWithMetrics<IAsset>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }



        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? bodyTypeID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, bodyTypeID, carID, mode, view);
            
            return new ModelWithMetrics<IAsset>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static List<IAsset> GetList(IModel model, Guid? bodyTypeID, Guid? carID, string mode, string view)
        {
            var bodyType = (carID == null
                ? model.BodyTypes.First(x => x.ID == bodyTypeID.Value)
                : model.Cars.First(x => x.ID == carID.Value).BodyType);

            var visibleIn = bodyType.VisibleIn.FirstOrDefault(x => x.Mode == mode && x.View == view);
            var list = visibleIn == null
                ? new List<IAsset>()
                : visibleIn.Assets.ToList();
            return list;
        }

    }
}
