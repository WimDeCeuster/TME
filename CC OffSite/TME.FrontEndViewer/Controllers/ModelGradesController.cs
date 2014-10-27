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
    public class ModelGradesController : Controller
    {

        public ActionResult Index(Guid modelID, Guid? subModelID = null)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;
            ViewBag.SubModelID = subModelID;

            var model = new CompareView<IGrade>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID,subModelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID,subModelID)
            };

            return View(model);
        }
        private static ModelWithMetrics<IGrade> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID,Guid? subModelID)
        {
            List<IGrade> list;
            var start = DateTime.Now;

            if (subModelID == null)
                list = TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                            .Grades
                            .Cast<TMME.CarConfigurator.Grade>()
                            .Select(x => new CarConfigurator.LegacyAdapter.Grade(x))
                            .Cast<IGrade>()
                            .ToList();
            else
                list =
                new CarConfigurator.LegacyAdapter.SubModel(
                    TMME.CarConfigurator.Model.GetModel(oldContext, modelID)
                    .SubModels.Cast<TMME.CarConfigurator.SubModel>()
                    .First(x => x.ID == subModelID)
                ).Grades.ToList();

            return new ModelWithMetrics<IGrade>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }

        private static ModelWithMetrics<IGrade> GetNewReaderModelWithMetrics(Context context, Guid modelID, Guid? subModelID )
        {
            List<IGrade> list;
            var start = DateTime.Now;
            if (subModelID == null)
                list = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID).Grades.ToList(); 
            else
                list = CarConfigurator.DI.Models
                .GetModels(context).First(x => x.ID == modelID)
                .SubModels.First(x => x.ID == subModelID)
                .Grades.ToList();
            
                


            return new ModelWithMetrics<IGrade>()
            {
                Model = list.ToList(),
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
