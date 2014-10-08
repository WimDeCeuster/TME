using System;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Service;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.S3
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly IPublicationService _publicationService;

        public PublicationRepository(IPublicationService publicationService = null)
        {
            if (publicationService == null) publicationService = new PublicationService();

            _publicationService = publicationService;
        }

        public Publication GetPublication(Guid publicationID)
        {
            return _publicationService.GetPublication(publicationID);
        }
    }
}