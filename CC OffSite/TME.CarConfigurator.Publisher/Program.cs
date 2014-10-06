using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
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

            //MyContext.SetSystemContext("Toyota", "BE", "nl");

            //var aygo2014 = Models.GetModels()["aygo"].Generations.Last();

            //var mapped = AutoMapper.Mapper.Map<Generation>(aygo2014);

            var service = new S3Service("Toyota", "DE", new S3Serialiser());
            var xs = service.GetObjects();

            foreach (var x in xs)
                Console.WriteLine(x.Key);

            Console.ReadLine();
        }
    }
}
