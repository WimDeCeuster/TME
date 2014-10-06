using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class Model : Core.BaseObject, IModel
    {
        private readonly Repository.Objects.Model _repositoryModel;

        public string Brand { get { return _repositoryModel.Brand; } }
        public bool Promoted { get { return _repositoryModel.Promoted; } }
        public String SSN { get { throw new NotImplementedException(); } }
        public ICarConfiguratorVersion CarConfiguratorVersion { get { throw new NotImplementedException(); } }
        public IEnumerable<ILink> Links { get { throw new NotImplementedException(); } }
        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }
        public IEnumerable<IBodyType> BodyTypes { get { throw new NotImplementedException(); } }
        public IEnumerable<IEngine> Engines { get { throw new NotImplementedException(); } }
        public IEnumerable<IFuelType> FuelTypes { get { throw new NotImplementedException(); } }
        public IEnumerable<ICar> Cars { get { throw new NotImplementedException(); } }

        public Model(Repository.Objects.Model repositoryModel)
            : base(repositoryModel)
        {
            if (repositoryModel == null) throw new ArgumentNullException("repositoryModel");

            _repositoryModel = repositoryModel;
        }
    }
}
