using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class ContextBuilder
    {
        private readonly Context _context;

        private ContextBuilder(Context context)
        {
            _context = context;
        }

        public static ContextBuilder Initialize()
        {
            var context = new Context
            {
                Brand = "not initialized",
                Country = "not initialized",
                Language = "not initialized",
            };

            return new ContextBuilder(context);
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