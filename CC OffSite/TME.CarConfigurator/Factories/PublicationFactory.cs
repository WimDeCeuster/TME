using System;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class PublicationFactory : IPublicationFactory
    {
        private readonly IPublicationService _publicationService;

        public PublicationFactory(IPublicationService publicationService)
        {
            if (publicationService == null) throw new ArgumentNullException("publicationService");

            _publicationService = publicationService;
        }

        public Publication GetPublication(Repository.Objects.Model repositoryModel, Context context)
        {
            var publicationInfo = repositoryModel.GetActivePublicationInfo();

            return _publicationService.GetPublication(publicationInfo.ID, context);
        }
    }
}