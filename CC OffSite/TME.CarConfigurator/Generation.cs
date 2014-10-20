using System;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class Generation : BaseObject, IGeneration
    {
        private Repository.Objects.Generation _repositoryGeneration;

        public Generation(Repository.Objects.Generation repositoryGeneration) 
            : base(repositoryGeneration)
        {
            if(repositoryGeneration == null) throw new ArgumentNullException("repositoryGeneration");
            _repositoryGeneration = repositoryGeneration;
        }
    }
}