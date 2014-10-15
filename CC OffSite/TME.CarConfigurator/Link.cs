using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class Link : ILink
    {
        private readonly Repository.Objects.Link _repositoryLink;

        public Link(Repository.Objects.Link repositoryLink)
        {
            if (repositoryLink == null) throw new ArgumentNullException("repositoryLink");
            _repositoryLink = repositoryLink;
        }

        public short ID { get { return _repositoryLink.ID; } }
        public string Name { get { return _repositoryLink.Name; } }
        public string Label { get { return _repositoryLink.Label; } }
        public string Url { get { return _repositoryLink.Url; } }
    }
}
