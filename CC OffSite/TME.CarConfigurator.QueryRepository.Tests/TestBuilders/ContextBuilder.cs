using FakeItEasy;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class ContextBuilder
    {
        private readonly IContext _context;

        private ContextBuilder(IContext context)
        {
            _context = context;
        }

        public static ContextBuilder FakeContext()
        {
            var context = A.Fake<IContext>();
            
            A.CallTo(() => context.Brand).Returns("not initialized");
            A.CallTo(() => context.Country).Returns("not initialized");
            A.CallTo(() => context.Language).Returns("not initialized");

            return new ContextBuilder(context);
        }

        public ContextBuilder WithBrand(string brand)
        {
            A.CallTo(() => _context.Brand).Returns(brand);

            return this;
        }

        public ContextBuilder WithCountry(string country)
        {
            A.CallTo(() => _context.Country).Returns(country);

            return this;

        }

        public ContextBuilder WithLanguage(string language)
        {
            A.CallTo(() => _context.Language).Returns(language);

            return this;
        }

        public IContext Build()
        {
            return _context;
        }
    }
}