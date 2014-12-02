using System;
using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using Link = TME.CarConfigurator.Repository.Objects.Link;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ILinkMapper
    {
        List<Link> MapLinks(ModelGeneration generation, IEnumerable<Administration.Link> links, bool isPreview);
        Link MapLink(Administration.Link link, Boolean isPreview);
    }
}
