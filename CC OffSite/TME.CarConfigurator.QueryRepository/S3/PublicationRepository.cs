using System;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.S3
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly IPublicationService _publicationService;

        public PublicationRepository(IPublicationService publicationService = null)
        {
            _publicationService = publicationService; // todo: local default
        }

        public Publication GetPublication(Guid publicationID, IContext context)
        {
            return _publicationService.GetPublication(publicationID, context);
        }
    }
}