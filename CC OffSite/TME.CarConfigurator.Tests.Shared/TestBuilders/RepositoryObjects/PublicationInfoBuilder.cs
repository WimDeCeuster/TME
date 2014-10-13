using System;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects
{
    public class PublicationInfoBuilder
    {
        private readonly Publication _publication;
        private PublicationState _publicationState;

        public PublicationInfoBuilder()
        {
            _publication = new Publication();
        }

        public static PublicationInfoBuilder Initialize()
        {
            return new PublicationInfoBuilder();
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