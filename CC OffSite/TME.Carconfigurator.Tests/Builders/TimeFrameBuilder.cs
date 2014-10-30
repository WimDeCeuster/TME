using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;

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
        private readonly Dictionary<Guid, GradeEquipment> _gradeEquipments = new Dictionary<Guid, GradeEquipment>();
        private readonly Dictionary<Guid, IList<GradePack>> _gradePacks = new Dictionary<Guid, IList<GradePack>>();
        private List<Transmission> _transmissions = new List<Transmission>();
        private List<SubModel> _subModels = new List<SubModel>();
        private List<ColourCombination> _colourCombinations = new List<ColourCombination>();

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

        public TimeFrameBuilder WithGradeEquipment(Guid gradeId, GradeEquipment gradeEquipment)
        {
            _gradeEquipments.Add(gradeId, gradeEquipment);
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

        public TimeFrameBuilder WithGradePacks(Guid gradeId, IList<GradePack> gradePacks)
        {
            _gradePacks.Add(gradeId, gradePacks);
            return this;
        }

        public TimeFrameBuilder WithColourCombinations(IEnumerable<ColourCombination> colourCombinations)
        {
            _colourCombinations = colourCombinations.ToList();
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
                _gradeEquipments,
                _gradePacks,
                _subModels,
                _colourCombinations);
        }
    }
}