using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ToyotaViewController : Controller
    {
        private readonly MyContext _context;

        public ToyotaViewController()
        {
            _context = MyContext.NewContext("BE", "nl");
        }

        public ActionResult Index()
        {

            var model = new CompareView<IModel>
            {
                OldReaderModel = TMME.CarConfigurator.Models.GetModels(_context)
                                        .Cast<Model>()
                                        .Select(x=> new CarConfigurator.LegacyAdapter.Model(x))
                                        .Cast<IModel>()
                                        .ToList(),
                NewReaderModel = new List<IModel>() //TODO: provide implementation
            };

            return View(model);
        }

    }
}
