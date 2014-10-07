using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using DBCar = TME.CarConfigurator.Administration.Car;

namespace TME.CarConfigurator.Publisher
{
    public class Mapper : IMapper
    {
        public IContext Map(String brand, String country, Guid generationID, ICarDbModelGenerationFinder generationFinder, IContext context)
        {
            var data = generationFinder.GetModelGeneration(brand, country, generationID);

            foreach (var entry in data) {
                var contextData = new ContextData();
                var modelGeneration = entry.Value.Item1;
                var model = entry.Value.Item2;
                var language = entry.Key;

                context.ModelGenerations[language] = modelGeneration;
                context.ContextData[language] = contextData;

                // fill contextData
                contextData.Generations.Add(AutoMapper.Mapper.Map<Generation>(modelGeneration));
                contextData.Models.Add(AutoMapper.Mapper.Map<Model>(model));
                
                foreach (var car in modelGeneration.Cars)
                    contextData.Cars.Add(AutoMapper.Mapper.Map<Car>(car));

                foreach (var bodyType in modelGeneration.BodyTypes)
                    contextData.GenerationBodyTypes.Add(AutoMapper.Mapper.Map<BodyType>(bodyType));

                context.TimeFrames[language] = GetTimeFrames(language, context);
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

            var timeFrames = new List<TimeFrame>();

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
                    { 
                        closeDate = point.Date;
                        if (openDate != closeDate)
                            timeFrames.Add(new TimeFrame(openDate.Value, closeDate, new ReadOnlyCollection<Car>(openCars.Select(MapCar).ToList())));
                    }

                    openCars.Add(point.Car);
                    openDate = point.Date;
                }
                else
                {
                    closeDate = point.Date;

                    // time lines with identical from/until can occur when multiple line off dates fall on the same point
                    // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
                    if (openDate != closeDate)
                        timeFrames.Add(new TimeFrame(openDate.Value, closeDate, new ReadOnlyCollection<Car>(openCars.Select(MapCar).ToList())));

                    openCars.Remove(point.Car);
                    openDate = openCars.Any() ? (DateTime?)point.Date : null;
                }
            }

            return timeFrames;
        }
    }
}
