using System;
using System.Linq;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Extensions
{
    internal static class RepositoryPublicationExtensions
    {
        internal static PublicationTimeFrame GetCurrentTimeFrame(this Publication publication)
        {
            return publication.TimeFrames.Single(tf => tf.LineOffFrom <= DateTime.Now && DateTime.Now <= tf.LineOffTo);
        }
    }
}