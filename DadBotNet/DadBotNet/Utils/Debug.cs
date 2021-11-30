using DadBotNet.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DadBotNet.Utils
{
    public static class Debug
    {
        private static readonly ReadOnlyDictionary<DebugLevel, ConsoleColor> _levelToColor = new ReadOnlyDictionary<DebugLevel, ConsoleColor>(new Dictionary<DebugLevel, ConsoleColor>() {
            { DebugLevel.Info, ConsoleColor.Green },
            { DebugLevel.Warning, ConsoleColor.Yellow },
            { DebugLevel.Error, ConsoleColor.Red }
        });
        static bool _debugEnabled = false;
        static Debug()
        {
            ConfigService config = new ConfigService();
            _debugEnabled = (config.GetField("enableDebugInfo").ToLower() == "true");
        }

        public static void Log(string message, DebugLevel level = DebugLevel.Info)
        {
            if(_debugEnabled)
            {
                PrintInfoMessage(message, level);
            }
        }

        public static void Log(object message, DebugLevel level = DebugLevel.Info)
        {
            Log(message.ToString(), level);
        }

        private static void PrintInfoMessage(string message, DebugLevel level)
        {
            Console.Write($"[{GetCurrentTime()}] ");

            Console.ForegroundColor = _levelToColor[level];

            Console.Write($"{level.ToString().ToUpper()}");

            Console.ResetColor();

            Console.Write(" | ");

            Console.WriteLine(message);
        }

        private static TimeSpan GetCurrentTime()
        {
            TimeSpan time = DateTime.Now.TimeOfDay;

            time = new TimeSpan(time.Hours, time.Minutes, time.Seconds);

            return time;
        }
    }
}
