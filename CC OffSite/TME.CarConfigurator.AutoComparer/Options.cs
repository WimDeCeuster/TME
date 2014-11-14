using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.AutoComparer
{
    class Options
    {
        [Option('b', "brand", DefaultValue = "Toyota",
            HelpText = "The brand of the publication.")]
        public string Brand { get; set; }

        [Option('e', "environment", DefaultValue = "Development",
            HelpText = "Environment to look in for publications.")]
        public string Environment { get; set; }

        [Option('t', "target", DefaultValue = "S3",
            HelpText = "Target to look in forpublications.")]
        public string Target { get; set; }

        [Option('d', "datasubset", DefaultValue = PublicationDataSubset.Live,
            HelpText = "The data subset of the publication.")]
        public PublicationDataSubset DataSubset { get; set; }

        [ValueList(typeof(List<string>))]
        public IList<string> Countries { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
