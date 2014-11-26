using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;
using EquipmentCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;
using SpecificationCategory = TME.CarConfigurator.Repository.Objects.TechnicalSpecifications.Category;

namespace TME.CarConfigurator.Publisher.Common
{
    public class ContextData
    {
        public Publication Publication { get; set; }

        public IList<Generation> Generations { get; private set; }
        public IList<Model> Models { get; private set; }
        public IList<EquipmentCategory> EquipmentCategories { get; set; }
        public IList<SpecificationCategory> SpecificationCategories { get; set; }
        public IDictionary<Guid, IList<Asset>> Assets { get; set; }
        public IDictionary<Guid, CarEquipment> CarEquipment { get; private set; }
        public IDictionary<Guid, IList<CarPart>> CarParts { get; private set; }
        public IDictionary<Guid, IReadOnlyList<CarPack>> CarPacks { get; private set; }
        public IDictionary<Guid, IReadOnlyList<CarTechnicalSpecification>> CarTechnicalSpecifications { get; private set; }
        public IDictionary<Guid, IDictionary<Guid, IList<Asset>>> CarAssets { get; private set; }
        public IDictionary<Guid, IDictionary<Guid, IList<Asset>>> SubModelAssets { get; private set; }
        public IDictionary<Guid, IDictionary<Guid, IList<Asset>>> CarEquipmentAssets { get; private set; }
        public IDictionary<Guid, IDictionary<Guid, IList<Asset>>> CarPartAssets { get; private set; }
        public IDictionary<Guid, IList<CarColourCombination>> CarColourCombinations { get; set; }
        public IDictionary<Guid, List<Asset>> CarColourCombinationAssets { get; private set; }

        public ContextData()
        {
            Models = new List<Model>();
            Generations = new List<Generation>();
            EquipmentCategories = new List<EquipmentCategory>();
            SpecificationCategories = new List<SpecificationCategory>();
            Assets = new Dictionary<Guid, IList<Asset>>();
            CarAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();
            SubModelAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();
            CarEquipment = new Dictionary<Guid, CarEquipment>();
            CarParts = new Dictionary<Guid, IList<CarPart>>();
            CarEquipmentAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();
            CarPartAssets = new Dictionary<Guid, IDictionary<Guid, IList<Asset>>>();
            CarPacks = new Dictionary<Guid, IReadOnlyList<CarPack>>();
            CarTechnicalSpecifications = new Dictionary<Guid, IReadOnlyList<CarTechnicalSpecification>>();
            CarColourCombinations = new Dictionary<Guid, IList<CarColourCombination>>();
            CarColourCombinationAssets = new Dictionary<Guid, List<Asset>>();
        }

    }
}
