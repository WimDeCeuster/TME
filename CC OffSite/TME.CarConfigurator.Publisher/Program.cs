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
            var aygo = Models.GetModels()["aygo"];
            var aygo2014 = aygo.Generations.Last();
            //
            //var mapped = AutoMapper.Mapper.Map<Generation>(aygo2014);

            var context = new Context("Toyota", "DE", aygo2014.ID, Enums.PublicationDataSubset.Live);
            var mapper = new Mapper();
            mapper.Map("Toyota", "DE", aygo2014.ID, new CarDbModelGenerationFinder(), context);


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
