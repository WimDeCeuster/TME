using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Enums;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Exceptions;
using TME.CarConfigurator.Publisher.Extensions;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Repository.Objects.Packs;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class TimeFrameMapper : ITimeFrameMapper
    {
        public IReadOnlyList<TimeFrame> GetTimeFrames(String language, IContext context)
        {
            var contextData = context.ContextData[language];
            var cars = contextData.Cars;

            //For preview, return only 1 Min/Max TimeFrame with all data
            if (context.DataSubset == PublicationDataSubset.Preview)
                return new List<TimeFrame> { GetPreviewTimeFrame(contextData) };

            var timeFrames = new List<TimeFrame>();

            var timeProjection = context.ModelGenerations[language]
                                        .Cars.Where(car => car.Approved)
                                        .SelectMany(car => new[] {
                                            new { Date = car.LineOffFromDate, Open = true, Car = car },
                                            new { Date = car.LineOffToDate, Open = false, Car = car }
                                        })
                                        .OrderBy(point => point.Date);


            var openCars = new List<Administration.Car>();

            DateTime? openDate = null;
            foreach (var point in timeProjection)
            {
                DateTime closeDate;
                if (point.Open)
                {
                    if (openDate != null)
                    {
                        closeDate = point.Date;
                        AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, context.ContextData[language]);
                    }

                    openCars.Add(point.Car);
                    openDate = point.Date;

                    continue;
                }

                closeDate = point.Date;

                AddTimeFrameIfRelevant(openDate, closeDate, timeFrames, openCars, context.ContextData[language]);

                openCars.Remove(point.Car);
                openDate = openCars.Any() ? (DateTime?)point.Date : null;
            }

            return timeFrames;
        }

        static void AddTimeFrameIfRelevant(DateTime? openDate, DateTime closeDate, ICollection<TimeFrame> timeFrames, IReadOnlyList<Administration.Car> timeFrameCars, ContextData contextData)
        {
            // time lines with identical from/until can occur when multiple line off dates fall on the same point
            // these "empty" time lines can simply be ignored (though the openCars logic is still relevant)
            if (openDate == closeDate) return;

            if (openDate == null)
                throw new CorruptDataException("The open date could not be retrieved, could not create timeframe");

            timeFrames.Add(GetTimeFrame(openDate.Value, closeDate, timeFrameCars, contextData));
        }

        static TimeFrame GetPreviewTimeFrame(ContextData contextData)
        {
            return new TimeFrame(
                        DateTime.MinValue,
                        DateTime.MaxValue,
                        contextData.Cars.ToList(),
                        contextData.BodyTypes.ToList(),
                        contextData.Engines.ToList(),
                        contextData.WheelDrives.ToList(),
                        contextData.Transmissions.ToList(),
                        contextData.Steerings.ToList(),
                        contextData.Grades.ToList(),
                        contextData.GradeEquipment.ToDictionary(),
                        contextData.GradePacks.ToDictionary(),
                        contextData.SubModels.ToList(),
                        contextData.ColourCombinations.ToList(),
                        contextData.EquipmentCategories.ToList());
        }

        static TimeFrame GetTimeFrame(DateTime openDate, DateTime closeDate, IReadOnlyList<Administration.Car> timeFrameCars, ContextData contextData)
        {
            var cars = contextData.Cars.Where(CarMatches(timeFrameCars)).ToList();
            var bodyTypes = contextData.BodyTypes.Where(BodyTypeIsPresentOn(timeFrameCars)).ToList();
            var engines = contextData.Engines.Where(EngineIsPresentOn(timeFrameCars)).ToList();
            var wheelDrives = contextData.WheelDrives.Where(WheelDriveIsPresentOn(timeFrameCars)).ToList();
            var transmissions = contextData.Transmissions.Where(TransmissionIsPresentOn(timeFrameCars)).ToList();
            var steerings = contextData.Steerings.Where(SteeringIsPresentOn(timeFrameCars)).ToList();
            var grades = contextData.Grades.Where(GradeIsPresentOn(timeFrameCars)).ToList();
            var subModels = contextData.SubModels.Where(SubModelIsPresentOn(timeFrameCars)).ToList();
            var gradeEquipments = FilterGradeEquipments(contextData.GradeEquipment, timeFrameCars);
            var colourCombinations = contextData.ColourCombinations.Where(ColourCombinationIsPresentOn(timeFrameCars)).ToList();
            var gradePacks = FilterGradePacks(contextData.GradePacks, timeFrameCars);
            var equipmentCategories = contextData.EquipmentCategories.ToList();

            return new TimeFrame(
                openDate,
                closeDate,
                cars,
                bodyTypes,
                engines,
                wheelDrives,
                transmissions,
                steerings,
                grades,
                gradeEquipments,
                gradePacks,
                subModels,
                colourCombinations,
                equipmentCategories);
        }

        private static IReadOnlyDictionary<Guid, IList<GradePack>> FilterGradePacks(IEnumerable<KeyValuePair<Guid, IList<GradePack>>> gradePacks, IEnumerable<Administration.Car> cars)
        {
            return gradePacks
                .Where(entry => cars.Any(c => c.GradeID == entry.Key))
                .ToDictionary(entry => entry.Key, entry => GetFilteredGradePacks(entry.Value, cars));
        }

        private static IList<GradePack> GetFilteredGradePacks(IEnumerable<GradePack> gradePacks, IEnumerable<Administration.Car> cars)
        {
            return gradePacks
                .Where(gradePack => cars.Any(c => c.Packs[gradePack.ID] != null))
                .ToList();
        }

        static IReadOnlyList<T> FilterGradeEquipmentItems<T>(IEnumerable<T> gradeEquipmentItems, IEnumerable<Administration.Car> cars)
            where T : EquipmentItem
        {
            return gradeEquipmentItems.Where(ItemIsPresentOnA(cars)).Cast<T>().ToList();
        }

        static GradeEquipment GetFilteredGradeEquipment(GradeEquipment gradeEquipment, IReadOnlyList<Administration.Car> cars)
        {
            return new GradeEquipment
            {
                Accessories = FilterGradeEquipmentItems(gradeEquipment.Accessories, cars),
                Options = FilterGradeEquipmentItems(gradeEquipment.Options, cars)
            };
        }

        static IReadOnlyDictionary<Guid, GradeEquipment> FilterGradeEquipments(IDictionary<Guid, GradeEquipment> gradeEquipments, IReadOnlyList<Administration.Car> cars)
        {
            return gradeEquipments.Where(entry => cars.Any(dbCar => dbCar.GradeID == entry.Key))
                                  .ToDictionary(
                                      entry => entry.Key,
                                      entry => GetFilteredGradeEquipment(entry.Value, cars));
        }

        static Func<EquipmentItem, Boolean> ItemIsPresentOnA(IEnumerable<Administration.Car> cars)
        {
            return item => cars.Any(car => car.Equipment.Any(equipment => equipment.ID == item.ID && equipment.Availability != Administration.Enums.Availability.NotAvailable));
        }

        static Func<Car, Boolean> CarMatches(IReadOnlyList<Administration.Car> timeFrameCars)
        {
            return car => timeFrameCars.Any(timeFrameCar => timeFrameCar.ID == car.ID);
        }

        static Func<BodyType, Boolean> BodyTypeIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return bodyType => cars.Any(car => car.BodyTypeID == bodyType.ID);
        }

        static Func<Engine, Boolean> EngineIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return engine => cars.Any(car => car.EngineID == engine.ID);
        }

        static Func<WheelDrive, Boolean> WheelDriveIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return wheelDrive => cars.Any(car => car.WheelDriveID == wheelDrive.ID);
        }

        static Func<Transmission, Boolean> TransmissionIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return transmission => cars.Any(car => car.TransmissionID == transmission.ID);
        }

        static Func<Steering, Boolean> SteeringIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return steering => cars.Any(car => car.SteeringID == steering.ID);
        }

        static Func<Grade, Boolean> GradeIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return grade => cars.Any(car => car.GradeID == grade.ID);
        }

        static Func<SubModel, Boolean> SubModelIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return subModel => cars.Any(car => car.SubModelID == subModel.ID);
        }

        private static Func<ColourCombination, Boolean> ColourCombinationIsPresentOn(IEnumerable<Administration.Car> cars)
        {
            return colourCombination => cars.Any(car => car.ColourCombinations[colourCombination.ExteriorColour.ID, colourCombination.Upholstery.ID] != null);
        }
    }
}
