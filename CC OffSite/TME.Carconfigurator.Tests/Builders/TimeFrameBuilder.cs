using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class TimeFrameBuilder
    {
        private DateTime _from;
        private DateTime _until;
        private List<Car> _cars = new List<Car>();
        private List<BodyType> _bodyTypes = new List<BodyType>();
        private List<Engine> _engines = new List<Engine>();
        private List<WheelDrive> _wheelDrives = new List<WheelDrive>();
        private List<Steering> _steerings = new List<Steering>();
        private List<Grade> _grades = new List<Grade>();
        private Dictionary<Guid, IReadOnlyList<GradeAccessory>> _gradeAccessories = new Dictionary<Guid, IReadOnlyList<GradeAccessory>>();
        private Dictionary<Guid, IReadOnlyList<GradeOption>> _gradeOptions = new Dictionary<Guid, IReadOnlyList<GradeOption>>();
        private List<Transmission> _transmissions = new List<Transmission>();
        private List<SubModel> _subModels = new List<SubModel>();

        public TimeFrameBuilder WithDateRange(DateTime from, DateTime until)
        {
            _from = from;
            _until = until;

            return this;
        }

        public TimeFrameBuilder WithCars(IEnumerable<Car> cars)
        {
            _cars = cars.ToList();
            return this;
        }

        public TimeFrameBuilder WithBodyTypes(IEnumerable<BodyType> bodyTypes)
        {
            _bodyTypes = bodyTypes.ToList();
            return this;
        }

        public TimeFrameBuilder WithEngines(IEnumerable<Engine> engines)
        {
            _engines = engines.ToList();
            return this;
        }

        public TimeFrameBuilder WithWheelDrives(IEnumerable<WheelDrive> wheelDrives)
        {
            _wheelDrives = wheelDrives.ToList();
            return this;
        }

        public TimeFrameBuilder WithSteerings(IEnumerable<Steering> steerings)
        {
            _steerings = steerings.ToList();
            return this;
        }

        public TimeFrameBuilder WithGrades(IEnumerable<Grade> grades)
        {
            _grades = grades.ToList();
            return this;
        }

        public TimeFrameBuilder WithGradeAccessories(Guid gradeId, IEnumerable<GradeAccessory> gradeAccessories)
        {
            _gradeAccessories.Add(gradeId, gradeAccessories.ToList());
            return this;
        }

        public TimeFrameBuilder WithGradeOptions(Guid gradeId, IEnumerable<GradeOption> gradeOptions)
        {
            _gradeOptions.Add(gradeId, gradeOptions.ToList());
            return this;
        }

        public TimeFrameBuilder WithTransmissions(IEnumerable<Transmission> transmissions)
        {
            _transmissions = transmissions.ToList();
            return this;
        }

        public TimeFrameBuilder WithSubModels(IEnumerable<SubModel> subModels)
        {
            _subModels = subModels.ToList();
            return this;
        }

        public TimeFrame Build()
        {
            return new TimeFrame(
                _from,
                _until,
                _cars,
                _bodyTypes,
                _engines,
                _wheelDrives,
                _transmissions,
                _steerings,
                _grades,
                _gradeAccessories,
                _gradeOptions,
                _subModels);
        }
    }
}