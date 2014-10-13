using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;
using Model = TME.CarConfigurator.LegacyAdapter.Model;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelsController : Controller
    {
        public ActionResult Index()
        {

            return Index( "Toyota","DE","DE");
        }

        private ActionResult Index(string brand, string country, string language)
        {
            var context = new Context()
            {
                Brand = brand,
                Country = country,
                Language = language
            };
            Session.Add("context", context);


            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            var model = new CompareView<IModel>
            {
               OldReaderModel = GetOldReaderModelWithMetrics(oldContext),
               NewReaderModel = GetNewReaderModelWithMetrics(context)
            };

            return View(model);
        }



        private static ModelWithMetrics<IModel> GetOldReaderModelWithMetrics(MyContext oldContext)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Models.GetModels(oldContext)
                .Cast<TMME.CarConfigurator.Model>()
                .Select(x => new CarConfigurator.LegacyAdapter.Model(x))
                .ToList();

            return  new ModelWithMetrics<IModel>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IModel> GetNewReaderModelWithMetrics(Context context)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models.GetModels(context);

            return new ModelWithMetrics<IModel>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
    }
}
