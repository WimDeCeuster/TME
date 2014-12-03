using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Enums;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Extensions;
using TME.CarConfigurator.Repository.Objects.Rules;
using CarPack = TME.CarConfigurator.Administration.CarPack;
using EquipmentItemRule = TME.CarConfigurator.Repository.Objects.Rules.EquipmentItemRule;
using RuleCategory = TME.CarConfigurator.Repository.Objects.Enums.RuleCategory;
using Rule = TME.CarConfigurator.Administration.Rule;

namespace TME.CarConfigurator.Publisher.Mappers
{
    class RuleInfo
    {
        public RuleType Type { get; set; }
        public Guid Target { get; set; }
        public Administration.Enums.RuleCategory Category { get; set; }
        public ColouringModes ColouringModes { get; set; }
    }

    public class RuleMapper : IRuleMapper
    {
        private static readonly Helpers.Comparer<RuleInfo> RuleInfoComparer = new Helpers.Comparer<RuleInfo>(rule => rule.Target);

        public RuleSets MapCarEquipmentRules(CarEquipmentItem equipmentItem, EquipmentGroups equipmentGroups, EquipmentItems equipmentItems)
        {
            var equipmentItemGroup = equipmentGroups.Find(equipmentItem.Group.ID);
            var groupRules = equipmentItemGroup.Rules;
            var itemRules = equipmentItem.Rules;
            var crmItemRules = equipmentItems[equipmentItem.ID].Rules;
            var orderedGroups = equipmentGroups.Concat(equipmentGroups.SelectMany(eg => eg.Groups.Flatten(subGroup => subGroup.Groups))).ToList();



            var groupRuleInfo = UnrollGroupRules(equipmentItemGroup.Rules, orderedGroups, equipmentItem, equipmentGroups);

            var crmRulesResult = ApplyRules(groupRuleInfo, UnrollGroupRules(crmItemRules, orderedGroups, equipmentItem, equipmentGroups));

            var equipmentItemRulesInfo = itemRules.OfType<CarEquipmentItemRule>().Where(item => EquipmentItemIsApplicable(equipmentItem.Car, item.ID)).Select(GetRuleInfo).ToList();

            var clearedTargets = itemRules.Where(rule => rule.Cleared).Select(rule => rule.ID).ToList();

            var result = ApplyRules(crmRulesResult, equipmentItemRulesInfo).Where(rule => !clearedTargets.Contains(rule.Target)).ToList();

            var includeRules = result.Where(rule => rule.Type == RuleType.Include);
            var excludeRules = result.Where(rule => rule.Type == RuleType.Exclude);

            var packRules = itemRules.OfType<CarPackRule>().Where(rule => !rule.Cleared).Where(rule => CarPackIsApplicable(equipmentItem.Car, rule.ID)).ToList();

            var includePackRules = packRules.Where(rule => rule.RuleType == RuleType.Include);
            var excludePackRules = packRules.Where(rule => rule.RuleType == RuleType.Exclude);

            var accessoryRules = result.Where(rule => equipmentItem.Car.Equipment[rule.Target].Type == EquipmentType.Accessory).ToList();
            var optionRules = result.Where(rule => equipmentItem.Car.Equipment[rule.Target].Type == EquipmentType.Option).ToList();

            var accessoryIncludeRules = accessoryRules.Where(rule => rule.Type == RuleType.Include).ToList();
            var optionIncludeRules = optionRules.Where(rule => rule.Type == RuleType.Include).ToList();

            var accessoryExcludeRules = accessoryRules.Where(rule => rule.Type == RuleType.Exclude).ToList();
            var optionExcludeRules = optionRules.Where(rule => rule.Type == RuleType.Exclude).ToList();

            return new RuleSets
            {
                Include = new RuleSet
                {
                    AccessoryRules = accessoryIncludeRules.Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    OptionRules = optionIncludeRules.Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    PackRules = includePackRules.Select(rule => MapPackRule(rule, equipmentItem.Car)).ToList()
                },
                Exclude = new RuleSet
                {
                    AccessoryRules = accessoryExcludeRules.Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    OptionRules = optionExcludeRules.Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    PackRules = excludePackRules.Select(rule => MapPackRule(rule, equipmentItem.Car)).ToList()
                }
            };
        }

        private PackRule MapPackRule(CarPackRule rule, Car car)
        {
            var pack = car.Packs[rule.ID];

            return new PackRule
            {
                Category = rule.Category.Convert(),
                Name = pack.Translation.Name.DefaultIfEmpty(pack.Name),
                ShortID = pack.ShortID ?? 0
            };
        }

        private EquipmentItemRule MapRuleInfo(RuleInfo info, Car car)
        {
            var item = car.Equipment[info.Target];

            return new EquipmentItemRule
            {
                Category = info.Category.Convert(),
                ColouringModes = info.ColouringModes.Convert(),
                Name = item.Translation.Name.DefaultIfEmpty(item.Name),
                ShortID = item.ShortID ?? 0
            };
        }

