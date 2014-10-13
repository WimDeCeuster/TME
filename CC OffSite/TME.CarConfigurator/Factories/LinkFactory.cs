using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator.Factories
{
    public class LinkFactory : ILinkFactory
    {
        public IEnumerable<ILink> CreateLinks(IEnumerable<Repository.Objects.Link> links)
        {
            return links.Select(l => new Link(l));
        }
    }
}