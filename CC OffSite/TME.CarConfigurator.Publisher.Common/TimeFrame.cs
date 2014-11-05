using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using EquipmentCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;
using SpecificationCategory = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.Category;

namespace TME.CarConfigurator.Publisher.Common
{
    public class TimeFrame
    {
        public DateTime From { get; private set; }
        public DateTime Until;

        public IReadOnlyList<Car> Cars { get; private set; }
        public IReadOnlyList<BodyType> BodyTypes { get; private set; }
        public IReadOnlyList<Engine> Engines { get; private set; }
        public IReadOnlyList<WheelDrive> WheelDrives { get; private set; }
        public IReadOnlyList<Transmission> Transmissions { get; private set; }
        public IReadOnlyList<Steering> Steerings { get; private set; }
        public IReadOnlyList<Grade> Grades { get; private set; }
        public IReadOnlyDictionary<Guid, GradeEquipment> GradeEquipments { get; private set; }
        public IReadOnlyDictionary<Guid, IList<Grade>> SubModelGrades { get; private set; }
        public IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>> GradePacks { get; private set; }
        public IReadOnlyList<SubModel> SubModels { get; private set; }
        public IReadOnlyList<ColourCombination> ColourCombinations { get; private set; }
        public IReadOnlyList<EquipmentCategory> EquipmentCategories { get; private set; }
        public IReadOnlyList<SpecificationCategory> SpecificationCategories { get; set; }
        public IReadOnlyDictionary<Guid,IDictionary<Guid,IList<Asset>>> SubModelAssets { get; set; }
        public IDictionary<Guid, IDictionary<Guid, GradeEquipment>> SubModelGradeEquipments { get; set; }
        public IReadOnlyDictionary<Guid, IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>>> SubModelGradePacks { get; set; }

        public readonly Guid ID;

        public TimeFrame(
            DateTime from,
            DateTime until,
            IReadOnlyList<Car> cars,
            IReadOnlyList<BodyType> bodyTypes,
            IReadOnlyList<Engine> engines,
            IReadOnlyList<WheelDrive> wheelDrives,
            IReadOnlyList<Transmission> transmissions,
            IReadOnlyList<Steering> steerings,
            IReadOnlyList<Grade> grades,
            IReadOnlyDictionary<Guid, GradeEquipment> gradeEquipments,
            IReadOnlyDictionary<Guid, IList<Grade>> subModelGrades,
            IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>> gradePacks,
            IReadOnlyList<SubModel> subModels,
            IReadOnlyList<ColourCombination> colourCombinations,
            IReadOnlyList<EquipmentCategory> equipmentCategories,
            IDictionary<Guid, IDictionary<Guid, GradeEquipment>> subModelGradeEquipments,
            IReadOnlyList<SpecificationCategory> specificationCategories,
            IReadOnlyDictionary<Guid, IDictionary<Guid, IList<Asset>>> subModelAssets,
            IReadOnlyDictionary<Guid, IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>>> subModelGradePacks)
        {
            if (cars == null) throw new ArgumentNullException("cars");
            if (bodyTypes == null) throw new ArgumentNullException("bodyTypes");
            if (engines == null) throw new ArgumentNullException("engines");
            if (wheelDrives == null) throw new ArgumentNullException("wheelDrives");
            if (transmissions == null) throw new ArgumentNullException("transmissions");
            if (steerings == null) throw new ArgumentNullException("steerings");
            if (grades == null) throw new ArgumentNullException("grades");
            if (gradeEquipments == null) throw new ArgumentNullException("gradeEquipments");
            if (subModelGrades == null) throw new ArgumentNullException("subModelGrades");
            if (gradePacks == null) throw new ArgumentNullException("gradePacks");
            if (subModels == null) throw new ArgumentNullException("subModels");
            if (colourCombinations == null) throw new ArgumentNullException("colourCombinations");
            if (subModelGradeEquipments == null) throw new ArgumentNullException("subModelGradeEquipments");
            if (equipmentCategories == null) throw new ArgumentNullException("equipmentCategories");
            if (specificationCategories == null) throw new ArgumentNullException("specificationCategories");
            if (subModelAssets == null) throw new ArgumentNullException("subModelAssets");
            if (subModelGradePacks == null) throw new ArgumentNullException("subModelGradePacks");

            From = from;
            Until = until;
            GradePacks = gradePacks;
            ColourCombinations = colourCombinations;
            Cars = cars;
            BodyTypes = bodyTypes;
            Engines = engines;
            WheelDrives = wheelDrives;
            Transmissions = transmissions;
            Steerings = steerings;
            Grades = grades;
            SubModelGrades = subModelGrades;
            GradeEquipments = gradeEquipments;
            SubModelAssets = subModelAssets;
            SubModels = subModels;
            SubModelGradeEquipments = subModelGradeEquipments;
            EquipmentCategories = equipmentCategories;
            SpecificationCategories = specificationCategories;
            SubModelGradePacks = subModelGradePacks;

            ID = Guid.NewGuid();
        }


    }
}
