using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ILinkFactory
    {
        IEnumerable<ILink> CreateLinks(IEnumerable<Link> links);
    }
}