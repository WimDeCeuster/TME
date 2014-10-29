using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using TME.CarConfigurator.Repository.Objects;



namespace TME.FrontEndViewer.Controllers
{
    public class ModelLinksController : Controller
    {
        public ActionResult Index(Guid modelID, Guid? subModelID = null)
        {

            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var model = new CompareView<IReadOnlyList<ILink>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID,subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID,subModelID)
            };

            return View(model);
        }

        private static ModelWithMetrics<IReadOnlyList<ILink>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID,Guid? subModelID)
        {
            List<ILink> list;
            var start = DateTime.Now;
            if(subModelID == null)
                list = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID))
                            .Links
                            .ToList();
            else
                list =new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID))
                .SubModels
                    .First(sub => sub.ID == subModelID)
                    .Links
                    .ToList();

            return new ModelWithMetrics<IReadOnlyList<ILink>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<ILink>> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? subModelID)
        {
            List<ILink> list;
            var start = DateTime.Now;
            if(subModelID == null)
                list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).Links.ToList();
            else
                list = CarConfigurator.DI.Models.GetModels(context).First(model => model.ID == modelID)
                .SubModels
                .First(subModel => subModel.ID == subModelID).Links.ToList();

            return new ModelWithMetrics<IReadOnlyList<ILink>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

    }
}
