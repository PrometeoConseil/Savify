using CommandLine;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CommandLine.Text;
using Savify.Core;

namespace Savify.CommandLine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Savify";
            var parser = new Parser(config => config.HelpWriter = null);
            var result = parser.ParseArguments<Options>(args);
            result.WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    if (error.Tag != ErrorType.HelpRequestedError &&
                        error.Tag != ErrorType.VersionRequestedError) continue;

                    Console.WriteLine(BuildHelp(result));
                    Environment.Exit(0);
                }

                var myHelpText = HelpText.AutoBuild(result, onError => BuildHelp(result), onExample => onExample);
                Console.Error.WriteLine(myHelpText);
                Environment.Exit(1);
            });

            var parsedResult = ((Parsed<Options>)result).Value;
            var target = string.IsNullOrEmpty(parsedResult.TargetFolder) ? "." : parsedResult.TargetFolder;
            var listName = Guid.NewGuid().ToString();

            SpotifyAgent.Auth();

            var link = new Uri(parsedResult.Url);
            var newList = DownloadManager.NewList(listName, new Uri(target));
            DownloadManager.AddTracksLink(link, newList);

            newList.TrackFormat = parsedResult.OutputFormat;
            newList.TrackQuality = parsedResult.Quality;

            DownloadManager.EnqueueList(newList);
            DownloadManager.DequeueList(newList);

            Console.WriteLine("***** PRESS ANY KEY TO EXIT ***** ");
            Console.ReadLine();
        }

        private static HelpText BuildHelp(ParserResult<Options> result)
        {
            var assembly = typeof(Program).GetTypeInfo().Assembly;

            var assemblyTitleAttribute = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute)).SingleOrDefault() as AssemblyTitleAttribute;
            var assemblyCopyrightAttribute = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute)).SingleOrDefault() as AssemblyCopyrightAttribute;
            var assemblyCompanyAttribute = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute)).SingleOrDefault() as AssemblyCompanyAttribute;
            var assemblyDescriptionAttribute = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute)).SingleOrDefault() as AssemblyDescriptionAttribute;
            var version = assembly.GetName().Version.ToString().ToString(CultureInfo.InvariantCulture);

            var nHelpText = new HelpText(SentenceBuilder.Create(), $"{assemblyTitleAttribute?.Title} {version}"
                                                                   + $"{(assemblyCopyrightAttribute == null && assemblyCompanyAttribute == null ? "" : "\r\n" + (assemblyCopyrightAttribute?.Copyright))} {assemblyCompanyAttribute?.Company}"
                                                                   + $"{((assemblyDescriptionAttribute == null) ? "" : "\r\n" + assemblyDescriptionAttribute.Description)}")
            {
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true,
                MaximumDisplayWidth = 4000,
                AddEnumValuesToHelpText = true
            };
            nHelpText.AddOptions(result);
            return HelpText.DefaultParsingErrorsHandler(result, nHelpText);
        }
    }
}

