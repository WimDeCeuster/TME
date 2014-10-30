using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Colours;

namespace TME.CarConfigurator.Colours
{
    public class Upholstery : BaseObject<Repository.Objects.Colours.Upholstery>, IUpholstery
    {
        private UpholsteryType _type;

        public Upholstery(Repository.Objects.Colours.Upholstery repositoryUpholstery)
            : base(repositoryUpholstery)
        {

        }

        public string InteriorColourCode
        {
            get { return RepositoryObject.InteriorColourCode; }
        }

        public string TrimCode
        {
            get { return RepositoryObject.TrimCode; }
        }

        public IUpholsteryType Type
        {
            get { return _type = _type ?? new UpholsteryType(RepositoryObject.Type); }
        }

        public IEnumerable<Interfaces.Assets.IAsset> Assets
        {
            get { throw new NotImplementedException(); }
        }
    }
}
