using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using EquipmentCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;
using SpecificationCategory = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.Category;

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
        private readonly Dictionary<Guid, IReadOnlyList<GradePack>> _gradePacks = new Dictionary<Guid, IReadOnlyList<GradePack>>();
        private readonly Dictionary<Guid, IList<Grade>> _subModelGrades = new Dictionary<Guid, IList<Grade>>();
        private readonly Dictionary<Guid, IDictionary<Guid, GradeEquipment>> _subModelGradeEquipments = new Dictionary<Guid, IDictionary<Guid, GradeEquipment>>();
        private readonly Dictionary<Guid, IDictionary<Guid, IReadOnlyList<GradePack>>> _subModelGradePacks = new Dictionary<Guid, IDictionary<Guid, IReadOnlyList<GradePack>>>();
        private List<Transmission> _transmissions = new List<Transmission>();
        private List<SubModel> _subModels = new List<SubModel>();
        private List<ColourCombination> _colourCombinations = new List<ColourCombination>();
        private List<EquipmentCategory> _equipmentCategories = new List<EquipmentCategory>();
        private List<SpecificationCategory> _specificationCategories = new List<SpecificationCategory>();
        private readonly Dictionary<Guid, IDictionary<Guid, IList<Asset>>> _subModelAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();

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

        public TimeFrameBuilder WithSubModelGradeEquipment(Guid submodelID,Guid gradeId, GradeEquipment gradeEquipment)
        {
            _subModelGradeEquipments.Add(submodelID,new Dictionary<Guid, GradeEquipment>(){{gradeId,gradeEquipment}});
            return this;
        }

        public TimeFrameBuilder WithSubModelGradePacks(Guid submodelID, Guid gradeId, params GradePack[] packs)
        {
            if (!_subModelGradePacks.ContainsKey(submodelID))
                _subModelGradePacks.Add(submodelID, new Dictionary<Guid, IReadOnlyList<GradePack>>());

            _subModelGradePacks[submodelID].Add(gradeId, packs);
            return this;
        }

        public TimeFrameBuilder WithSubModelAssets(Guid subModelID, Guid objectID, IEnumerable<Asset> assets)
        {
            _subModelAssets.Add(subModelID,new Dictionary<Guid, IList<Asset>>(){{objectID,assets.ToList()}});
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

        public TimeFrameBuilder WithGradePacks(Guid gradeId, IReadOnlyList<GradePack> gradePacks)
        {
            _gradePacks.Add(gradeId, gradePacks);
            return this;
        }

        public TimeFrameBuilder WithColourCombinations(IEnumerable<ColourCombination> colourCombinations)
        {
            _colourCombinations = colourCombinations.ToList();
            return this;
        }

        public TimeFrameBuilder WithSubModelGrades(SubModel subModel, IReadOnlyList<Grade> grades)
        {
            _subModelGrades.Add(subModel.ID, grades.ToList());
            return this;
        }
               	
	    public TimeFrameBuilder WithEquipmentCategories(params Category[] categories)
        {
            _equipmentCategories = categories.ToList();
            return this;
        }

        public TimeFrameBuilder WithSpecificationCategories(params SpecificationCategory[] categories)
        {
            _specificationCategories = categories.ToList();
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
                _subModelGrades,
                _gradePacks,
                _subModels,
                _colourCombinations,
                _equipmentCategories,
                _subModelGradeEquipments,
                _specificationCategories,
                _subModelAssets,
                _subModelGradePacks.ToDictionary(
                    entry => entry.Key,
                    entry => (IReadOnlyDictionary<Guid, IReadOnlyList<GradePack>>)entry.Value.ToDictionary()
                ));
        }
    }
}