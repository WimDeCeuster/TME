using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Packs;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGrade
{
    public class WhenAccessingItsPacksForTheFirstTime : TestBase
    {
        private GradePack _pack1;
        private GradePack _pack2;
        private IGrade _grade;
        private IEnumerable<IGradePack> _packs;
        private IPackService _packService;

        protected override void Arrange()
        {
            _pack1 = new GradePackBuilder().Build();
            _pack2 = new GradePackBuilder().Build();

            var repoGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var gradeService = A.Fake<IGradeService>();
            A.CallTo(() => gradeService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.Grade> { repoGrade });

            _packService = A.Fake<IPackService>();
            A.CallTo(() => _packService.GetGradePacks(A<Guid>._, A<Guid>._, repoGrade.ID, A<Context>._)).Returns(new List<GradePack> { _pack1, _pack2 });

            var packFactory = new PackFactoryBuilder()
                .WithPackService(_packService)
                .Build();

            var gradeFactory = new GradeFactoryBuilder()
                .WithGradeService(gradeService)
                .WithPackFactory(packFactory)
                .Build();

            _grade = gradeFactory.GetGrades(publication, context).Single();
        }

        protected override void Act()
        {
            _packs = _grade.Packs;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _packService.GetGradePacks(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveThePacks()
        {
            _packs.Should().HaveCount(2);

            _packs.Should().Contain(a => a.ID == _pack1.ID);
            _packs.Should().Contain(a => a.ID == _pack2.ID);
        }
    }
}