using System.Linq;
using System.Web.Mvc;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.FrontEndViewer.Models;
using TMME.CarConfigurator;

namespace TME.FrontEndViewer.Controllers
{
    public class ModelsController : Controller
    {
        private readonly MyContext _oldContext;
        private readonly Context _newContext;

        public ModelsController()
        {
            _oldContext = MyContext.NewContext("DE", "DE", ReaderMode.Marketing);
            _newContext = new Context {Brand = "Toyota", Country = "DE", Language = "DE"};
        }

        public ActionResult Index()
        {
            var model = new CompareView<IModel>
            {
                OldReaderModel = TMME.CarConfigurator.Models.GetModels(_oldContext)
                                        .Cast<TMME.CarConfigurator.Model>()
                                        .Select(x=> new CarConfigurator.LegacyAdapter.Model(x)),
                NewReaderModel = CarConfigurator.Models.GetModels(_newContext)
            };

            return View(model);
        }

    }
}
