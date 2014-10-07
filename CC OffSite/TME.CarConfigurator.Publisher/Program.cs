using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Repository.Objects;
using Models = TME.CarConfigurator.Administration.Models;

namespace TME.CarConfigurator.Publisher
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AutoMapperConfig.Configure();

            MyContext.SetSystemContext("Toyota", "DE", "de");

            var multiTimeFrameGens = new Dictionary<ModelGeneration, Tuple<Int32, Int32>>();

            var genId = Guid.Parse("0b6b6f08-ca5f-4ef9-8720-9a1e033f1276");
            var cars = ModelGeneration.GetModelGeneration(genId).Cars;

            var timeFrames = new[] {
                Tuple.Create(new DateTime(2014, 1, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 2, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 2, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 2, 1), new DateTime(2014, 5, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 3, 1), new DateTime(2014, 4, 1)),
                Tuple.Create(new DateTime(2014, 5, 1), new DateTime(2014, 6, 1)),
                Tuple.Create(new DateTime(2014, 7, 1), new DateTime(2014, 8, 1)),
                Tuple.Create(new DateTime(2014, 9, 1), new DateTime(2014, 11, 1)),
                Tuple.Create(new DateTime(2014, 10, 1), new DateTime(2014, 12, 1))
            };
            
            foreach (var entry in cars.Zip(timeFrames, Tuple.Create))
            {
                var car = entry.Item1;
                var lineOffFrom = entry.Item2.Item1;
                var lineOffTo = entry.Item2.Item2;
            
                car.LineOffFromDate = lineOffFrom;
                car.LineOffToDate = lineOffTo;
            }

            foreach (var car in cars)
                Console.WriteLine("{0} - {1}", car.LineOffFromDate, car.LineOffToDate);


            //Models.GetModels().First().Generations.First().Cars.Add()

            //foreach (var model in Models.GetModels())
            //foreach (var gen in Models.GetModels()["aygo"].Generations)
            //    multiTimeFrameGens.Add(gen, Tuple.Create(gen.Cars.Count, gen.Cars.Select(car => Tuple.Create(car.LineOffFromDate, car.LineOffToDate)).Distinct().Count()));

            //foreach (var entry in multiTimeFrameGens.Where(entry => entry.Value.Item1 > 1))
            //    Console.WriteLine("{0} ({1})<{4}>: {2} - {3}", entry.Key.Name, entry.Key.Model.Name, entry.Value.Item1, entry.Value.Item2, entry.Key.ID);

            Console.ReadLine();


            //var aygo2014 = Models.GetModels()["aygo"].Generations.Last();
            //
            //var mapped = AutoMapper.Mapper.Map<Generation>(aygo2014);



            var service = new S3Service("Toyota", "DE", new S3Serialiser());
            var xs = service.GetObjects();
            
            foreach (var x in xs) { 
                Console.WriteLine(x.Key);
            }

            Console.WriteLine();
            Console.Write("Delete all? ");
            if (Console.ReadLine().Equals("y", StringComparison.InvariantCultureIgnoreCase)) {
                service.DeleteAll();
                Console.WriteLine("All cleared");
                Console.ReadLine();
            }
    
        }
    }
}
