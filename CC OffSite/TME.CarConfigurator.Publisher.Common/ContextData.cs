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
    public class ContextData
    {
        public Publication Publication { get; set; }

        public IList<Generation> Generations { get; private set; }
        public IList<Model> Models { get; private set; }
        public IDictionary<Guid, IList<Asset>> Assets { get; set; }
        public IDictionary<Guid, IList<CarPart>> CarCarParts { get; private set; }
        public IDictionary<Guid, IReadOnlyList<CarPack>> CarPacks { get; private set; }
        public IDictionary<Guid, IDictionary<Guid, IList<Asset>>> CarAssets { get; private set; }
        public IDictionary<Guid, IDictionary<Guid, IList<Asset>>> SubModelAssets { get; private set; }

        public ContextData()
        {
            Models = new List<Model>();
            Generations = new List<Generation>();
            Assets = new Dictionary<Guid, IList<Asset>>();
            CarAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();
            SubModelAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();
            CarCarParts = new Dictionary<Guid, IList<CarPart>>();
            CarPacks = new Dictionary<Guid, IReadOnlyList<CarPack>>();
        }
    }
}
