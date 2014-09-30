using System;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator.Core
{
    public abstract class BaseObject : IBaseObject
    {

        #region Dependencies (Adapter)

        private Repository.Objects.Core.BaseObject RepositoryBaseObject { get; set; }

        #endregion

        #region Construction

        protected BaseObject(Repository.Objects.Core.BaseObject repositoryBaseObject)
        {
            if (repositoryBaseObject == null) throw new ArgumentNullException("repositoryBaseObject");

            RepositoryBaseObject = repositoryBaseObject;
        }

        #endregion

        #region Properties

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

        #endregion 
    }
}
