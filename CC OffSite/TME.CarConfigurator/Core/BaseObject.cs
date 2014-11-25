using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Core
{
    public abstract class BaseObject<T> : IBaseObject where T : Repository.Objects.Core.BaseObject
    {
        protected readonly T RepositoryObject;
        private IReadOnlyList<ILabel> _labels;

        public Guid ID
        {
            get { return RepositoryObject.ID; }
        }

        public string InternalCode
        {
            get { return RepositoryObject.InternalCode; }
        }

        public string LocalCode
        {
            get { return RepositoryObject.LocalCode; }
        }

        public string Name
        {
            get { return RepositoryObject.Name; }
        }

        public string Description
        {
            get { return RepositoryObject.Description; }
        }

        public string FootNote
        {
            get { return RepositoryObject.FootNote; }
        }

        public string ToolTip
        {
            get { return RepositoryObject.ToolTip; }
        }

        public int SortIndex
        {
            get { return RepositoryObject.SortIndex; }
        }

        public IReadOnlyList<ILabel> Labels
        {
            get { return _labels = _labels ?? RepositoryObject.Labels.Select(label => new Label(label)).ToArray(); }
        }

        protected BaseObject(T repositoryObject)
        {
            if (repositoryObject == null) throw new ArgumentNullException("repositoryObject");

            RepositoryObject = repositoryObject;
        }


        #region System.Object Overrides

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Guid obj)
        {
            return (ID == obj);
        }
        public bool Equals(string obj)
        {
            return
                InternalCode.Equals(obj, StringComparison.InvariantCultureIgnoreCase) || 
                LocalCode.Equals(obj, StringComparison.InvariantCultureIgnoreCase) || 
                Name.Equals(obj, StringComparison.InvariantCultureIgnoreCase);
        }
        public bool Equals(IBaseObject obj)
        {
            return (ID == obj.ID);
        }
        public override bool Equals(object obj)
        {
            if (obj is Guid) return Equals((Guid) obj);
            if (obj is string) return Equals((string)obj);
            if (obj is IBaseObject) return Equals((IBaseObject)obj);
            return false;
        }
        #endregion
    }
}
