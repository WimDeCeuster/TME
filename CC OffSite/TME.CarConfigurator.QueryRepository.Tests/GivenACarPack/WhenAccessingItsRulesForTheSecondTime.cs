using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Interfaces.Rules;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.Repository.Objects.Rules;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarPack
{
    public class WhenAccessingItsRulesForTheSecondTime : TestBase
    {
        private IRuleSets _secondPackRules;
        private RuleSets _repoRuleSets;
        private IRuleService _ruleService;
        private IReadOnlyList<ICarPack> _carPack;
        private IRuleSets _firstPackRules;

        protected override void Arrange()
        {
            var accessoryRule = new EquipmentItemRuleBuilder().WithShortID(0).WithCategory(RuleCategory.Product).Build();
            var optionRule = new EquipmentItemRuleBuilder().WithShortID(1).WithCategory(RuleCategory.Visual).Build();
            var packRule = new PackRuleBuilder().WithShortID(2).WithCategory(RuleCategory.Marketing).Build();

            _repoRuleSets = new RuleSets
            {
                Exclude = A.Fake<RuleSet>(),
                Include = new RuleSetBuilder()
                .WithAccessoryRules(accessoryRule)
                .WithOptionRules(optionRule)
                .WithPackRules(packRule)
                .Build()
            };

            var repoCarPack = new CarPackBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var carID = Guid.NewGuid();

            var publication = new PublicationBuilder().Build();

            var context = new ContextBuilder().Build();

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetCarPacks(publication.ID, carID, context)).Returns(new[] { repoCarPack });

            _ruleService = A.Fake<IRuleService>();
            A.CallTo(() => _ruleService.GetCarItemRuleSets(repoCarPack.ID, carID, publication.ID, context)).Returns(_repoRuleSets);

            var ruleFactory = new RuleFactoryBuilder()
                .WithRuleService(_ruleService).Build();

            var packFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .WithRuleFactory(ruleFactory)
                .Build();

            _carPack = packFactory.GetCarPacks(publication, context, carID);

            _firstPackRules = _carPack.First().Rules;
        }

        protected override void Act()
        {
            _secondPackRules = _carPack.First().Rules;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheRules()
        {
            A.CallTo(() => _ruleService.GetCarItemRuleSets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheExcludeRulesForAccessories()
        {
            _secondPackRules.Should().BeSameAs(_firstPackRules);

            _secondPackRules.Include.Accessories.First().ShortID.Should().Be(_repoRuleSets.Include.AccessoryRules.First().ShortID);
            _secondPackRules.Include.Options.First().ShortID.Should().Be(_repoRuleSets.Include.OptionRules.First().ShortID);
            _secondPackRules.Include.Packs.First().ShortID.Should().Be(_repoRuleSets.Include.PackRules.First().ShortID);
        }
    }
}