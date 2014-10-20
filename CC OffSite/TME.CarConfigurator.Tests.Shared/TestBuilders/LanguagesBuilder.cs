using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class LanguagesBuilder
    {
        private readonly Languages _languages;

        public LanguagesBuilder()
        {
            _languages = new Languages();
        }

        public static LanguagesBuilder Initialize()
        {
            return new LanguagesBuilder();
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