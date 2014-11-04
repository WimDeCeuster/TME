using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using RepositoryCategory = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.Category;

namespace TME.CarConfigurator.TechnicalSpecifications
{
    public class Category : BaseObject<RepositoryCategory>, ICategory
    {

        public Category(RepositoryCategory category)
            : base(category)
        { }

        public string Path
        {
            get { return RepositoryObject.Path; }
        }

        public ICategory Parent
        {
            get;
            internal set;
        }

        public IReadOnlyList<ICategory> Categories
        {
            get;
            internal set;
        }
    }
}
