using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lol_facts.Constants
{
    public static class Constant
    {
        public const string EnabledChannelsFilePath = "./Content/EnabledChannels.csv";

        public const string FactsFilePath = "./Content/Facts.csv";
        public const string FactsWithUnknownTagFilePath = "./Content/FactsWithUnknownTag.csv";
        public const string RawFactsPath = "./Content/RawFacts/";

        public const string ChangelogChannelsFilePath = "./Content/ChangelogChannels.csv";
        public const string ChangelogPath = "./Content/Changelogs/";
        public const string ChangelogArchivesPath = $"{ChangelogPath}Archives/";
    }
}
