﻿using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Equipment
{
    public abstract class GradeEquipmentItem<T> : BaseObject<T>, IGradeEquipmentItem
        where T : Repository.Objects.Equipment.GradeEquipmentItem
    {
        readonly IColourFactory _colourFactory;
        readonly Context _context;
        readonly Publication _publication;

        IExteriorColour _exteriorColour;
        ICategoryInfo _categoryInfo;
        IReadOnlyList<ILink> _links;
        IReadOnlyList<ICarInfo> _standardOn;
        IReadOnlyList<ICarInfo> _optionalOn;
        IReadOnlyList<ICarInfo> _notAvailableOn;

        public GradeEquipmentItem(T repositoryEquipmentItem, Publication publication, Context context, IColourFactory colourFactory)
            : base(repositoryEquipmentItem)
        {
            if (publication == null) throw new ArgumentNullException("publication");
            if (context == null) throw new ArgumentNullException("context");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _publication = publication;
            _context = context;
            _colourFactory = colourFactory;
        }

        public int ShortID { get { return RepositoryObject.ShortID; } }

        public string InternalName { get { return RepositoryObject.InternalName; } }

        public string PartNumber { get { return RepositoryObject.PartNumber; } }

        public string Path { get { return RepositoryObject.Path; } }

        public bool KeyFeature { get { return RepositoryObject.KeyFeature; } }

        public bool GradeFeature { get { return RepositoryObject.GradeFeature; } }

        public bool OptionalGradeFeature { get { return RepositoryObject.OptionalGradeFeature; } }

        [Obsolete]
        public bool Brochure { get { return RepositoryObject.Visibility.HasFlag(Repository.Objects.Enums.Visibility.Brochure); } }

        public bool Standard { get { return RepositoryObject.Standard; } }

        public bool Optional { get { return RepositoryObject.Optional; } }

        public bool NotAvailable { get { return RepositoryObject.NotAvailable; } }

        public Interfaces.Enums.Visibility Visibility { get { return RepositoryObject.Visibility.ToVisibility(); } }

        public IBestVisibleIn BestVisibleIn
        {
            get { throw new NotImplementedException(); }
        }

        public ICategoryInfo Category { get { return _categoryInfo = _categoryInfo ?? new CategoryInfo(RepositoryObject.Category); } }

        public Interfaces.Colours.IExteriorColour ExteriorColour { get { return RepositoryObject.ExteriorColour == null ? null : _exteriorColour = _exteriorColour ?? _colourFactory.GetExteriorColour(RepositoryObject.ExteriorColour, _publication, _context); } }

        public IEnumerable<Interfaces.Assets.IAsset> Assets { get { throw new NotImplementedException(); } }

        public IEnumerable<Interfaces.ILink> Links { get { return _links = _links ?? RepositoryObject.Links.Select(link => new Link(link)).ToList(); } }

        public IReadOnlyList<ICarInfo> StandardOn { get { return _standardOn = _standardOn ?? RepositoryObject.StandardOn.Select(carInfo => new CarInfo(carInfo)).ToList(); } }

        public IReadOnlyList<ICarInfo> OptionalOn { get { return _optionalOn = _optionalOn ?? RepositoryObject.OptionalOn.Select(carInfo => new CarInfo(carInfo)).ToList(); } }

        public IReadOnlyList<ICarInfo> NotAvailableOn { get { return _notAvailableOn = _notAvailableOn ?? RepositoryObject.NotAvailableOn.Select(carInfo => new CarInfo(carInfo)).ToList(); } }
    }
}
