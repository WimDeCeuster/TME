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

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, bodyTypeID, carID, mode, view),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, bodyTypeID, carID, mode, view)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid? bodyTypeID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, bodyTypeID, carID, mode, view);

            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }



        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? bodyTypeID, Guid? carID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, bodyTypeID, carID, mode, view);
            
            return new ModelWithMetrics<IReadOnlyList<IAsset>>
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

            var visibleIn = bodyType.VisibleIn.FirstOrDefault(x => (string.IsNullOrEmpty(mode) || x.Mode == mode) && x.View == view);
            var list = visibleIn == null
                ? new List<IAsset>()
                : visibleIn.Assets.ToList();
            return list;
        }

    }
}
