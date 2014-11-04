using System;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Equipment
{
    public class BestVisibleIn : IBestVisibleIn
    {
        private readonly Repository.Objects.Equipment.BestVisibleIn _repositoryBestVisibleIn;

        public BestVisibleIn(Repository.Objects.Equipment.BestVisibleIn repositoryBestVisibleIn)
        {
            if (repositoryBestVisibleIn == null) throw new ArgumentNullException("repositoryBestVisibleIn");
            _repositoryBestVisibleIn = repositoryBestVisibleIn;
        }

        public string Mode { get { return _repositoryBestVisibleIn.Mode; }}
        public string View { get { return _repositoryBestVisibleIn.View; }}
        public int Angle { get { return _repositoryBestVisibleIn.Angle; } }
    }
}