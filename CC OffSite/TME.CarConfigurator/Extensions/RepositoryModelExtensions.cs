using System.Linq;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Extensions
{
    internal static class RepositoryModelExtensions
    {
        internal static PublicationInfo GetActivePublicationInfo(this Repository.Objects.Model repositoryModel)
        {
            return repositoryModel.Publications.Single(p => p.State == PublicationState.Activated);
        }
    }
}