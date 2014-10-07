using System;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders.RepositoryObjects
{
    internal class PublicationInfoBuilder
    {
        private readonly Publication _publication;
        private PublicationState _publicationState;

        private PublicationInfoBuilder(Publication publication)
        {
            _publication = publication;
        }

        public static PublicationInfoBuilder Initialize()
        {
            var publication = new Publication();

            return new PublicationInfoBuilder(publication);
        }

        public PublicationInfoBuilder WithGeneration(Generation generation)
        {
            _publication.Generation = generation;

            return this;
        }

        public PublicationInfoBuilder WithDateRange(DateTime from, DateTime to)
        {
            _publication.LineOffFrom = from;
            _publication.LineOffTo = to;

            return this;
        }

        public PublicationInfoBuilder WithState(PublicationState publicationState)
        {
            _publicationState = publicationState;

            return this;
        }

        public PublicationInfoBuilder WithID(Guid id)
        {
            _publication.ID = id;

            return this;
        }

        public PublicationInfoBuilder CurrentlyActive()
        {
            return WithDateRange(DateTime.MinValue, DateTime.MaxValue).WithState(PublicationState.Activated);
        }

        public PublicationInfo Build()
        {
            return new PublicationInfo(_publication) {State = _publicationState};
        }
    }
}