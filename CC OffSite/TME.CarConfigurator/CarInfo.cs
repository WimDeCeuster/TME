using System;
using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class CarInfo : ICarInfo
    {
        readonly Repository.Objects.CarInfo _repositoryCarInfo;

        public CarInfo(Repository.Objects.CarInfo repositoryCarInfo)
        {
            if (repositoryCarInfo == null) throw new ArgumentNullException("repositoryCarInfo");

            _repositoryCarInfo = repositoryCarInfo;
        }

        public int ShortID
        {
            get { return _repositoryCarInfo.ShortID; }
        }

        public string Name
        {
            get { return _repositoryCarInfo.Name; }
        }
    }
}
