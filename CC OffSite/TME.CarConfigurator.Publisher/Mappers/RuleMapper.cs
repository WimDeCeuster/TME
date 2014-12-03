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

        public IDictionary<Guid, RuleSets> MapCarRules(Car car, EquipmentGroups equipmentGroups, EquipmentItems equipmentItems)
        {
            var packRules = car.Packs.Where(pack => pack.Availability != Availability.NotAvailable)
                .ToDictionary(pack => pack, pack => MapCarPackRules(pack.Rules));
            var equipmentRules = car.Equipment.Where(eq => eq.Availability != Availability.NotAvailable)
                .ToDictionary(carEquipmentItem => carEquipmentItem, carEquipmentItem => MapCarEquipmentRules(carEquipmentItem, equipmentGroups, equipmentItems));

            Func<Repository.Objects.Rules.EquipmentItemRule, Administration.CarEquipmentItem, Repository.Objects.Rules.EquipmentItemRule> CopyEquipmentToEquipmentRule = (rule, item) => new Repository.Objects.Rules.EquipmentItemRule
            {
                Category = rule.Category,
                ColouringModes = rule.ColouringModes,
                Name = item.Translation.Name.DefaultIfEmpty(item.Name),
                ShortID = item.ShortID ?? 0
            };

            Func<Repository.Objects.Rules.PackRule, Administration.CarEquipmentItem, Repository.Objects.Rules.EquipmentItemRule> CopyPackToEquipmentRule = (rule, item) => new Repository.Objects.Rules.EquipmentItemRule
            {
                Category = rule.Category,
                ColouringModes = Repository.Objects.Enums.ColouringModes.None,
                Name = item.Translation.Name.DefaultIfEmpty(item.Name),
                ShortID = item.ShortID ?? 0
            };

            Func<Repository.Objects.Rules.EquipmentItemRule, Administration.CarPack, Repository.Objects.Rules.PackRule> CopyEquipmentToPackRule = (rule, pack) => new Repository.Objects.Rules.PackRule
            {
                Category = rule.Category,
                Name = pack.Translation.Name.DefaultIfEmpty(pack.Name),
                ShortID = pack.ShortID ?? 0
            };

            Func<Repository.Objects.Rules.PackRule, Administration.CarPack, Repository.Objects.Rules.PackRule> CopyPackToPackRule = (rule, pack) => new Repository.Objects.Rules.PackRule
            {
                Category = rule.Category,
                Name = pack.Translation.Name.DefaultIfEmpty(pack.Name),
                ShortID = pack.ShortID ?? 0
            };

            foreach (var entry in packRules)
            {
                var sourceShortId = entry.Key.ShortID ?? 0;

                foreach (var excludeRule in entry.Value.Exclude.AccessoryRules)
                {
                    var targetRules = equipmentRules.First(targetEntry => targetEntry.Key.ShortID == excludeRule.ShortID && targetEntry.Key.Type == EquipmentType.Accessory).Value;
                    if (!targetRules.Exclude.PackRules.Any(rule => rule.ShortID == sourceShortId))
                        targetRules.Exclude.PackRules.Add(CopyEquipmentToPackRule(excludeRule, entry.Key));
                }

                foreach (var excludeRule in entry.Value.Exclude.OptionRules)
                {
                    var targetRules = equipmentRules.First(targetEntry => targetEntry.Key.ShortID == excludeRule.ShortID && targetEntry.Key.Type == EquipmentType.Option).Value;
                    if (!targetRules.Exclude.PackRules.Any(rule => rule.ShortID == sourceShortId))
                        targetRules.Exclude.PackRules.Add(CopyEquipmentToPackRule(excludeRule, entry.Key));
                }

                foreach (var excludeRule in entry.Value.Exclude.PackRules)
                {
                    var targetRules = packRules.First(targetEntry => targetEntry.Key.ShortID == excludeRule.ShortID).Value;
                    if (!targetRules.Exclude.PackRules.Any(rule => rule.ShortID == sourceShortId))
                        targetRules.Exclude.PackRules.Add(CopyPackToPackRule(excludeRule, entry.Key));
                }
            }

            foreach (var entry in equipmentRules)
            {
                var sourceShortId = entry.Key.ShortID ?? 0;

                foreach (var excludeRule in entry.Value.Exclude.AccessoryRules)
                {
                    var targetRules = equipmentRules.First(targetEntry => targetEntry.Key.ShortID == excludeRule.ShortID && targetEntry.Key.Type == EquipmentType.Accessory).Value;
                    if (entry.Key.Type == EquipmentType.Accessory)
                    {
                        if (!targetRules.Exclude.AccessoryRules.Any(rule => rule.ShortID == sourceShortId))
                            targetRules.Exclude.AccessoryRules.Add(CopyEquipmentToEquipmentRule(excludeRule, entry.Key));
                    }
                    else if (entry.Key.Type == EquipmentType.Option)
                    {
                        if (!targetRules.Exclude.OptionRules.Any(rule => rule.ShortID == sourceShortId))
                            targetRules.Exclude.OptionRules.Add(CopyEquipmentToEquipmentRule(excludeRule, entry.Key));
                    }
                }

                foreach (var excludeRule in entry.Value.Exclude.OptionRules)
                {
                    var targetRules = equipmentRules.First(targetEntry => targetEntry.Key.ShortID == excludeRule.ShortID && targetEntry.Key.Type == EquipmentType.Option).Value;
                    if (entry.Key.Type == EquipmentType.Accessory)
                    {
                        if (!targetRules.Exclude.AccessoryRules.Any(rule => rule.ShortID == sourceShortId))
                            targetRules.Exclude.AccessoryRules.Add(CopyEquipmentToEquipmentRule(excludeRule, entry.Key));
                    }
                    else if (entry.Key.Type == EquipmentType.Option)
                    {
                        if (!targetRules.Exclude.OptionRules.Any(rule => rule.ShortID == sourceShortId))
                            targetRules.Exclude.OptionRules.Add(CopyEquipmentToEquipmentRule(excludeRule, entry.Key));
                    }
                }

                foreach (var excludeRule in entry.Value.Exclude.PackRules)
                {
                    var targetRules = packRules.First(targetEntry => targetEntry.Key.ShortID == excludeRule.ShortID).Value;
                    if (entry.Key.Type == EquipmentType.Accessory)
                    {
                        if (!targetRules.Exclude.AccessoryRules.Any(rule => rule.ShortID == sourceShortId))
                            targetRules.Exclude.AccessoryRules.Add(CopyPackToEquipmentRule(excludeRule, entry.Key));
                    }
                    else if (entry.Key.Type == EquipmentType.Option)
                    {
                        if (!targetRules.Exclude.OptionRules.Any(rule => rule.ShortID == sourceShortId))
                            targetRules.Exclude.OptionRules.Add(CopyPackToEquipmentRule(excludeRule, entry.Key));
                    }
                }
            }

            var carRules = packRules.ToDictionary(entry => entry.Key.ID, entry => entry.Value).Concat(equipmentRules.ToDictionary(entry => entry.Key.ID, entry => entry.Value)).ToDictionary();

            return carRules;
        }

        private static RuleSets MapCarEquipmentRules(CarEquipmentItem equipmentItem, EquipmentGroups equipmentGroups, EquipmentItems equipmentItems)
        {
            var equipmentItemGroup = equipmentGroups.Find(equipmentItem.Group.ID);
            var groupRules = new [] {equipmentItemGroup.Rules}.Concat(GetParentGroups(equipmentItemGroup).Select(group => group.Rules)).Reverse().ToList();
            var itemRules = equipmentItem.Rules;
            var crmItemRules = equipmentItems[equipmentItem.ID].Rules;
            var orderedGroups = equipmentGroups.Concat(equipmentGroups.SelectMany(eg => eg.Groups.Flatten(subGroup => subGroup.Groups))).ToList();

            var groupRulesInfos = groupRules.Select(rules => UnrollGroupRules(rules, orderedGroups, equipmentItem, equipmentGroups));

            var groupRuleInfo = groupRulesInfos.Aggregate(ApplyRules);
            
            var crmRulesResult = ApplyRules(groupRuleInfo, UnrollGroupRules(crmItemRules, orderedGroups, equipmentItem, equipmentGroups));

            var equipmentItemRulesInfo = itemRules.OfType<CarEquipmentItemRule>().Where(item => EquipmentItemIsApplicable(equipmentItem.Car, item.ID)).Select(GetRuleInfo).ToList();

            if (equipmentItem is CarOption)
            {
                var option = equipmentItem as CarOption;
                if (option.HasParentOption)
                {
                    equipmentItemRulesInfo.Add(new RuleInfo
                    {
                        Category = Administration.Enums.RuleCategory.VISUAL,
                        ColouringModes = ColouringModes.None,
                        Target = option.ParentOption.ID,
                        Type = RuleType.Include
                    });
                }
            }

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
                    AccessoryRules = accessoryIncludeRules.Where(rule => rule.Target != equipmentItem.ID).Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    OptionRules = optionIncludeRules.Where(rule => rule.Target != equipmentItem.ID).Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    PackRules = includePackRules.Select(rule => MapPackRule(rule, equipmentItem.Car)).ToList()
                },
                Exclude = new RuleSet
                {
                    AccessoryRules = accessoryExcludeRules.Where(rule => rule.Target != equipmentItem.ID).Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    OptionRules = optionExcludeRules.Where(rule => rule.Target != equipmentItem.ID).Select(rule => MapRuleInfo(rule, equipmentItem.Car)).ToList(),
                    PackRules = excludePackRules.Select(rule => MapPackRule(rule, equipmentItem.Car)).ToList()
                }
            };
        }

        private static PackRule MapPackRule(CarPackRule rule, Car car)
        {
            var pack = car.Packs[rule.ID];

            return new PackRule
            {
                Category = rule.Category.Convert(),
                Name = pack.Translation.Name.DefaultIfEmpty(pack.Name),
                ShortID = pack.ShortID ?? 0
            };
        }

        private static EquipmentItemRule MapRuleInfo(RuleInfo info, Car car)
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
            var subGroups = group.Groups.Flatten(subGroup => subGroup.Groups);
            var equipment = group.Equipment.Concat(subGroups.SelectMany(subGroup => subGroup.Equipment));
            var groupItemRules = equipment.Where(item => EquipmentItemIsApplicable(equipmentItem.Car, item.ID)).Select(item => GetRuleInfo(groupRule, item.ID)).ToList();
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

        private static List<EquipmentGroup> GetParentGroups(EquipmentGroup group)
        {
            return group.ParentGroup.ID == Guid.Empty ? new List<EquipmentGroup>() : new[] { group.ParentGroup }.Concat(GetParentGroups(group.ParentGroup)).ToList();
        }

        private static RuleSets MapCarPackRules(CarPackRules rules)
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