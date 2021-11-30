using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DadBotNet.Services
{
    public interface IDadJokeService
    {
        string GetJoke();

        bool AddJoke(string joke);
    }
}
