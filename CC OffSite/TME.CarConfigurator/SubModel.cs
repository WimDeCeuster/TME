using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class SubModel : BaseObject<Repository.Objects.SubModel>, ISubModel
    {
        private readonly Publication _repositoryPublication;
        private readonly Context _repositoryContext;
        private readonly IAssetFactory _assetFactory;
        private readonly IGradeFactory _gradeFactory;

        private IEnumerable<IAsset> _assets;
        private IEnumerable<IGrade> _grades; 
        private IEnumerable<ILink> _links;
        private IPrice _startingPrice;

        public SubModel(Repository.Objects.SubModel repositorySubModel,Publication repositoryPublication,Context repositoryContext,IAssetFactory assetFactory,IGradeFactory gradeFactory) 
            : base(repositorySubModel)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");

            _repositoryPublication = repositoryPublication;
            _repositoryContext = repositoryContext;
            _assetFactory = assetFactory;
            _gradeFactory = gradeFactory;
        }

        public IPrice StartingPrice { get { return _startingPrice = _startingPrice ?? new Price(RepositoryObject.StartingPrice); } }

        public IEnumerable<IEquipmentItem> Equipment
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IGrade> Grades
        {
            get
            {
                return
                    _grades =
                        _grades ??
                        RepositoryObject.Grades.Select(
                            grade => _gradeFactory.GetSubModelGrade(grade,_repositoryPublication,_repositoryContext));
//                        _gradeFactory.GetSubModelGrades(_repositoryPublication, _repositoryContext, RepositoryObject);
            }
        }

        public IEnumerable<IAsset> Assets { get { return _assets = _assets ?? _assetFactory.GetAssets(_repositoryPublication, ID, _repositoryContext); } }

        public IEnumerable<ILink> Links
        {
            get { return _links = _links ?? RepositoryObject.Links.Select(l => new Link(l)).ToArray(); }
        }

 
    }
}