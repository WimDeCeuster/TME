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
            var country = "DE";
            var language = "DE";

            var context = new Context()
            {
                Brand = brand,
                Country = country,
                Language = language,
            };

            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);

            var newModel = CarConfigurator.DI.Models.GetModels(context).First();

            var oldModel = TMME.CarConfigurator.Models.GetModels(oldContext)
                .Cast<TMME.CarConfigurator.Model>()
                .Select(x => new CarConfigurator.LegacyAdapter.Model(x))
                .First(x => x.ID == newModel.ID);

            var compareResult = new TME.CarConfigurator.Comparer.Comparer().Compare(oldModel, newModel);


            return Content(compareResult.Result);

            //return Json(compareResult, JsonRequestBehavior.AllowGet);

            //return View();
        }

    }
}
