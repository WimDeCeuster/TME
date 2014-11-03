using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Publisher.Common
{
    public class ContextData
    {
        public Publication Publication { get; set; }

        public IList<Car> Cars { get; private set; }
        public IList<Generation> Generations { get; private set; }
        public IList<Model> Models { get; private set; }
        public IList<BodyType> BodyTypes { get; private set; }
        public IList<Engine> Engines { get; private set; }
        public IList<Transmission> Transmissions { get; private set; }
        public IDictionary<Guid, IList<Asset>> Assets { get; set; }
        public IList<WheelDrive> WheelDrives { get; private set; }
        public IList<Steering> Steerings { get; private set; }
        public IList<Grade> Grades { get; private set; }
        public IList<SubModel> SubModels { get; private set; }
        public IDictionary<Guid, GradeEquipment> GradeEquipment { get; private set; }
        public IList<ColourCombination> ColourCombinations { get; private set; }
        public IList<Category> EquipmentCategories { get; private set; }
        public IDictionary<Guid, IDictionary<Guid, IList<Asset>>> CarAssets { get; private set; }
        public IDictionary<Guid, IList<GradePack>> GradePacks { get; private set; }

        public ContextData()
        {
            Cars = new List<Car>();
            Models = new List<Model>();
            Generations = new List<Generation>();
            BodyTypes = new List<BodyType>();
            Engines = new List<Engine>();
            Assets = new Dictionary<Guid, IList<Asset>>();
            Transmissions = new List<Transmission>();
            CarAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();
            WheelDrives = new List<WheelDrive>();
            Steerings = new List<Steering>();
            Grades = new List<Grade>();
            SubModels = new List<SubModel>();
            GradeEquipment = new Dictionary<Guid, GradeEquipment>();
            ColourCombinations = new List<ColourCombination>();
            GradePacks = new Dictionary<Guid, IList<GradePack>>();
            EquipmentCategories = new List<Category>();
        }
    }
}
