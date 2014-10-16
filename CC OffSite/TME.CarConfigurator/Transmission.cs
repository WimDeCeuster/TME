using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Extensions;

namespace TME.CarConfigurator
{
    public class Transmission : BaseObject, ITransmission
    {
        private readonly Repository.Objects.Transmission _repositoryTransmission;

        private ITransmissionType _type;
        private IEnumerable<IVisibleInModeAndView> _visibleInModeAndViews;

        public Transmission(Repository.Objects.Transmission transmission)
            : base(transmission)
        {
            if (transmission == null) throw new ArgumentNullException("transmission");

            _repositoryTransmission = transmission;
        }

        public ITransmissionType Type { get { return _type = _type ?? new TransmissionType(_repositoryTransmission.Type); } }
        public Boolean KeyFeature { get { return _repositoryTransmission.KeyFeature; } }
        public Boolean Brochure { get { return _repositoryTransmission.Brochure; } }
        public Int32 NumberOfGears { get { return _repositoryTransmission.NumberOfGears; } }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get
            {
                throw new NotImplementedException();
                //return _visibleInModeAndViews = _visibleInModeAndViews ?? _repositoryTransmission.VisibleIn.Select(visibleInModeAndView => new VisibleInModeAndView(_repositoryEngine.ID, visibleInModeAndView, _repositoryPublication, _repositoryContext, _assetFactory)).ToList();
            }
        }

        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }

        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInExteriorSpin { get { return VisibleIn.VisibleInExteriorSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInInteriorSpin { get { return VisibleIn.VisibleInInteriorSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRay4X4Spin { get { return VisibleIn.VisibleInXRay4X4Spin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRayHybridSpin { get { return VisibleIn.VisibleInXRayHybridSpin(); } }
        [Obsolete("Use the new VisibleIn property instead")]
        public bool VisibleInXRaySafetySpin { get { return VisibleIn.VisibleInXRaySafetySpin(); } }

    }
}