using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelCarEquipmentVisibleInAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid carID, Guid? packID, Guid equipmentID, string mode, string view)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            if (string.IsNullOrEmpty(mode)) mode = string.Empty;

            var model = new CompareView<IReadOnlyList<IAsset>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, carID, packID, equipmentID,mode,view),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, carID, packID, equipmentID, mode, view)
            };

            return View("Assets/Index", model);
        }

        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid carID, Guid? packID, Guid equipmentID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model, carID, packID, equipmentID, mode, view);


            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IAsset>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid carID, Guid? packID, Guid equipmentID, string mode, string view)
        {
            var start = DateTime.Now;
            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model, carID, packID, equipmentID, mode, view);


            return new ModelWithMetrics<IReadOnlyList<IAsset>>
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static List<IAsset> GetList(IModel model, Guid carID, Guid? packID, Guid equipmentID, string mode, string view)
        {
            var car = model.Cars.First(x => x.ID == carID);
            return packID.HasValue
                ? GetPackEquipmentItemAssets(car, packID.Value, equipmentID, mode, view)
                : GetEquipmentItemAssets(car, equipmentID, mode, view);
        }

        private static List<IAsset> GetPackEquipmentItemAssets(ICar car, Guid packID, Guid equipmentID, string mode, string view)
        {
            var pack = car.Packs.FirstOrDefault(x => x.ID == packID);
            if (pack == null) return new List<IAsset>();

            ICarPackEquipmentItem packItem = pack.Equipment.Options.FirstOrDefault(x => x.ID == equipmentID);
            if (packItem == null) packItem = pack.Equipment.Accessories.FirstOrDefault(x => x.ID == equipmentID);
            if (packItem == null) packItem = pack.Equipment.ExteriorColourTypes.FirstOrDefault(x => x.ID == equipmentID);
            if (packItem == null) packItem = pack.Equipment.UpholsteryTypes.FirstOrDefault(x => x.ID == equipmentID);

            if (packItem == null) return new List<IAsset>();

            var visibleIn = packItem.VisibleIn.FirstOrDefault(x => x.Mode == mode && x.View == view);
            var list = visibleIn == null
                ? new List<IAsset>()
                : visibleIn.Assets.ToList();
            return list;

        }
        private static List<IAsset> GetEquipmentItemAssets(ICar car, Guid equipmentID, string mode, string view)
        {
            ICarEquipmentItem item = car.Equipment.Options.FirstOrDefault(x => x.ID == equipmentID);
            if (item == null) item = car.Equipment.Accessories.FirstOrDefault(x => x.ID == equipmentID);

            if (item == null) return new List<IAsset>();

            var visibleIn = item.VisibleIn.FirstOrDefault(x => x.Mode == mode && x.View == view);
            var list = visibleIn == null
                ? new List<IAsset>()
                : visibleIn.Assets.ToList();
            return list;
        }
    }
}