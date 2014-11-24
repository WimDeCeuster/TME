using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Equipment;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Packs;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using CarAccessory = TME.CarConfigurator.Repository.Objects.Equipment.CarAccessory;
using CarEquipment = TME.CarConfigurator.Equipment.CarEquipment;
using CarOption = TME.CarConfigurator.Repository.Objects.Equipment.CarOption;
using Category = TME.CarConfigurator.Equipment.Category;
using GradeAccessory = TME.CarConfigurator.Equipment.GradeAccessory;
using GradeEquipment = TME.CarConfigurator.Equipment.GradeEquipment;
using GradeOption = TME.CarConfigurator.Equipment.GradeOption;
using RepoCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;
using RepoGradeAccessory = TME.CarConfigurator.Repository.Objects.Equipment.GradeAccessory;
using RepoGradeOption = TME.CarConfigurator.Repository.Objects.Equipment.GradeOption;

namespace TME.CarConfigurator.Factories
{
    public class EquipmentFactory : IEquipmentFactory
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IColourFactory _colourFactory;
        private readonly IAssetFactory _assetFactory;

        public EquipmentFactory(IEquipmentService equipmentService, IColourFactory colourFactory, IAssetFactory assetFactory)
        {
            if (equipmentService == null) throw new ArgumentNullException("equipmentService");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _equipmentService = equipmentService;
            _colourFactory = colourFactory;
            _assetFactory = assetFactory;
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

        public ICarEquipment GetCarEquipment(Guid carID, Publication publication, Context context)
        {
            var carEquipment = _equipmentService.GetCarEquipment(carID, publication.ID, context);
            return new CarEquipment(
                carEquipment.Accessories.Select(accessory => new Equipment.CarAccessory(accessory, carID, publication, context, _assetFactory)),
                carEquipment.Options.Select(option => GetCarOption(carID, publication, context, option, carEquipment.Options)));
        }

        public IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId)
        {
            var gradeEquipment = _equipmentService.GetGradeEquipment(publication.ID, publication.GetCurrentTimeFrame().ID, gradeId, context);

            return new GradeEquipment(
                gradeEquipment.Accessories.Select(accessory => GetGradeAccessory(accessory, publication, context)),
                gradeEquipment.Options.Select(option => GetGradeOption(option, gradeEquipment.Options, publication, context)));
        }

        public ICarPackEquipment GetCarPackEquipment(Repository.Objects.Packs.CarPackEquipment repoCarPackEquipment, Publication publication, Context context, Guid carId)
        {
            return new CarPackEquipment(
                repoCarPackEquipment.Accessories.Select(accessory => new CarPackAccessory(accessory, publication, carId, context, _assetFactory)),
                repoCarPackEquipment.Options.Select(option => new CarPackOption(option, publication, carId, context, _assetFactory)),
                repoCarPackEquipment.ExteriorColourTypes.Select(type => new CarPackExteriorColourType(type, publication, carId, context, _assetFactory)),
                repoCarPackEquipment.UpholsteryTypes.Select(type => new CarPackUpholsteryType(type, publication, carId, context, _assetFactory)));
        }

        ICarOption GetCarOption(Guid carID, Publication publication, Context context, CarOption option, IEnumerable<CarOption> repoCarOptions)
        {
            var parentCarOption = option.ParentOptionShortID == 0
                 ? null
                 : repoCarOptions.Single(grd => grd.ShortID == option.ParentOptionShortID);

            var parentOptionInfo = parentCarOption == null ? null : new OptionInfo(parentCarOption.ShortID, parentCarOption.Name);

            return new Equipment.CarOption(option, parentOptionInfo, carID, publication, context, _assetFactory);
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
            var repoCategories = _equipmentService.GetCategories(publication.ID, context).ToList();

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
