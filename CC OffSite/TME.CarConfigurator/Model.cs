using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class Model : Core.BaseObject, IModel
    {
        
        #region Dependencies (Adaptees)

        private Repository.Objects.Model RepositoryModel { get; set; }
        private Repository.Objects.Publication RepositoryPublication { get; set; }

        #endregion

        #region Constructor
        internal Model(
            Repository.Objects.Model repositoryModel, 
            Repository.Objects.Publication repositoryPublication
            )
            : base(repositoryModel)
        {
            if (repositoryModel == null) throw new ArgumentNullException("repositoryModel");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");

            RepositoryModel = repositoryModel;
            RepositoryPublication = repositoryPublication;
        }

        #endregion

        #region Properties
        public string Brand
        {
            get { return RepositoryModel.Brand; }
        }
        public bool Promoted
        {
            get { return RepositoryModel.Promoted; }
        }

        public IEnumerable<String> SSNs
        {
            get { return RepositoryPublication.Generation.SSNs; }
        }



        public ICarConfiguratorVersion CarConfiguratorVersion
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<ILink> Links
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IAsset> Assets
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IBodyType> BodyTypes
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IEngine> Engines
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IFuelType> FuelTypes
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<ICar> Cars
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
    }
}
