using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.TechnicalSpecifications
{
    public class ModelTechnicalSpecifications : IModelTechnicalSpecifications
    {
        private readonly Publication _publication;
        private readonly Context _context;
        private readonly ISpecificationsFactory _specificationsFactory;

        private IReadOnlyList<ICategory> _categories;

        public ModelTechnicalSpecifications(Publication publication, Context context, ISpecificationsFactory specificationsFactory)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (specificationsFactory == null) throw new ArgumentNullException("specificationsFactory");

            _publication = publication;
            _context = context;
            _specificationsFactory = specificationsFactory;
        }

        public IReadOnlyList<ICategory> Categories { get { return _categories = _categories ?? _specificationsFactory.GetCategories(_publication, _context); } }
    }
}
