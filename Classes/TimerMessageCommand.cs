using DSharpPlus;
using DSharpPlus.Entities;
using lol_facts.Entities;
using lol_facts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace lol_facts.Classes
{
    public static class TimerMessageCommand
    {
        private static List<TimerMessage> _timers = new();
        private static DiscordClient _discordClient;

        /// <summary>
        /// Set a new timer for a message to post
        /// </summary>
        /// <param name="discordChannel"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void SetTimerMessage(DiscordChannel discordChannel, int day, int hour, uint? id = null)
        {
            int dud = DaysUntilDate(day, hour);

            TimeSpan now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));     // The current time in 24 hour format
            TimeSpan target = new(hour + (24 * dud), 0, 0);
            TimeSpan timeLeftUntilHour = target - now;

#if DEBUG
            timeLeftUntilHour = new(0, 0, 10);
#endif

            // Timer creation
            if (id == null)
            {
                id = GetNewTimerId();

                var timer = new TimerMessage(id.Value)
                {
                    Timer = new()
                    {
                        Interval = timeLeftUntilHour.TotalMilliseconds
                    },
                    Day = day,
                    Hour = hour,
                    DiscordChannelId = discordChannel.Id,
                };

                _timers.Add(timer);
                FileReader.AddTimer(timer);
            }
            // Timer edition
            else
            {
                var timer = _timers.First(t => t.Id == id);

                if (timer.Timer != null)
                {
                    timer.Timer.Dispose();
                    timer.Timer = null;
                }

                timer.Timer = new()
                {
                    Interval = timeLeftUntilHour.TotalMilliseconds
                };
            }

            _timers.First(t => t.Id == id).Timer.Elapsed += (sender, e) => SendRandomFact(sender, e, discordChannel, day, hour, id.Value);
            _timers.First(t => t.Id == id).Timer.Start();
        }

        /// <summary>
        /// Restarts all the timers previously recorded at the beginning of the application
        /// </summary>
        /// <param name="discord"></param>
        public static void RestartTimersOnAppStarting(DiscordClient discord)
        {
            _discordClient = discord;

            foreach (var timer in _timers)
            {
                if (timer.Timer != null)
                {
                    timer.Timer.Dispose();
                }
            }
            _timers = FileReader.ReadAllTimers();

            foreach (var timer in _timers)
            {
                var discordChannelId = discord.Guilds.Values.SelectMany(g => g.Channels).FirstOrDefault(c => c.Key == timer.DiscordChannelId).Value;
                if (discordChannelId == null)
                {
                    continue;
                }

                SetTimerMessage(discordChannelId, timer.Day, timer.Hour, timer.Id);
            }
        }

        /// <summary>
        /// Sets a new Id for the timer
        /// </summary>
        /// <returns></returns>
        private static uint GetNewTimerId()
        {
            uint id = 0;

            if (_timers.Any())
                id = _timers.Max(t => t.Id) + 1;

            return id;
        }

        /// <summary>
        /// Cancels a timer from its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool TryCancelTimerFromId(uint id)
        {
            FileReader.RemoveLineFromId(id);
            var timer = _timers.Find(t => t.Id == id);

            if (timer == null)
            {
                return false;
            }

            timer.Timer.Dispose();
            timer.Timer.Close();
            _timers.Remove(timer);

            return true;
        }

        /// <summary>
        /// Sends a specific message to a channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="discordChannel"></param>
        /// <param name="message"></param>
        private async static void SendRandomFact(object sender, ElapsedEventArgs e, DiscordChannel discordChannel, int day, int hour, uint id)
        {
            _timers.Find(t => t.Id == id).Timer.Stop();

            int index = new Random().Next(FactsManager.CountFactsWithTag(string.Empty));
            string message = FactsManager.GetFactMessage(string.Empty, ref index);

            // Sends the message
            var discordMessage = await discordChannel.SendMessageAsync(message);

            // Refresh the timer interval
            SetTimerMessage(discordChannel, day, hour, id);
        }

        /// <summary>
        /// Calculates the days until the <paramref name="day"/>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        private static int DaysUntilDate(int day, long hour)
        {
            int dow = (int)DateTime.Now.DayOfWeek;

            if (dow > day ||
               (dow == day && hour <= DateTime.Now.Hour))
            {
                day += 7;
            }

            return day - dow;
        }
    }
}
