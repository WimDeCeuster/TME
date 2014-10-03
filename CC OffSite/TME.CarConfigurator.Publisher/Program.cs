using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects;
using Models = TME.CarConfigurator.Administration.Models;

namespace TME.CarConfigurator.Publisher
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            AutoMapperConfig.Configure();

            MyContext.SetSystemContext("Toyota", "BE", "nl");

            var aygo2014 = Models.GetModels()["aygo"].Generations.Last();

            var mapped = AutoMapper.Mapper.Map<Generation>(aygo2014);

            Console.ReadLine();
        }
    }
}
