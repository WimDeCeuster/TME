using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Equipment
{
    public class ModelEquipment : IModelEquipment
    {
        readonly Context _context;
        readonly Publication _publication;
        readonly IEquipmentFactory _equipmentFactory;

        private IReadOnlyList<ICategory> _categories;

        public ModelEquipment(Publication publication, Context context, IEquipmentFactory equipmentFactory)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (equipmentFactory == null) throw new ArgumentNullException("equipmentFactory");

            _publication = publication;
            _context = context;
            _equipmentFactory = equipmentFactory;
        }

        public IReadOnlyList<ICategory> Categories { get { return _categories = _categories ?? _equipmentFactory.GetCategories(_publication, _context); } }
    }
}