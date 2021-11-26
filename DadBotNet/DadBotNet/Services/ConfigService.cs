using DadBotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DadBotNet.Services
{
    internal class ConfigService
    {
        Config config;

        internal ConfigService()
        {
            config = new Config(File.ReadAllText("config.json"));
        }


        internal string GetField(string field)
        {
            return config.GetField(field);
        }
    }
}
