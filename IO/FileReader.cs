using lol_facts.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lol_facts.Constants;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace lol_facts.IO
{
    public static class FileReader
    {
        public static void CreateFilesIfNotExist()
        {
            if (!File.Exists(Constant.EnabledChannelsFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Constant.EnabledChannelsFilePath));
                File.Create(Constant.EnabledChannelsFilePath);
            }

            if (!File.Exists(Constant.ChangelogChannelsFilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Constant.ChangelogChannelsFilePath));
                File.Create(Constant.ChangelogChannelsFilePath);
            }
        }

        /// <summary>
        /// Generate the Facts file at the start of the program from all RawFacts
        /// </summary>
        internal static void GenerateFactFile()
        {
            var rawFactsFiles = Directory.GetFiles(Constant.RawFactsPath, string.Empty, SearchOption.AllDirectories);

            var facts = string.Empty;

            foreach (var rawFactFile in rawFactsFiles)
            {
                string fact = string.Join('\n', File.ReadAllLines(rawFactFile).ToList().Skip(1)) + '\n';

                if (!string.IsNullOrWhiteSpace(fact))
                {
                    facts += fact;
                }
            }

            File.WriteAllText(Constant.FactsFilePath, facts);
        }

        /// <summary>
        /// Reads the last available change log
        /// </summary>
        /// <returns></returns>
        internal static string ReadLastAvailableChangelog()
        {
            var changelogs = Directory.GetFiles(Constant.ChangelogPath, string.Empty, SearchOption.TopDirectoryOnly).ToList();

            string result = string.Empty;

            if (changelogs == null || changelogs.Count == 0)
            {
                Console.WriteLine("Aucune fichier Changelog trouvé.");
            }

            // If there are multiple changelogs, we write them all
            foreach (var changelog in changelogs)
            {
                Console.WriteLine($"Fichier Changelog trouvé : '{changelog}'.");

                // Notes the version of the file
                result += Path.GetFileNameWithoutExtension(changelog) + "\n\n";

                result += string.Join("\n", File.ReadLines(changelog));
            }

            return result;
        }

        /// <summary>
        /// Returns all facts from the facts file
        /// </summary>
        /// <returns></returns>
        public static List<Fact> ReadAllFacts()
        {
            using var reader = new StreamReader(Constant.FactsFilePath);
            using var csv = new CsvReader(reader, Constant.csvConfiguration);
            var result = csv.GetRecords<Fact>().ToList();

            return result;
        }

        /// <summary>
        /// Returns true if the channelId parameter is found in the enabled channels file
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public static bool IsChannelEnabled(ulong channelId)
        {
            return ReadAllEnabledChannels().Contains(channelId.ToString());
        }

        /// <summary>
        /// Adds a channel id to the enabled channels file
        /// </summary>
        /// <param name="channelId"></param>
        public static void AddEnableChannel(ulong channelId)
        {
            var enabledChannels = ReadAllEnabledChannels();
            enabledChannels.Add(channelId.ToString());

            File.WriteAllLines(Constant.EnabledChannelsFilePath, enabledChannels.Distinct());
        }

        /// <summary>
        /// Removes a channel id to the enabled channels file
        /// </summary>
        /// <param name="channelId"></param>
        public static void RemoveEnableChannel(ulong channelId)
        {
            var enabledChannels = ReadAllEnabledChannels();
            enabledChannels.RemoveAll(ec => ec == channelId.ToString());

            File.WriteAllLines(Constant.EnabledChannelsFilePath, enabledChannels);
        }

        /// <summary>
        /// Adds a channel id to the enabled changelogs file
        /// </summary>
        /// <param name="channelId"></param>
        public static void AddEnableChangelog(ulong channelId)
        {
            var enabledChangelogChannels = ReadAllEnabledChangelogChannels();
            enabledChangelogChannels.Add(channelId.ToString());

            File.WriteAllLines(Constant.ChangelogChannelsFilePath, enabledChangelogChannels.Distinct());
        }

        /// <summary>
        /// Removes a channel id to the enabled changelogs file
        /// </summary>
        /// <param name="channelId"></param>
        public static void RemoveEnableChangelog(ulong channelId)
        {
            var enabledChangelogChannels = ReadAllEnabledChangelogChannels();
            enabledChangelogChannels.RemoveAll(ec => ec == channelId.ToString());

            File.WriteAllLines(Constant.ChangelogChannelsFilePath, enabledChangelogChannels);
        }

        /// <summary>
        /// Reads all enabled changelog channels
        /// </summary>
        /// <returns></returns>
        public static List<string> ReadAllEnabledChangelogChannels()
        {
            var enabledChangelogChannels = File.ReadAllLines(Constant.ChangelogChannelsFilePath).ToList();

            return enabledChangelogChannels;
        }

        /// <summary>
        /// Moves the change log files to the archives folder so they won't be send again
        /// </summary>
        internal static void ArchiveChangelogs()
        {
            var changelogs = Directory.GetFiles(Constant.ChangelogPath, string.Empty, SearchOption.TopDirectoryOnly).ToList();

            foreach (var changelog in changelogs)
            {
                File.Move(changelog, Path.Combine(Constant.ChangelogArchivesPath, Path.GetFileName(changelog)), true);
            }
        }

        #region Private methods

        /// <summary>
        /// Converts a list of FactsWithUnknownTag objects to a csv file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> FromFactsWithUnknownTagToCsv(List<FactsSearchLogged> data)
        {
            List<string> result = data.Select(d => $"{d.UserName};{d.Mention};{d.Tag};{d.Count};{d.IsCorrectSearch}").ToList();

            return result;
        }

        /// <summary>
        /// Reads all enabled channels
        /// </summary>
        /// <returns></returns>
        private static List<string> ReadAllEnabledChannels()
        {
            var enabledChannels = File.ReadAllLines(Constant.EnabledChannelsFilePath)
                                   .ToList();

            return enabledChannels;
        }

        #endregion
    }
}
