using System.Linq;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.LegacyAdapter.Equipment;
using TME.CarConfigurator.LegacyAdapter.Rules;

namespace TME.CarConfigurator.LegacyAdapter.Packs
{
    public class CarPackRules : IRuleSets
    {
        private readonly RuleSet _include = new RuleSet();
        private readonly RuleSet _exclude = new RuleSet();

        public CarPackRules(TMME.CarConfigurator.CarPack adaptee, TMME.CarConfigurator.Car carOfAdaptee)
        {

            SetupIncludeRules(adaptee);
            SetupExcludeRules(adaptee, carOfAdaptee);
        }

        private void SetupIncludeRules(TMME.CarConfigurator.CarPack adaptee)
        {
            _include.Packs = adaptee.IncludePacks.Cast<TMME.CarConfigurator.CarPack>().Select(x => new PackRule(x)).ToList();
            _include.Accessories = adaptee.IncludeEquipment.OfType<TMME.CarConfigurator.CarAccessory>().Select(x => new EquipmentRule(x)).ToList();
            _include.Options = adaptee.IncludeEquipment.OfType<TMME.CarConfigurator.CarOption>().Select(x => new EquipmentRule(x)).ToList();
        }
        private void SetupExcludeRules(TMME.CarConfigurator.CarPack adaptee, TMME.CarConfigurator.Car carOfAdaptee)
        {
            var packs = adaptee.ExcludePacks.Cast<TMME.CarConfigurator.CarPack>();
            var accessories = adaptee.ExcludeEquipment.OfType<TMME.CarConfigurator.CarAccessory>();
            var options = adaptee.ExcludeEquipment.OfType<TMME.CarConfigurator.CarOption>();

            var packsThatExcludeMe = carOfAdaptee.Packs.Cast<TMME.CarConfigurator.CarPack>().Where(x => x.ExcludePacks.Contains(adaptee.ID));
            var accessoriesThatExcludeMe = carOfAdaptee.Equipment.OfType<TMME.CarConfigurator.CarAccessory>().Where(x => x.ExcludePacks.Contains(adaptee.ID));
            var optionsThatExcludeMe = carOfAdaptee.Equipment.OfType<TMME.CarConfigurator.CarOption>().Where(x => x.ExcludePacks.Contains(adaptee.ID));

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