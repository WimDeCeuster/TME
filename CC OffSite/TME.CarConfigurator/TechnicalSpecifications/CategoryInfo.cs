using System;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;

namespace TME.CarConfigurator.TechnicalSpecifications
{
    public class CategoryInfo : ICategoryInfo
    {
        readonly Repository.Objects.TechnicalSpecifications.CategoryInfo _repositoryCategory;

        public CategoryInfo(Repository.Objects.TechnicalSpecifications.CategoryInfo repositoryCategory)
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
