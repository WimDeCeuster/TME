using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;

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
        public IReadOnlyDictionary<Guid, IReadOnlyList<GradeAccessory>> GradeAccessories { get; private set; }
        public IReadOnlyDictionary<Guid, IReadOnlyList<GradeOption>> GradeOptions { get; private set; }
        public IReadOnlyList<SubModel> SubModels { get; private set; }

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
            IReadOnlyDictionary<Guid, IReadOnlyList<GradeAccessory>> gradeAccessories,
            IReadOnlyDictionary<Guid, IReadOnlyList<GradeOption>> gradeOptions,
            IReadOnlyList<SubModel> subModels)
        {
            if (cars == null) throw new ArgumentNullException("cars");
            if (bodyTypes == null) throw new ArgumentNullException("bodyTypes");
            if (engines == null) throw new ArgumentNullException("engines");
            if (wheelDrives == null) throw new ArgumentNullException("wheelDrives");
            if (transmissions == null) throw new ArgumentNullException("transmissions");
            if (steerings == null) throw new ArgumentNullException("steerings");
            if (grades == null) throw new ArgumentNullException("grades");
            if (gradeAccessories == null) throw new ArgumentNullException("gradeAccessories");
            if (gradeOptions == null) throw new ArgumentNullException("gradeOptions");
            if (subModels == null) throw new ArgumentNullException("subModels");

            From = from;
            Until = until;
            Cars = cars;
            BodyTypes = bodyTypes;
            Engines = engines;
            WheelDrives = wheelDrives;
            Transmissions = transmissions;
            Steerings = steerings;
            Grades = grades;
            GradeAccessories = gradeAccessories;
            GradeOptions = gradeOptions;
            SubModels = subModels;

            ID = Guid.NewGuid();
        }
    }
}
