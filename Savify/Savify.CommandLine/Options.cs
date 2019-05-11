using CommandLine;

namespace Savify.CommandLine
{
    public class Options
    {
        [Option('u', "URL", Required = true, HelpText = "The shared URL to grab")]
        public string  Url { get; set; }

        [Option('t', "Target", HelpText = "The target folder")]
        public string  TargetFolder { get; set; }

        [Option('f', "Format", HelpText = "The output format", Default = Core.Format.mp3)]
        public Core.Format OutputFormat { get; set; }

        [Option('q', "Quality", HelpText = "The output quality", Default = Core.Quality._320kbps)]
        public Core.Quality Quality { get; set; }

    }
}
