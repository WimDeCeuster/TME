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
        private readonly List<IModel> _oldReaderModel = new List<IModel>();
        private readonly IList<IModel> _newReaderModel = new List<IModel>();

        public ToyotaViewController()
        {
            _context = MyContext.NewContext("BE", "nl");
        }

        public ActionResult Index()
        {
            var oldData = TMME.CarConfigurator.Models.GetModels(_context).Cast<Model>();
            var newData = new List<Model>();
//            TME.CarConfigurator.QueryRepository.IModelRepository.GetModels(_context).Cast<Model>(); todo Implementatie.

            //Old Reader Model
            AutoMapper.Mapper.CreateMap<Model, ModelDTO>();

            foreach (var item in oldData)
            {
                var modelDTO = AutoMapper.Mapper.Map<ModelDTO>(item);
                _oldReaderModel.Add(modelDTO);
            }

            //New Reader Model
            foreach (var item in newData)
            {
                var modelDTO = AutoMapper.Mapper.Map<ModelDTO>(item);
                _newReaderModel.Add(modelDTO);
            }

            var model = new ComparingViewModel {OldReaderModel = _oldReaderModel,NewReaderModel = _newReaderModel};


            return View(model);
        }

    }
}
