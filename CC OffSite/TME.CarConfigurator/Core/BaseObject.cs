using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Core;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Core
{
    public abstract class BaseObject : IBaseObject
    {
        protected readonly Repository.Objects.Core.BaseObject RepositoryBaseObject;
        protected readonly Context Context;

        public Guid ID
        {
            get { return RepositoryBaseObject.ID; }
        }

        public string InternalCode
        {
            get { return RepositoryBaseObject.InternalCode; }
        }

        public string LocalCode
        {
            get { return RepositoryBaseObject.LocalCode; }
        }

        public string Name
        {
            get { return RepositoryBaseObject.Name; }
        }

        public string Description
        {
            get { return RepositoryBaseObject.Description; }
        }

        public string FootNote
        {
            get { return RepositoryBaseObject.FootNote; }
        }

        public string ToolTip
        {
            get { return RepositoryBaseObject.ToolTip; }
        }

        public int SortIndex
        {
            get { return RepositoryBaseObject.SortIndex; }
        }

        public IEnumerable<ILabel> Labels
        {
            get { throw new NotImplementedException(); }
        }

        protected BaseObject(Repository.Objects.Core.BaseObject repositoryBaseObject, Context context)
        {
            if (repositoryBaseObject == null) throw new ArgumentNullException("repositoryBaseObject");
            if (context == null) throw new ArgumentNullException("context");

            Context = context;
            RepositoryBaseObject = repositoryBaseObject;
        }
    }
}
