using System;
using TME.CarConfigurator.QueryRepository.Service.Base;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Service
{
    public class PublicationService : ServiceBase, IPublicationService
    {
        public PublicationService(ISerialiser serialiser, IService service, IKeyManager keyManager)
            :base(serialiser,service,keyManager)
        {

        }

        public Publication GetPublication(Guid publicationId)
        {
            throw new NotImplementedException();
        }
    }
}