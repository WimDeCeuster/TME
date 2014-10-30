using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using Model = TME.CarConfigurator.LegacyAdapter.Model;
using MyContext = TMME.CarConfigurator.MyContext;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelsController : Controller
    {

        public ActionResult SelectCountryLanguage(string countrycode, string languagecode)
        {
            return Index("Toyota", countrycode, languagecode);
        }
        public ActionResult Index()
        {
            if (Session["context"] ==  null) return Index( "Toyota","DE","DE");
            var context = ((Context) Session["context"]);
            return Index(context.Brand, context.Country, context.Language);
        }

        private ActionResult Index(string brand, string country, string language)
        {
            var context = new Context()
            {
                Brand = brand,
                Country = country,
                Language = language,
            };
            Session.Add("context", context);



            var oldContext = MyContext.NewContext(context.Brand, context.Country, context.Language);
            var model = new ModelCompare
            {
               SelectedCountry = country,
               SelectedLanguage = language,
               Countries = GetCountries(),
               OldReaderModel = GetOldReaderModelWithMetrics(oldContext),
               NewReaderModel = GetNewReaderModelWithMetrics(context)
            };

            return View("Index", model);
        }
        
        private static List<Country> GetCountries()
        {
            CarConfigurator.Administration.MyContext.SetSystemContext("Toyota", "ZZ", "EN");
            return CarConfigurator.Administration.MyContext.GetContext().Countries.Select(x=> x).ToList();
        }


        private static ModelWithMetrics<IReadOnlyList<IModel>> GetOldReaderModelWithMetrics(MyContext oldContext)
        {
            var start = DateTime.Now;
            var list = TMME.CarConfigurator.Models.GetModels(oldContext)
                .Cast<TMME.CarConfigurator.Model>()
                .Select(x => new CarConfigurator.LegacyAdapter.Model(x))
                .Cast<IModel>()
                .ToList();

            return  new ModelWithMetrics<IReadOnlyList<IModel>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }
        private static ModelWithMetrics<IReadOnlyList<IModel>> GetNewReaderModelWithMetrics(Context context)
        {
            var start = DateTime.Now;
            var list = CarConfigurator.DI.Models.GetModels(context).ToList();

            return new ModelWithMetrics<IReadOnlyList<IModel>>()
            {
                Model = list,
                TimeToLoad = DateTime.Now.Subtract(start)
            };
        }


    }
}
