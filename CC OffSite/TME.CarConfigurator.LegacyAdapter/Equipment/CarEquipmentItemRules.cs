using System.Linq;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.LegacyAdapter.Packs;
using TME.CarConfigurator.LegacyAdapter.Rules;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter.Equipment
{
    public class CarEquipmentItemRules : IRuleSets
    {
        private readonly RuleSet _include = new RuleSet();
        private readonly RuleSet _exclude = new RuleSet();

        public CarEquipmentItemRules(Legacy.CarEquipmentItem adaptee, Legacy.Car carOfAdaptee)
        {

            SetupIncludeRules(adaptee);
            SetupExcludeRules(adaptee, carOfAdaptee);
        }

        private void SetupIncludeRules(Legacy.CarEquipmentItem adaptee)
        {
            _include.Packs = adaptee.IncludePacks.Cast<Legacy.CarPack>().Select(x => new PackRule(x)).ToList();
            _include.Accessories = adaptee.IncludeEquipment.OfType<Legacy.CarAccessory>().Select(x => new EquipmentRule(x)).ToList();
            _include.Options = adaptee.IncludeEquipment.OfType<Legacy.CarOption>().Select(x => new EquipmentRule(x)).ToList();
        }
        private void SetupExcludeRules(Legacy.CarEquipmentItem adaptee, TMME.CarConfigurator.Car carOfAdaptee)
        {
            var packs = adaptee.ExcludePacks.Cast<Legacy.CarPack>();
            var accessories = adaptee.ExcludeEquipment.OfType<Legacy.CarAccessory>();
            var options = adaptee.ExcludeEquipment.OfType<Legacy.CarOption>();

            var packsThatExcludeMe = carOfAdaptee.Packs.Cast<Legacy.CarPack>().Where(x => x.ExcludeEquipment.Contains(adaptee.ID));
            var accessoriesThatExcludeMe = carOfAdaptee.Equipment.OfType<Legacy.CarAccessory>().Where(x => x.ExcludeEquipment.Contains(adaptee.ID));
            var optionsThatExcludeMe = carOfAdaptee.Equipment.OfType<Legacy.CarOption>().Where(x => x.ExcludeEquipment.Contains(adaptee.ID));

            _exclude.Packs = packs.Union(packsThatExcludeMe).Select(x => new PackRule(x)).ToList();
            _exclude.Accessories = accessories.Union(accessoriesThatExcludeMe).Select(x => new EquipmentRule(x)).ToList();
            _exclude.Options = options.Union(optionsThatExcludeMe).Select(x => new EquipmentRule(x)).ToList();
        }


        public IRuleSet Include
        {
            get { return _include; }
        }
        public IRuleSet Exclude
        {
            get { return _exclude; }
        }
    }
}