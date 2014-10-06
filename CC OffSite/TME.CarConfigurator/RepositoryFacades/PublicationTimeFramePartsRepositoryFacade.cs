using System;
using TME.CarConfigurator.QueryRepository;
using TME.CarConfigurator.QueryRepository.Interfaces;

namespace TME.CarConfigurator.RepositoryFacades
{
    public class PublicationTimeFramePartsRepositoryFacade
    {
        
        public IBodyTypeRepository BodyTypeRepository { get; private set; }
        public IEngineRepository EngineRepository { get; private set; }

        public PublicationTimeFramePartsRepositoryFacade(IBodyTypeRepository bodyTypeRepository,IEngineRepository engineRepository)
        {
            if (bodyTypeRepository == null) throw new ArgumentNullException("bodyTypeRepository");
            if (engineRepository == null) throw new ArgumentNullException("engineRepository");

            BodyTypeRepository = bodyTypeRepository;
            EngineRepository = engineRepository;
        }

    }
}