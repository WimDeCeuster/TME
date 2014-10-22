using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelSubModelLinksController : Controller
    {
        public ActionResult Index(Guid modelID,Guid subModelID)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<ILink>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext,modelID, subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID,subModelID)
            };

            return View("Links/Index",model);
        }

        private static ModelWithMetrics<ILink> GetOldReaderModelWithMetrics(MyContext oldContext,Guid modelID, Guid subModelID)
        {
            var start = DateTime.Now;
            var links = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID))
                .SubModels
                    .First(sub => sub.ID == subModelID)
                    .Links
                    .ToList();


            return new ModelWithMetrics<ILink>()
            {
                Model = links,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<ILink> GetNewReaderModelWithMetrics(Context context,Guid modelID ,Guid subModelID)
        {
            var start = DateTime.Now;

            var list = CarConfigurator.DI.Models.GetModels(context).First(model => model.ID == modelID)
                .SubModels
                .First(subModel => subModel.ID == subModelID).Links.ToList();

            return new ModelWithMetrics<ILink>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}