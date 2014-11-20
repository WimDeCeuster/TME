using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.TechnicalSpecifications;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.TechnicalSpecifications;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using RepoCategory = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.Category;

namespace TME.CarConfigurator.Factories
{
    public class SpecificationsFactory : ISpecificationsFactory
    {
        private readonly ISpecificationsService _specificationsService;
        
        public SpecificationsFactory(ISpecificationsService specificationService)
        {
            if (specificationService == null) throw new ArgumentNullException("specificationService");
           
            _specificationsService = specificationService;
        }

        public IModelTechnicalSpecifications GetModelSpecifications(Publication publication, Context context)
        {
            return new ModelTechnicalSpecifications(publication, context, this);
        }
        public IReadOnlyList<ICarTechnicalSpecification> GetCarTechnicalSpecifications(Guid carID, Publication publication, Context context)
        {
            return
                _specificationsService.GetCarTechnicalSpecifications(publication.ID, carID, context)
                    .Select(x => new CarTechnicalSpecification(x))
                    .ToList();
        }

        public IReadOnlyList<ICategory> GetCategories(Publication publication, Context context)
        {
            var repoCategories = _specificationsService.GetCategories(publication.ID, context).ToList();

            var mappedCategories = repoCategories.Select(category => new Category(category)).ToList(); ;

            LinkParents(mappedCategories, repoCategories);
            LinkChildren(mappedCategories);

            var rootCategory = GetRootCategory();
            var topLevelCategories = mappedCategories.Where(category => category.Parent == null).ToList(); 
            rootCategory.Categories = topLevelCategories;
            foreach (var category in topLevelCategories)
                category.Parent = rootCategory;

            return topLevelCategories.ToList();
        }



        static void LinkParents(IReadOnlyList<Category> mappedCategories, IReadOnlyList<RepoCategory> repoCategories)
        {
            foreach (var category in mappedCategories)
            {
                var repoCategory = repoCategories.Single(repoCat => repoCat.ID == category.ID);
                if ((repoCategory.ParentCategoryID ?? Guid.Empty) != Guid.Empty)
                    category.Parent = mappedCategories.Single(mappedCategory => mappedCategory.ID == repoCategory.ParentCategoryID);
            }
        }

        static void LinkChildren(IReadOnlyList<Category> mappedCategories)
        {
            foreach (var category in mappedCategories)
                category.Categories = mappedCategories.Where(mappedCategory => mappedCategory.Parent == category).ToList();
        }

        static Category GetRootCategory()
        {
            return new Category(new RepoCategory
            {
                Description = "",
                FootNote = "",
                InternalCode = "",
                LocalCode = "",
                Name = "",
                ToolTip = "",
                Path = ""
            });
        }
    }
}
