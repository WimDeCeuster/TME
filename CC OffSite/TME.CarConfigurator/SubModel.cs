using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class SubModel : BaseObject<Repository.Objects.SubModel>, ISubModel
    {
        protected readonly Publication RepositoryPublication;
        protected readonly Context RepositoryContext;
        protected readonly IAssetFactory AssetFactory;
        private readonly IGradeFactory _gradeFactory;

        protected IReadOnlyList<IAsset> FetchedAssets;
        private IEnumerable<IGrade> _grades;
        private IEnumerable<ILink> _links;
        private IPrice _startingPrice;

        public SubModel(Repository.Objects.SubModel repositorySubModel, Publication repositoryPublication, Context repositoryContext, IAssetFactory assetFactory, IGradeFactory gradeFactory)
            : base(repositorySubModel)
        {
            if (repositoryPublication == null) throw new ArgumentNullException("repositoryPublication");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (gradeFactory == null) throw new ArgumentNullException("gradeFactory");

            RepositoryPublication = repositoryPublication;
            RepositoryContext = repositoryContext;
            AssetFactory = assetFactory;
            _gradeFactory = gradeFactory;
        }

        public IPrice StartingPrice { get { return _startingPrice = _startingPrice ?? new Price(RepositoryObject.StartingPrice); } }

        public IEnumerable<IGrade> Grades { get { return _grades = _grades ?? _gradeFactory.GetSubModelGrades(RepositoryObject.ID, RepositoryPublication, RepositoryContext); } }

        public virtual IReadOnlyList<IAsset> Assets { get { return FetchedAssets = FetchedAssets ?? AssetFactory.GetAssets(RepositoryPublication,ID, RepositoryContext); } }

        public IEnumerable<ILink> Links { get { return _links = _links ?? RepositoryObject.Links.Select(l => new Link(l)).ToArray(); } }


    }
}