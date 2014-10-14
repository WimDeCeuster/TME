using System.Linq;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class LanguagesBuilder
    {
        private readonly Languages _languages;

        private LanguagesBuilder(Languages languages)
        {
            _languages = languages;
        }

        public static LanguagesBuilder Initialize()
        {
            var languages = new Languages();

            var builder = new LanguagesBuilder(languages);

            return builder;
        }

        public LanguagesBuilder AddLanguage(string language)
        {
            _languages.Add(new Language(language));

            return this;
        }

        public LanguagesBuilder AddModelToLanguage(string languageCode, Model model)
        {
            var language = _languages.Single(l => l.Code == languageCode);

            language.Models.Add(model);

            return this;
        }

        public Languages Build()
        {
            return _languages;
        }
    }
}