using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Rules;
using CarPack = TME.CarConfigurator.Administration.CarPack;
using EquipmentItemRule = TME.CarConfigurator.Repository.Objects.Rules.EquipmentItemRule;
using RuleCategory = TME.CarConfigurator.Repository.Objects.Enums.RuleCategory;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class RuleMapper : IRuleMapper
    {
        public RuleSets MapCarEquipmentRules(CarEquipmentRules rules)
        {
            return MapRuleSets(rules, rules.EquipmentItem.Car);
        }

        public RuleSets MapCarPackRules(CarPackRules rules)
        {
            return MapRuleSets(rules, rules.Pack.Car);
        }

        private static RuleSets MapRuleSets(CarRules rules, Car car)
        {
            var excludes = rules.Where(r => r.RuleType == RuleType.Exclude).ToList();
            var includes = rules.Where(r => r.RuleType == RuleType.Include).ToList();

            var carPackExcludeRules = excludes.OfType<CarPackRule>();
            var carEquipmentItemExcludeRules = excludes.OfType<CarEquipmentItemRule>().ToList();

            var carPackIncludeRules = includes.OfType<CarPackRule>();
            var carEquipmentItemIncludeRules = includes.OfType<CarEquipmentItemRule>().ToList();

            var carAccessoryExcludeRules = carEquipmentItemExcludeRules.Where(rule => car.Equipment[rule.ID].Type == EquipmentType.Accessory);
            var carOptionExcludeRules = carEquipmentItemExcludeRules.Where(rule => car.Equipment[rule.ID].Type == EquipmentType.Accessory);

            var carAccessoryIncludeRules = carEquipmentItemIncludeRules.Where(rule => car.Equipment[rule.ID].Type == EquipmentType.Accessory);
            var carOptionIncludeRules = carEquipmentItemIncludeRules.Where(rule => car.Equipment[rule.ID].Type == EquipmentType.Accessory);

            return new RuleSets
            {
                Exclude = new RuleSet
                {
                    PackRules = carPackExcludeRules.Select(carPackRule => MapCarPackRule(carPackRule, car.Packs[carPackRule.ID])).ToList(),
                    AccessoryRules = carAccessoryExcludeRules.Select(carEquipmentItemRule => MapCarEquipmentRule(carEquipmentItemRule, car.Equipment[carEquipmentItemRule.ID])).ToList(),
                    OptionRules = carOptionExcludeRules.Select(carEquipmentItemRule => MapCarEquipmentRule(carEquipmentItemRule, car.Equipment[carEquipmentItemRule.ID])).ToList()
                },
                Include = new RuleSet
                {
                    PackRules = carPackIncludeRules.Select(carPackRule => MapCarPackRule(carPackRule, car.Packs[carPackRule.ID])).ToList(),
                    AccessoryRules = carAccessoryIncludeRules.Select(carEquipmentItemRule => MapCarEquipmentRule(carEquipmentItemRule, car.Equipment[carEquipmentItemRule.ID])).ToList(),
                    OptionRules = carOptionIncludeRules.Select(carEquipmentItemRule => MapCarEquipmentRule(carEquipmentItemRule, car.Equipment[carEquipmentItemRule.ID])).ToList()}
            };
        }

        private static PackRule MapCarPackRule(CarRule carRule, CarPack pack)
        {
            return new PackRule
            {
                Name = carRule.Name,
                ShortID = pack.ShortID ?? 0,
                Category = MapRuleCategory(carRule.Category)
            };
        }

        private static EquipmentItemRule MapCarEquipmentRule(CarEquipmentItemRule carRule, CarEquipmentItem equipmentItem)
        {
            return new EquipmentItemRule
            {
                ShortID = equipmentItem.ShortID ?? 0,
                ColouringModes = carRule.ColouringMode.Convert(),
                Name = carRule.Name,
                Category = MapRuleCategory(carRule.Category)
            };
        }

        private static RuleCategory MapRuleCategory(Administration.Enums.RuleCategory category)
        {
            switch (category)
            {
                case Administration.Enums.RuleCategory.MARKETING :
                    return RuleCategory.Marketing;
                case Administration.Enums.RuleCategory.PRODUCT :
                    return RuleCategory.Product;
                case Administration.Enums.RuleCategory.VISUAL :
                    return RuleCategory.Visual;
                default :
                    throw new UnrecognisedRuleCategoryException(category);
            }
        }
    }
}