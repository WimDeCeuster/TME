using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class Link : ILink
    {
        private readonly Repository.Objects.Link _link;

        public Link(Repository.Objects.Link link)
        {
            if (link == null) throw new ArgumentNullException("link");
            _link = link;
        }

        public short ID { get { return _link.ID; } }
        public string Name { get { return _link.Name; } }
        public string Label { get { return _link.Label; } }
        public string Url { get { return _link.Url; } }
    }
}
