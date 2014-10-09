using System;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly IPublicationService _publicationService;

        public PublicationRepository(IPublicationService publicationService)
        {
            _publicationService = publicationService; // todo: local default
        }

        public Publication GetPublication(Guid publicationID, Context context)
        {
            return _publicationService.GetPublication(publicationID, context);
        }
    }
}