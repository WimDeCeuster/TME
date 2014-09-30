using System;
using TME.CarConfigurator.QueryRepository;

namespace TME.CarConfigurator.RepositoryFacades
{
    public class PublicationRepositoryFacade
    {
        public IPublicationRepository PublicationRepository { get; private set; }
        public PublicationTimeFrameRepositoryFacade PublicationTimeFrameRepositoryFacade { get; private set; }


        public PublicationRepositoryFacade(IPublicationRepository publicationRepository, PublicationTimeFrameRepositoryFacade publicationTimeFrameRepositoryFacade)
        {
            if (publicationRepository == null) throw new ArgumentNullException("publicationRepository");
            if (publicationTimeFrameRepositoryFacade == null)  throw new ArgumentNullException("publicationTimeFrameRepositoryFacade");

            PublicationRepository = publicationRepository;
            PublicationTimeFrameRepositoryFacade = publicationTimeFrameRepositoryFacade;
        }
    }
}
