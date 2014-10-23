using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Core
{
    public abstract class BaseObject<T> : IBaseObject where T : Repository.Objects.Core.BaseObject
    {
        protected readonly T RepositoryObject;
        private IEnumerable<ILabel> _labels;

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

        public IEnumerable<ILabel> Labels
        {
            get { return _labels = _labels ?? RepositoryObject.Labels.Select(label => new Label(label)).ToArray(); }
        }

        protected BaseObject(T repositoryObject)
        {
            if (repositoryObject == null) throw new ArgumentNullException("repositoryObject");

            RepositoryObject = repositoryObject;
        }
    }
}
