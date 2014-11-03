using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelTechnicalSpecificationCategoriesController : Controller
    {

        public ActionResult Index(Guid modelID)
        {
            var context = (Context)Session["context"];
            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            ViewBag.ModelID = modelID;

            var model = new CompareView<IReadOnlyList<ICategory>>
            {
                OldReaderModel = GetOldReaderModelWithMetrics(oldContext, modelID),
                NewReaderModel = GetNewReaderModelWithMetrics(context, modelID)
            };

            return View(model);
        }
        private static ModelWithMetrics<IReadOnlyList<ICategory>> GetOldReaderModelWithMetrics(MyContext oldContext, Guid modelID)
        {
            var start = DateTime.Now;
            var model = new CarConfigurator.LegacyAdapter.Model(TMME.CarConfigurator.Model.GetModel(oldContext, modelID));
            var list = GetList(model);


            return new ModelWithMetrics<IReadOnlyList<ICategory>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }


        private static ModelWithMetrics<IReadOnlyList<ICategory>> GetNewReaderModelWithMetrics(Context context, Guid modelID)
        {
            var start = DateTime.Now;

            var model = CarConfigurator.DI.Models.GetModels(context).First(x => x.ID == modelID);
            var list = GetList(model);
            return new ModelWithMetrics<IReadOnlyList<ICategory>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }


        private static IReadOnlyList<ICategory> GetList(IModel model)
        {
            var list = new List<ICategory>();
            AddToList(list, model.TechnicalSpecifications.Categories);
            return list;
        }

        private static void AddToList(ICollection<ICategory> list, IEnumerable<ICategory> categories)
        {
            foreach (var category in categories)
            {
                list.Add(category);
                AddToList(list, category.Categories);
            }
        }
    }
}
