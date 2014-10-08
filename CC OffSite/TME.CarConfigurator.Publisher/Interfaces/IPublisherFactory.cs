using System;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IPublisherFactory
    {
        IPublisher Get(String target);
    }
}