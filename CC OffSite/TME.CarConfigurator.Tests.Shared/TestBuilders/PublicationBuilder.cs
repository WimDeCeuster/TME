using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class PublicationBuilder
    {
        private readonly Publication _publication;

        public PublicationBuilder()
        {
            _publication = new Publication
            {
                TimeFrames = new List<PublicationTimeFrame>()
            };
        }

        public static PublicationBuilder Initialize()
        {
            return new PublicationBuilder();
        }

        public PublicationBuilder WithGeneration(Generation generation)
        {
            _publication.Generation = generation;

            return this;
        }

        public PublicationBuilder WithID(Guid publicationId)
        {
            _publication.ID = publicationId;

            return this;
        }

        public PublicationBuilder WithDateRange(DateTime from, DateTime to)
        {
            _publication.LineOffFrom = from;
            _publication.LineOffTo = to;

            return this;
        }

        public PublicationBuilder WithTimeFrames(params PublicationTimeFrame[] timeFrames)
        {
            _publication.TimeFrames.AddRange(timeFrames);

            return this;
        }

        public Publication Build()
        {
            return _publication;
        }

        public PublicationBuilder AddTimeFrame(PublicationTimeFrame publicationTimeFrame)
        {
            if (_publication.TimeFrames == null)
                _publication.TimeFrames = new List<PublicationTimeFrame>();

            _publication.TimeFrames.Add(publicationTimeFrame);

            return this;
        }
    }
}