using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelCarsController : Controller
    {
        //
        // GET: /ModelCars/

                private readonly MyContext _context;

                public ModelCarsController()
        {
            _context = MyContext.NewContext("BE", "nl");
        }

        public ActionResult Index(Guid modelID)
        {
            var model = new CompareView<ICar>
            {
                OldReaderModel = Model.GetModel(_context, modelID).Cars
                                        .Cast<Car>()
                                        .Select(x => new CarConfigurator.LegacyAdapter.Car(x))
                                        .Cast<ICar>()
                                        .ToList(),
                NewReaderModel = new List<ICar>() //TODO: provide implementation
            };

            return View(model);
        }

    }
}
