using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Constants
{
    public static class Constant
    {
        // Data that gets modified on the run : should not be changed between versions
        public const string EnabledChannelsFilePath = "../_Content_lol-facts/EnabledChannels.csv";
        public const string ChangelogChannelsFilePath = "../_Content_lol-facts/ChangelogChannels.csv";

        // Permanent data that changes with the version, not as the program runs
        public const string FactsFilePath = "./Content/Facts.csv";
        public const string RawFactsPath = "./Content/RawFacts/";

        public const string ChangelogPath = "./Content/Changelogs/";
        public const string ChangelogArchivesPath = $"{ChangelogPath}Archives/";

        public static readonly CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null,
            Delimiter = ";",
        };
    }
}
