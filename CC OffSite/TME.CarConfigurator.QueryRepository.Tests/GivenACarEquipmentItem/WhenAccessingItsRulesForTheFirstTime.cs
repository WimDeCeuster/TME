using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Repository.Objects.Rules;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarEquipmentItem
{
    public class WhenAccessingItsRulesForTheFirstTime : TestBase
    {
        private ICarEquipment _equipmentItem;
        private IRuleSets _equipmentItemRules;
        private RuleSets _repoRuleSets;
        private IRuleService _ruleService;

        protected override void Arrange()
        {
            var carAccessory = new CarAccessoryBuilder().WithId(Guid.NewGuid()).Build();
            var carOption = new CarOptionBuilder().WithId(Guid.NewGuid()).Build();

            var accessoryRule = new EquipmentItemRuleBuilder().WithShortID(0).WithCategory(RuleCategory.Product).Build();
            var optionRule = new EquipmentItemRuleBuilder().WithShortID(1).WithCategory(RuleCategory.Visual).Build();
            var packRule = new PackRuleBuilder().WithShortID(2).WithCategory(RuleCategory.Marketing).Build();

            _repoRuleSets = new RuleSets
            {
                Exclude = new RuleSetBuilder()
                .WithAccessoryRules(accessoryRule)
                .WithOptionRules(optionRule)
                .WithPackRules(packRule)
                .Build(),

                Include = A.Fake<RuleSet>()
            };

            var repoCarEquipment = new CarEquipmentBuilder()
                .WithAccessories(carAccessory)
                .WithOptions(carOption)
                .Build();

            var carID = Guid.NewGuid();

            var publication = new PublicationBuilder().Build();

            var context = new ContextBuilder().Build();

            var equipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => equipmentService.GetCarEquipment(carID, publication.ID, context)).Returns(repoCarEquipment);

            _ruleService = A.Fake<IRuleService>();
            A.CallTo(() => _ruleService.GetCarItemRuleSets(carAccessory.ID, carID, publication.ID, context)).Returns(_repoRuleSets);

            var ruleFactory = new RuleFactoryBuilder().WithRuleService(_ruleService).Build();

            var equipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(equipmentService)
                .WithRuleFactory(ruleFactory)
                .Build();
                
            _equipmentItem = equipmentFactory.GetCarEquipment(carID, publication, context);
        }

        protected override void Act()
        {
            _equipmentItemRules = _equipmentItem.Accessories.First().Rules;
        }

        [Fact]
        public void ThenItShouldCalculateTheRules()
        {
            A.CallTo(() => _ruleService.GetCarItemRuleSets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheExcludeRulesForAccessories()
        {
            _equipmentItemRules.Exclude.Accessories.First().ShortID.Should().Be(_repoRuleSets.Exclude.AccessoryRules.First().ShortID);
            _equipmentItemRules.Exclude.Options.First().ShortID.Should().Be(_repoRuleSets.Exclude.OptionRules.First().ShortID);
            _equipmentItemRules.Exclude.Packs.First().ShortID.Should().Be(_repoRuleSets.Exclude.PackRules.First().ShortID);
        }
    }
}