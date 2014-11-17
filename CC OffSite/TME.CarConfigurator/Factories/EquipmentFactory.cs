using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Equipment;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using RepoCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;
using RepoGradeAccessory = TME.CarConfigurator.Repository.Objects.Equipment.GradeAccessory;
using RepoGradeOption = TME.CarConfigurator.Repository.Objects.Equipment.GradeOption;

namespace TME.CarConfigurator.Factories
{
    public class EquipmentFactory : IEquipmentFactory
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IColourFactory _colourFactory;

        public EquipmentFactory(IEquipmentService equipmentService, IColourFactory colourFactory)
        {
            if (equipmentService == null) throw new ArgumentNullException("equipmentService");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _equipmentService = equipmentService;
            _colourFactory = colourFactory;
        }

        public IGradeEquipment GetSubModelGradeEquipment(Publication publication, Guid subModelID, Context context,
            Guid gradeID)
        {
            var gradeEquipment = _equipmentService.GetSubModelGradeEquipment(publication.ID,
                publication.GetCurrentTimeFrame().ID, gradeID, subModelID, context);

            return new GradeEquipment(
                gradeEquipment.Accessories.Select(accessory => GetGradeAccessory(accessory, publication, context)),
                gradeEquipment.Options.Select(option => GetGradeOption(option, gradeEquipment.Options, publication, context)));
        }

        public IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId)
        {
            var gradeEquipment = _equipmentService.GetGradeEquipment(publication.ID, publication.GetCurrentTimeFrame().ID, gradeId, context);

            return new GradeEquipment(
                gradeEquipment.Accessories.Select(accessory => GetGradeAccessory(accessory, publication, context)),
                gradeEquipment.Options.Select(option => GetGradeOption(option, gradeEquipment.Options, publication, context)));
        }

        IGradeAccessory GetGradeAccessory(RepoGradeAccessory accessory, Publication publication, Context context)
        {
            return new GradeAccessory(accessory, publication, context, _colourFactory);
        }

        IGradeOption GetGradeOption(RepoGradeOption repoGradeOption, IEnumerable<RepoGradeOption> repoGradeOptions, Publication publication, Context context)
        {
            var parentGradeOption = repoGradeOption.ParentOptionShortID == 0
                ? null
                : repoGradeOptions.Single(grd => grd.ShortID == repoGradeOption.ParentOptionShortID);

            var parentOptionInfo = parentGradeOption == null ? null : new OptionInfo(parentGradeOption.ShortID, parentGradeOption.Name);

            return new GradeOption(repoGradeOption, parentOptionInfo, publication, context, _colourFactory);
        }

        public IModelEquipment GetModelEquipment(Publication publication, Context context)
        {
            return new ModelEquipment(publication, context, this);
        }

        public IReadOnlyList<ICategory> GetCategories(Publication publication, Context context)
        {
            var repoCategories = _equipmentService.GetCategories(publication.ID, publication.GetCurrentTimeFrame().ID, context).ToList();

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
                ToolTip = ""
            });
        }
    }
}
