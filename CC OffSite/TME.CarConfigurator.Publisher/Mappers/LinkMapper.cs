using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Publisher.Interfaces;
using Link = TME.CarConfigurator.Repository.Objects.Link;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class LinkMapper : ILinkMapper
    {

        public List<Link> MapLinks(ModelGeneration generation, IEnumerable<Administration.Link> links, bool isPreview)
        {
            return links.Where(link => link.IsApplicableFor(generation))
                .Select(link => MapLink(link, isPreview))
                .OrderBy(link => link.Name)
                .ToList();

        }
        public Link MapLink(Administration.Link link, Boolean isPreview)
        {
            var currentContext = MyContext.GetContext();
            var countryCode = currentContext.CountryCode;
            var languageCode = currentContext.LanguageCode;

            var baseLink = BaseLinks.GetBaseLinks(link.Type, isPreview)
                                                   .SingleOrDefault(baseLnk => baseLnk.CountryCode == countryCode &&
                                                                               baseLnk.LanguageCode == languageCode);

            return new Link
            {
                ID = link.Type.ID,
                Label = link.Label,
                Name = link.Type.TypeName,
                Url = GetUrl(baseLink, link)
            };
        }

        static String GetUrl(BaseLink baseLink, Administration.Link link)
        {
            if (baseLink == null || String.IsNullOrWhiteSpace(baseLink.Url))
                return link.UrlPart;
            if (link.UrlPart.StartsWith("http://") || link.UrlPart.StartsWith("https://"))
                return link.UrlPart;
            return new Uri(new Uri(baseLink.Url), link.UrlPart).ToString();
        }

    }
}
