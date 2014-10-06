using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.QueryRepository.Tests.GivenModels
{
    public class WhenRequestingTheListOfActiveModels : TestBase
    {
        private IModels _modelsFromRespository;
        private IContext _context;
        private IModels _models;
        private IModelRepository _modelsRepository;

        protected override void Arrange()
        {
            ArrangeContext();
            ArrangeModelsRepository();
        }

        private void ArrangeModelsRepository()
        {
            _modelsFromRespository = new TestImplementations.Models(); // TODO: initialize with values

            _modelsRepository = A.Fake<IModelRepository>();
            A.CallTo(() => _modelsRepository.GetModels(null))
                .WhenArgumentsMatch(args =>
                {
                    var contextInArgs = (IContext) args[0];
                    return TestHelpers.Context.AreEqual(contextInArgs, _context);
                })
                .Returns(_modelsFromRespository);
        }

        private void ArrangeContext()
        {
            _context = A.Fake<IContext>();
            A.CallTo(() => _context.Brand).Returns("brand");
            A.CallTo(() => _context.Country).Returns("country");
            A.CallTo(() => _context.Language).Returns("language");
        }

        protected override void Act()
        {
            _models = Models.GetModels(_context, _modelsRepository);
        }

        [Fact]
        public void ThenTheListShouldBeFetchedFromTheRepository()
        {
            A.CallTo(() => _modelsRepository.GetModels(null))
                .WhenArgumentsMatch(args =>
                {
                    var contextInArgs = (IContext) args[0];
                    return TestHelpers.Context.AreEqual(contextInArgs, _context);
                })
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheModelsListShouldBeWhatWasReturnedFromTheRepository()
        {
            _models.ShouldBeEquivalentTo(_modelsFromRespository, "because the repository should already take care of filtering active models");
        }
    }
}