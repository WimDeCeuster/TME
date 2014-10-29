using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator
{
    public class CategoryInfo : ICategoryInfo
    {
        readonly Repository.Objects.Equipment.CategoryInfo _repositoryCategory;

        public CategoryInfo(Repository.Objects.Equipment.CategoryInfo repositoryCategory)
        {
            if (repositoryCategory == null) throw new ArgumentNullException("repositoryCategory");

            _repositoryCategory = repositoryCategory;
        }

        public Guid ID
        {
            get { return _repositoryCategory.ID; }
        }

        public string Path
        {
            get { return _repositoryCategory.Path; }
        }

        public int SortIndex
        {
            get { return _repositoryCategory.SortIndex; }
        }
    }
}
