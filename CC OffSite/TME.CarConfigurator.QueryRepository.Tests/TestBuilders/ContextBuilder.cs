using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Query.Tests.TestBuilders
{
    public class ContextBuilder
    {
        private readonly Context _context;

        public ContextBuilder()
        {
            _context = new Context
            {
                Brand = "not initialized",
                Country = "not initialized",
                Language = "not initialized",
            };

        }

        public static ContextBuilder Initialize()
        {
            return new ContextBuilder();
        }

        public ContextBuilder WithBrand(string brand)
        {
            _context.Brand = brand;

            return this;
        }

        public ContextBuilder WithCountry(string country)
        {
            _context.Country = country;

            return this;

        }

        public ContextBuilder WithLanguage(string language)
        {
            _context.Language = language;

            return this;
        }

        public Context Build()
        {
            return _context;
        }
    }
}