using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;
using TME.CarConfigurator.Publisher.Common.Enums;

namespace TME.CarConfigurator.AutoComparer
{
    class Options
    {
        [Option('b', "brand", DefaultValue = "Toyota",
            HelpText = "The brand of the publication.")]
        public string Brand { get; set; }

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
