using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders;
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
            _context = ContextBuilder.FakeContext().Build();

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