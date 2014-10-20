using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.DI;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.Services.GivenAGradeService
{
    public class WhenGetGradesIsCalled : TestBase
    {
        private Context _context;
        private IGradeService _gradeService;
        private IEnumerable<Repository.Objects.Grade> _expectedGrades;
        private IEnumerable<Repository.Objects.Grade> _actualGrades;

        protected override void Arrange()
        {
            _context = ContextBuilder.Initialize().Build();

            const string s3Key = "fake s3 key";
            const string serializedObject = "this object is serialized";

            _expectedGrades = new List<Repository.Objects.Grade>
            {
                new GradeBuilder().Build(),
                new GradeBuilder().Build(),
            };

            var serialiser = A.Fake<ISerialiser>();
            var service = A.Fake<IService>();
            var keyManager = A.Fake<IKeyManager>();

            A.CallTo(() => keyManager.GetGradesKey(A<Guid>._, A<Guid>._)).Returns(s3Key);
            A.CallTo(() => service.GetObject(_context.Brand, _context.Country, s3Key)).Returns(serializedObject);
            A.CallTo(() => serialiser.Deserialise<IEnumerable<Repository.Objects.Grade>>(serializedObject)).Returns(_expectedGrades);

            var serviceFacade = new S3ServiceFacade()
                .WithService(service)
                .WithSerializer(serialiser)
                .WithKeyManager(keyManager);

            _gradeService = serviceFacade.CreateGradeService();
        }

        protected override void Act()
        {
            _actualGrades = _gradeService.GetGrades(Guid.NewGuid(), Guid.NewGuid(), _context);
        }

        [Fact]
        public void ThenItShouldReturnTheCorrectListOfGrades()
        {
            _actualGrades.Should().BeSameAs(_expectedGrades);
        }
    }
}