using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DadBotNet.Models
{
    public class Joke
    {
        readonly string _jokeText;

        public Joke(string jokeText)
        {
            _jokeText = jokeText;
        }


        public override string ToString()
        {
            return _jokeText;
        }
    }
}
