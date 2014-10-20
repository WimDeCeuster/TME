using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class PublicationTimeFrameBuilder
    {
        private readonly PublicationTimeFrame _timeFrame;

        public PublicationTimeFrameBuilder()
        {
            _timeFrame = new PublicationTimeFrame();
        }

        public PublicationTimeFrameBuilder WithID(Guid publicationTimeFrameID)
        {
            _timeFrame.ID = publicationTimeFrameID;
            return this;
        }

        public PublicationTimeFrameBuilder WithDateRange(DateTime from, DateTime to)
        {
            _timeFrame.LineOffFrom = from;
            _timeFrame.LineOffTo = to;

            return this;
        }

        public PublicationTimeFrame Build()
        {
            return _timeFrame;
        }
    }
}