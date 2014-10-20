using System;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;



namespace TME.FrontEndViewer.Controllers
{
    public class ModelTransmissionAssetsController : Controller
    {
        public ActionResult Index(Guid modelID, Guid transmissionID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IAsset>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID, transmissionID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID, transmissionID)
            };

            return View("Assets/Index",model);
        }

        private static ModelWithMetrics<IAsset> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID, Guid transmissionID)
        {
            var start = DateTime.Now;
            var list =
                new CarConfigurator.LegacyAdapter.Transmission(
                    TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                    .Transmissions.Cast<TMME.CarConfigurator.Transmission>()
                    .First(x => x.ID == transmissionID)
                ).Assets.ToList();


            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IAsset> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid transmissionID)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .Transmissions.First(x => x.ID == transmissionID)
                .Assets
                .ToList();

            return new ModelWithMetrics<IAsset>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
