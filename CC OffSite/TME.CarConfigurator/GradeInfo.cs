using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class GradeInfo : IGradeInfo
    {
    
        readonly Repository.Objects.GradeInfo _repositoryGradeInfo;

         public GradeInfo(Repository.Objects.GradeInfo repositoryGradeInfo)
        {
            if (repositoryGradeInfo == null) throw new ArgumentNullException("repositoryGradeInfo");

            _repositoryGradeInfo = repositoryGradeInfo;
        }

        public Guid ID
        {
            get { return _repositoryGradeInfo.ID; }
        }

        public string Name
        {
            get { return _repositoryGradeInfo.Name; }
        }
    }
}
