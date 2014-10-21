using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModel
{
    public class WhenAccessingItsEquipmentForTheFirstTime : TestBase
    {
        private ISubModel _subModel;
        private IEnumerable<IEquipmentItem> _equipment;
        private EquipmentItem _accessoryItem;
        private EquipmentItem _gradeAccessoryItem;
        private EquipmentItem _gradeOptionItem;
        private EquipmentItem _optionItem;

        protected override void Arrange()
        {
            _accessoryItem = new AccessoryBuilder().Build();
            _gradeAccessoryItem = new GradeAccessoryBuilder().Build();
            _gradeOptionItem = new GradeOptionBuilder().Build();
            _optionItem = new OptionBuilder().Build();

            var repositorySubModel = new SubModelBuilder()
                .WithID(Guid.NewGuid())
                .WithEquipment(_accessoryItem,_gradeAccessoryItem,_gradeOptionItem,_optionItem)
                .Build();

            var publicationTimeFrame =
                new PublicationTimeFrameBuilder()
                .WithID(Guid.NewGuid())
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var subModelService = A.Fake<ISubModelService>();
            A.CallTo(() => subModelService.GetSubModels(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.SubModel>() { repositorySubModel });

            var subModelFactory = new SubModelFactoryBuilder()
                .WithSubModelService(subModelService)
                .Build();

            _subModel = subModelFactory.GetSubModels(publication, context).Single();
        }

        protected override void Act()
        {
            _equipment = _subModel.Equipment;
        }

        [Fact]
        public void ThenTheStartingPriceShouldBeCorrect()
        {
            _startingPrice.PriceExVat.Should().Be(_repoPrice.ExcludingVat);
            _startingPrice.PriceInVat.Should().Be(_repoPrice.IncludingVat);
        }
    }

    public class OptionBuilder
    {
        private Option _option;

        public OptionBuilder()
        {
            _option = new Option();
        }

        public EquipmentItem Build()
        {
            return _option;
        }
    }

    public class GradeOptionBuilder
    {
        private GradeOption _gradeOption;

        public GradeOptionBuilder()
        {
            _gradeOption = new GradeOption();
        }

        public EquipmentItem Build()
        {
            return _gradeOption;
        }
    }

    public class GradeAccessoryBuilder
    {
        private GradeAccessory _gradeAccessory;

        public GradeAccessoryBuilder()
        {
            _gradeAccessory = new GradeAccessory();
        }

        public EquipmentItem Build()
        {
            return _gradeAccessory;
        }
    }

    public class AccessoryBuilder
    {
        private Accessory _accessory;

        public AccessoryBuilder()
        {
            _accessory = new Accessory();
        }

        public EquipmentItem Build()
        {
            return _accessory;
        }
    }
}