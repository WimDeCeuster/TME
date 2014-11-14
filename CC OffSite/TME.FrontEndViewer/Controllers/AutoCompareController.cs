using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TME.CarConfigurator.Repository.Objects;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class AutoCompareController : Controller
    {
        //
        // GET: /AutoCompare/

        public ActionResult Index()
        {
            var brand = "Toyota";
            var country = "FR";
            var language = "FR";

            var context = new Context()
            {
                Brand = brand,
                Country = country,
                Language = language
            };

            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var newModels = CarConfigurator.DI.Models.GetModels(context).ToList();
            var oldModels = TMME.CarConfigurator.Models.GetModels(oldContext)
                                                       .Cast<TMME.CarConfigurator.Model>()
                                                       .Select(x => new CarConfigurator.LegacyAdapter.Model(x))
                                                       .ToList();

            var newModel = newModels.First();
            
            var oldModel = oldModels
                .First(x => x.ID == newModel.ID);



            var compareResult = new TME.CarConfigurator.Comparer.Comparer().Compare(oldModels, newModels);


            return View(compareResult);
        }

    }
}
