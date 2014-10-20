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
        readonly IGradeFactory _gradeFactory;

        Price _price;
        Boolean _fetchedBasedUponGrade = false;
        IGrade _basedUponGrade;

        public Grade(Repository.Objects.Grade repositoryGrade, Repository.Objects.Publication repositoryPublication, Repository.Objects.Context repositoryContext, IGradeFactory gradeFactory)
            : base(repositoryGrade)
        {
            if (repositoryGrade == null) throw new ArgumentNullException("repositoryGrade");
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");

            _repositoryGrade = repositoryGrade;
            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _gradeFactory = gradeFactory;
        }

        public bool Special { get { return _repositoryGrade.Special; } }

        public IGrade BasedUpon
        {
            get
            {
                if (!_fetchedBasedUponGrade)
                    _basedUponGrade = _gradeFactory.GetGrade(_repositoryPublication, _repositoryContext, _repositoryGrade.BasedUponGradeID);

                return _basedUponGrade;
            }
        }

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
