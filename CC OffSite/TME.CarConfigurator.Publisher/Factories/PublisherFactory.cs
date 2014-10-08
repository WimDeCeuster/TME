using Spring.Context;
using Spring.Context.Support;
using System;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.Publisher.Factories
{
    public class PublisherFactory : IPublisherFactory
    {
        public IPublisher Get(IS3Service service)
        {
            var springContext = ContextRegistry.GetContext();
            var serialiser = springContext.CreateObject<IS3Serialiser>("S3Serialiser");
            var publicationService = springContext.CreateObject<IS3PublicationService>("S3PublicationService", service, serialiser);
            var languageService = springContext.CreateObject<IS3LanguageService>("S3LanguageService", service, serialiser);
            var bodyTypeService = springContext.CreateObject<IS3BodyTypeService>("S3BodyTypeService", service, serialiser);
            return springContext.CreateObject<IPublisher>("Publisher", publicationService, languageService, bodyTypeService);
        }
    }

    public static class SpringExtension
    {
        public static T CreateObject<T>(this IApplicationContext context, String name, params object[] args)
        {
            return (T)context.CreateObject(name, typeof(T), args);
        }
    }
}
