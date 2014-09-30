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
        private readonly List<IModel> _newModel = new List<IModel>(); 

        public ToyotaViewController()
        {
            _context = MyContext.NewContext("BE", "nl");
        }

        public ActionResult Index()
        {
            var model = TMME.CarConfigurator.Models.GetModels(_context).Cast<Model>();
            AutoMapper.Mapper.CreateMap<Model, ModelDTO>();

            foreach (var item in model)
            {
                var modelDTO = AutoMapper.Mapper.Map<ModelDTO>(item);
                _newModel.Add(modelDTO);
            }
            

            return View(_newModel);
        }

    }
}
