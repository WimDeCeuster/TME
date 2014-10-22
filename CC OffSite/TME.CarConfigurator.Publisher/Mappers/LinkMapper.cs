using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class LinkMapper : ILinkMapper
    {
        public Link MapLink(Administration.Link link, Boolean isPreview)
        {
            var currentContext = Administration.MyContext.GetContext();
            var countryCode = currentContext.CountryCode;
            var languageCode = currentContext.LanguageCode;

            var baseLink = Administration.BaseLinks.GetBaseLinks(link.Type, isPreview)
                                                   .SingleOrDefault(baseLnk => baseLnk.CountryCode == countryCode &&
                                                                               baseLnk.LanguageCode == languageCode);

            return new Link
            {
                ID = link.Type.ID,
                Label = link.Label,
                Name = link.Type.Name,
                Url = GetUrl(baseLink, link)
            };
        }

        String GetUrl(Administration.BaseLink baseLink, Administration.Link link)
        {
            if (baseLink == null || String.IsNullOrWhiteSpace(baseLink.Url))
                return link.UrlPart;
            if (link.UrlPart.StartsWith("http://") || link.UrlPart.StartsWith("https://"))
                return link.UrlPart;
            return new Uri(new Uri(baseLink.Url), link.UrlPart).ToString();
        }

    }
}
