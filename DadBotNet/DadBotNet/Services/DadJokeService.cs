using DadBotNet.Models;

namespace DadBotNet.Services
{
    public class DadJokeService : IDadJokeService
    {
        IConfigService _configService;
        private bool allowSaving;
        private IReadOnlyList<Joke> jokes;
        private int lastJokeIndex = -1;
        private Random random;

        public DadJokeService(IConfigService configService)
        {
            _configService = configService;

            allowSaving = bool.Parse(configService.GetField("allowWriteToJokeFile"));

            random = new Random();

            var path = configService.GetField("jokeFile");

            var jokesFromFile = File.ReadAllLines(path);
            List<Joke> jokeList = new List<Joke>();
            foreach(var joke in jokesFromFile)
            {
                jokeList.Add(new Joke(joke));
            }

            jokes = jokeList.AsReadOnly();
        }

        public bool AddJoke(string joke)
        {
            throw new NotImplementedException("Support for adding jokes not added :(");
        }

        public string GetJoke()
        {
            int newJokeIndex = random.Next(jokes.Count);

            while (newJokeIndex == lastJokeIndex)
            {
                newJokeIndex = random.Next(jokes.Count);
            }

            return jokes[newJokeIndex].ToString();
        }
    }
}
