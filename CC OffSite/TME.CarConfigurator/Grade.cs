using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;

namespace TME.CarConfigurator
{
    public class Grade : BaseObject, IGrade
    {
        readonly Repository.Objects.Grade _repositoryGrade;
        readonly Repository.Objects.Publication _repositoryPublication;
        readonly Repository.Objects.Context _repositoryContext;

        Price _price;
        IGrade _basedUponGrade;

        public Grade(Repository.Objects.Grade repositoryGrade, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IGrade basedUponGrade)
            : base(repositoryGrade)
        {
            if (repositoryGrade == null) throw new ArgumentNullException("repositoryGrade");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");

            _repositoryGrade = repositoryGrade;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _basedUponGrade = basedUponGrade;
        }

        public bool Special { get { return _repositoryGrade.Special; } }

        public IGrade BasedUpon { get { return _basedUponGrade; } }

        public IPrice StartingPrice { get { return _price = _price ?? new Price(_repositoryGrade.StartingPrice); } }

        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }

        public IEnumerable<IVisibleInModeAndView> VisibleIn
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<Interfaces.Equipment.IGradeEquipmentItem> Equipment
        {
            get { throw new NotImplementedException(); }
        }
    }
}