        private static bool EquipmentItemIsApplicable(Car car, Guid equipmentItemId)
        {
            return car.Equipment[equipmentItemId] != null && car.Equipment[equipmentItemId].Availability != Availability.NotAvailable;
        }

        private static bool CarPackIsApplicable(Car car, Guid carPackId)
        {
            return car.Packs[carPackId] != null && car.Packs[carPackId].Availability != Availability.NotAvailable;
        }



        private static List<RuleInfo> ApplyRules(IList<RuleInfo> existingRules, IList<RuleInfo> newRules)
        {
            var existingIncludes = existingRules.Where(rule => rule.Type == RuleType.Include).ToList();
            var existingExcludes = existingRules.Where(rule => rule.Type == RuleType.Exclude).ToList();
            var newIncludes = newRules.Where(rule => rule.Type == RuleType.Include).ToList();
            var newExcludes = newRules.Where(rule => rule.Type == RuleType.Exclude).ToList();

            var resultIncludes = existingIncludes.Except(newExcludes).Concat(newIncludes);
            var resultExcludes = existingExcludes.Except(newIncludes).Concat(newExcludes);

            return resultIncludes.Concat(resultExcludes).ToList();
        }

        private static RuleInfo GetRuleInfo(Rule rule, Guid targetId)
        {
            return new RuleInfo { Category = rule.Category, Type = rule.Type, Target = targetId };
        }

        private static RuleInfo GetRuleInfo(Rule rule)
        {
            return new RuleInfo { Category = rule.Category, Type = rule.Type, Target = rule.ID };
        }

        private static RuleInfo GetRuleInfo(CarEquipmentItemRule rule)
        {
            return new RuleInfo { Category = rule.Category, Type = rule.RuleType, Target = rule.ID, ColouringModes = rule.ColouringMode };
        }

        private static IList<RuleInfo> GetGroupRules(CarEquipmentItem equipmentItem, EquipmentGroupRule groupRule, EquipmentGroups equipmentGroups)
        {
            var group = equipmentGroups.Find(groupRule.ID);
            var groupItemRules = group.Equipment.Where(item => EquipmentItemIsApplicable(equipmentItem.Car, item.ID)).Select(item => GetRuleInfo(groupRule, item.ID)).ToList();
            return groupItemRules.ToList();
        }

        private static List<RuleInfo> MergeGroupRules(IList<RuleInfo> rules, EquipmentGroupRule groupRule, CarEquipmentItem equipmentItem, EquipmentGroups equipmentGroups)
        {
            return ApplyRules(rules, GetGroupRules(equipmentItem, groupRule, equipmentGroups));
        }

        private static IList<RuleInfo> UnrollGroupRules(IList<Rule> rules, List<EquipmentGroup> orderedGroups, CarEquipmentItem equipmentItem, EquipmentGroups equipmentGroups)
        {
            var orderedGroupRules = rules.OfType<EquipmentGroupRule>().OrderBy(rule => orderedGroups.FindIndex(subGroup => subGroup.ID == rule.ID));

            var groupRulesResult = orderedGroupRules.Aggregate(new List<RuleInfo>(), (rls, groupRule) => MergeGroupRules(rls, groupRule, equipmentItem, equipmentGroups));

            var groupItemRules = rules.OfType<Administration.EquipmentItemRule>().Where(item => EquipmentItemIsApplicable(equipmentItem.Car, item.ID)).Select(GetRuleInfo).ToList();

            return ApplyRules(groupRulesResult, groupItemRules);
        }



















        
        //public RuleSets MapCarEquipmentRules(CarEquipmentRules rules)
        //{
        //    return MapRuleSets(rules, rules.EquipmentItem.Car);
        //}

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
            var carOptionExcludeRules = carEquipmentItemExcludeRules.Where(rule => car.Equipment[rule.ID].Type == EquipmentType.Option);

            var carAccessoryIncludeRules = carEquipmentItemIncludeRules.Where(rule => car.Equipment[rule.ID].Type == EquipmentType.Accessory);
            var carOptionIncludeRules = carEquipmentItemIncludeRules.Where(rule => car.Equipment[rule.ID].Type == EquipmentType.Option);

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
                Category = carRule.Category.Convert()
            };
        }

        private static EquipmentItemRule MapCarEquipmentRule(CarEquipmentItemRule carRule, CarEquipmentItem equipmentItem)
        {
            return new EquipmentItemRule
            {
                ShortID = equipmentItem.ShortID ?? 0,
                ColouringModes = carRule.ColouringMode.Convert(),
                Name = equipmentItem.Translation.Name ?? carRule.Name,
                Category = carRule.Category.Convert()
            };
        }
    }
}