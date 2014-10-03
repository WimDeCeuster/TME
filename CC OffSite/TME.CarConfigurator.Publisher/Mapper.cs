using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;
using ModelGeneration = TME.CarConfigurator.Administration.ModelGeneration;
using DBCar = TME.CarConfigurator.Administration.Car;

namespace TME.CarConfigurator.Publisher
{
    public interface IMapper
    {
        IContext Map(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context);
    }

    public class Mapper : IMapper
    {
        public IContext Map(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context)
        {
            var data = generationFinder.GetModelGeneration(brand, country, generationID);

            foreach (var entry in data) {
                var contextData = new ContextData();
                var modelGeneration = entry.Value;
                var language = entry.Key;

                context.ModelGenerations[language] = modelGeneration;
                context.ContextData[language] = contextData;
                context.TimeFrames[language] = GetTimeFrames(language, context);

                // fill contextData
                contextData.Generations.Add(AutoMapper.Mapper.Map<Generation>(modelGeneration));
            }

            return context;
        }

        public IReadOnlyList<TimeFrame> GetTimeFrames(String language, IContext context)
        {
            var generation = context.ModelGenerations[language];
            var cars = context.ContextData[language].Cars;

            //For preview, return only 1 Min/Max TimeFrame with all cars
            if (context.DataSubset == PublicationDataSubset.Preview)
                return new List<TimeFrame> { new TimeFrame(DateTime.MinValue, DateTime.MaxValue, cars.ToList()) };

            var TimeFrames = new List<TimeFrame>();

            var timeProjection = generation.Cars.SelectMany(car => new[] {
                                                    new { Date = car.LineOffFromDate, Open = true, Car = car },
                                                    new { Date = car.LineOffToDate, Open = false, Car = car }
                                                })
                                                .OrderBy(point => point.Date);

            Func<DBCar, Car> MapCar = dbCar => cars.Single(car => car.ID == dbCar.ID);

            var openCars = new List<DBCar>();
            DateTime? openDate = null;
            DateTime closeDate;
            foreach (var point in timeProjection)
            {
                if (point.Open)
                {
                    if (openDate != null)
                        closeDate = point.Date;

                    openCars.Add(point.Car);
                    openDate = point.Date;
                }
                else
                {
                    closeDate = point.Date;

                    // time lines with identical from/until can occur when multiple line off dates fall on the same point
                    // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
                    if (openDate != closeDate)
                        TimeFrames.Add(new TimeFrame(openDate.Value, closeDate, new ReadOnlyCollection<Car>(openCars.Select(MapCar).ToList())));

                    openCars.Remove(point.Car);
                    openDate = openCars.Any() ? (DateTime?)point.Date : null;
                }
            }

            return TimeFrames;
        }
    }

    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            AutoMapper.Mapper.CreateMap<TME.CarConfigurator.Administration.ModelGenerationCarConfiguratorVersion, CarConfiguratorVersion>();

            AutoMapper.Mapper.CreateMap<ModelGeneration, Generation>()
                             .ForMember(generation => generation.Links,
                                        opt => opt.Ignore())
                             .ForMember(generation => generation.Assets,
                                        opt => opt.Ignore())
                             .ForMember(generation => generation.SSNs,
                                        opt => opt.MapFrom(modelGeneration =>
                                            modelGeneration.FactoryGenerations.Select(factoryGeneration => factoryGeneration.SSN).ToList()));
        }
    }
}
