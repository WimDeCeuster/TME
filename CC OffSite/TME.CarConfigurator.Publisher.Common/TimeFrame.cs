using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
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
        public IReadOnlyList<Guid> TimeFrameCarIds { get; private set; }

        public IReadOnlyList<Car> Cars { get; set; }
        public IReadOnlyList<BodyType> BodyTypes { get; set; }
        public IReadOnlyList<Engine> Engines { get; set; }
        public IReadOnlyList<WheelDrive> WheelDrives { get; set; }
        public IReadOnlyList<Transmission> Transmissions { get; set; }
        public IReadOnlyList<Steering> Steerings { get; set; }
        public IReadOnlyList<Grade> Grades { get; set; }
        public IReadOnlyDictionary<Guid, GradeEquipment> GradeEquipments { get;set; }
        public IReadOnlyDictionary<Guid, IReadOnlyList<Grade>> SubModelGrades { get; set; }
        public IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>> GradePacks { get; set; }
        public IReadOnlyList<SubModel> SubModels { get; set; }
        public IReadOnlyList<ColourCombination> ColourCombinations { get; set; }
        public IReadOnlyList<EquipmentCategory> EquipmentCategories { get; set; }
        public IReadOnlyList<SpecificationCategory> SpecificationCategories { get; set; }
        public IReadOnlyDictionary<Guid, IReadOnlyDictionary<Guid, GradeEquipment>> SubModelGradeEquipments { get; set; }
        public IReadOnlyDictionary<Guid, IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>>> SubModelGradePacks { get; set; }

        public readonly Guid ID;

        public TimeFrame(DateTime from, DateTime until, IEnumerable<Guid> timeFrameCarIds)
        {
            if (timeFrameCarIds == null) throw new ArgumentNullException("timeFrameCarIds");

            ID = Guid.NewGuid();
            From = from;
            Until = until;

            TimeFrameCarIds = timeFrameCarIds.ToList();

            GradePacks = new Dictionary<Guid, IReadOnlyList<GradePack>>();
            ColourCombinations = new List<ColourCombination>();
            Cars = new List<Car>();
            BodyTypes = new List<BodyType>();
            Engines = new List<Engine>();
            WheelDrives = new List<WheelDrive>();
            Transmissions = new List<Transmission>();
            Steerings = new List<Steering>();
            Grades = new List<Grade>();
            SubModelGrades = new Dictionary<Guid, IReadOnlyList<Grade>>();
            GradeEquipments = new Dictionary<Guid, GradeEquipment>();
            SubModels = new List<SubModel>();
            SubModelGradeEquipments = new Dictionary<Guid, IReadOnlyDictionary<Guid, GradeEquipment>>();
            EquipmentCategories = new List<EquipmentCategory>();
            SpecificationCategories = new List<SpecificationCategory>();
            SubModelGradePacks = new Dictionary<Guid, IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>>>();
        }
    }
}
