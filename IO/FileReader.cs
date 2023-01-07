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
            List<string> paths = new()
            {
                Constant.EnabledChannelsFilePath,
                Constant.ChangelogChannelsFilePath,
                Constant.TimerMessagePath,
            };

            foreach (var path in paths)
            {
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.Create(path);
                }
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
        /// NOT CALLED IN DEBUG
        /// </summary>
        public static void ArchiveChangelogs()
        {
            var changelogs = Directory.GetFiles(Constant.ChangelogPath, string.Empty, SearchOption.TopDirectoryOnly).ToList();

            foreach (var changelog in changelogs)
            {
                File.Move(changelog, Path.Combine(Constant.ChangelogArchivesPath, Path.GetFileName(changelog)), true);
            }
        }

        #region Timers

        /// <summary>
        /// Add a timer to the log files
        /// </summary>
        /// <param name="timerMessage"></param>
        public static void AddTimer(TimerMessage timerMessage)
        {
            var data = ReadAllTimers();
            data.Add(timerMessage);

            File.WriteAllLines(Constant.TimerMessagePath, data.Select(d => $"{d.Id};{d.DiscordChannelId};{d.Day};{d.Hour}"));
        }

        /// <summary>
        /// Read all timers in the csv file
        /// </summary>
        /// <returns></returns>
        public static List<TimerMessage> ReadAllTimers()
        {
            List<TimerMessage> data;

            using (var reader = new StreamReader(Constant.TimerMessagePath))
            using (var csv = new CsvReader(reader, Constant.csvConfiguration))
            {
                data = csv.GetRecords<TimerMessage>().ToList();
            }

            return data;
        }

        /// <summary>
        /// Returns the FileStream of the timer file
        /// </summary>
        /// <returns></returns>
        public static FileStream GetTimerFile()
        {
            FileStream fsSource = new(Constant.TimerMessagePath, FileMode.Open, FileAccess.Read);
            return fsSource;
        }

        /// <summary>
        /// Delete a timer in the timer file
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveLineFromId(uint id)
        {
            var data = ReadAllTimers();
            data.RemoveAll(d => d.Id == id);

            File.WriteAllLines(Constant.TimerMessagePath, data.Select(d => $"{d.Id};{d.DiscordChannelId};{d.Day};{d.Hour}"));
        }

        #endregion

        #region Private methods

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
