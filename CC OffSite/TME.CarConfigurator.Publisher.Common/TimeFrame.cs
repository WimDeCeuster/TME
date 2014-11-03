using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using BodyType = TME.CarConfigurator.Repository.Objects.BodyType;
using Car = TME.CarConfigurator.Repository.Objects.Car;
using Engine = TME.CarConfigurator.Repository.Objects.Engine;
using Steering = TME.CarConfigurator.Repository.Objects.Steering;
using Transmission = TME.CarConfigurator.Repository.Objects.Transmission;
using WheelDrive = TME.CarConfigurator.Repository.Objects.WheelDrive;

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
        public IDictionary<Guid, IDictionary<Guid, GradeEquipment>> SubModelGradeEquipments { get; private set; }
        public IReadOnlyDictionary<Guid, IList<GradePack>> GradePacks { get; private set; }
        public IReadOnlyList<SubModel> SubModels { get; private set; }
        public IReadOnlyList<ColourCombination> ColourCombinations { get; private set; }

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
            IReadOnlyDictionary<Guid, IList<GradePack>> gradePacks,
            IReadOnlyList<SubModel> subModels,
            IReadOnlyList<ColourCombination> colourCombinations,
            IDictionary<Guid, IDictionary<Guid, GradeEquipment>> subModelGradeEquipments)
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
            SubModels = subModels;
            SubModelGradeEquipments = subModelGradeEquipments;

            ID = Guid.NewGuid();
        }
    }
}
