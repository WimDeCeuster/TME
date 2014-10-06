using System;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Repository.Objects
{
    public class PublicationInfo
    {
        public Guid ID { get; set; }
        public DateTime? LineOffFrom { get; set; }
        public DateTime? LineOffTo { get; set; }
        public Guid GenerationID { get; set; }
        public PublicationState State { get; set; }

        public PublicationInfo()
        {

        }

        public PublicationInfo(Publication publication)
        {
            ID = publication.ID;
            LineOffFrom = publication.LineOffFrom;
            LineOffTo = publication.LineOffTo;
            GenerationID = publication.Generation.ID;
            State = PublicationState.Activated;
        }
    }
}