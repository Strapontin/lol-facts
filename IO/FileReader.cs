using lol_facts.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lol_facts.Constants;

namespace lol_facts.IO
{
    public static class FileReader
    {
        public static void CreateFilesIfNotExist()
        {
            if (!File.Exists(Constant.EnabledChannelsFilePath))
                File.Create(Constant.EnabledChannelsFilePath);

            if (!File.Exists(Constant.FactsWithUnknownTagFilePath))
                File.Create(Constant.FactsWithUnknownTagFilePath);
        }

        /// <summary>
        /// Returns all facts from the facts file
        /// </summary>
        /// <returns></returns>
        public static List<Fact> ReadAllFacts()
        {
            var result = File.ReadAllLines(Constant.FactsFilePath)
                             .Skip(1)
                             .Select(content => FromCsvToFact(content))
                             .ToList();

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
        /// Adds an incorrect search (tag non-existing) to the fact command log
        /// </summary>
        /// <param name="username"></param>
        /// <param name="mention"></param>
        /// <param name="tag"></param>
        /// <param name="isCorrectSearch">Allows us to discriminate if the searched tag already exists or not</param>
        public static void AddIncorrectSearch(string username, string mention, string tag, bool isCorrectSearch)
        {
            var data = File.ReadAllLines(Constant.FactsWithUnknownTagFilePath)
                             .Select(content => FromCsvToFactsWithUnknownTag(content))
                             .ToList();

            var element = data.Where(d => d.UserName == username && d.Mention == mention && d.Tag == tag && d.IsCorrectSearch == isCorrectSearch).FirstOrDefault();

            if (element == null)
            {
                data.Add(new FactsWithUnknownTag()
                {
                    UserName = username,
                    Mention = mention,
                    Tag = tag,
                    Count = 1,
                    IsCorrectSearch = isCorrectSearch,
                });
            }
            else
            {
                element.Count++;
            }

            File.WriteAllLines(Constant.FactsWithUnknownTagFilePath, FromFactsWithUnknownTagToCsv(data));
        }

        /// <summary>
        /// Reads all stats from the fact request
        /// </summary>
        public static List<FactsWithUnknownTag> ReadAllStats()
        {
            var data = File.ReadAllLines(Constant.FactsWithUnknownTagFilePath)
                             .Select(content => FromCsvToFactsWithUnknownTag(content))
                             .ToList();

            return data;
        }

        #region Private methods

        /// <summary>
        /// Converts a csv line to a Fact object
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Fact FromCsvToFact(string content)
        {
            var values = content.Split(';');

            Fact result = new Fact()
            {
                Text = values[0],
                OriginalText = values[1],
                Tags = values[2].Split(',').ToList(),
                Url = values[3],
            };

            return result;
        }

        /// <summary>
        /// Converts a csv file to a FactsWithUnknownTag object
        /// </summary>
        /// <param name="content"></param>
        private static FactsWithUnknownTag FromCsvToFactsWithUnknownTag(string content)
        {
            var values = content.Split(';');

            FactsWithUnknownTag result = new FactsWithUnknownTag()
            {
                UserName = values[0],
                Mention = values[1],
                Tag = values[2],
                Count = int.Parse(values[3]),
                IsCorrectSearch = bool.Parse(values[4]),
            };

            return result;
        }

        /// <summary>
        /// Converts a list of FactsWithUnknownTag objects to a csv file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> FromFactsWithUnknownTagToCsv(List<FactsWithUnknownTag> data)
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
