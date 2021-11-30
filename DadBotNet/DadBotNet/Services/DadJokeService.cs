using DadBotNet.Models;
using DadBotNet.Utils;

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
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/Jokes"))
            {
                Debug.Log("Could not find /Jokes. Creating it instead", DebugLevel.Warning);
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Jokes");
            }
            _configService = configService;

            allowSaving = bool.Parse(configService.GetField("allowWriteToJokeFile"));

            random = new Random();

            var path = configService.GetField("jokeFile");

            var jokesFromFile = File.ReadAllLines(path);
            List<Joke> jokeList = new List<Joke>();
            for (int i = 0; i < jokesFromFile.Length; i++)
            {
                jokeList.Add(new Joke(jokesFromFile[i],i));
            }

            jokes = jokeList.AsReadOnly();
        }

        public bool AddJoke(Joke joke)
        {
            throw new NotImplementedException();
        }

        public Joke GetJoke()
        {
            int newJokeIndex = random.Next(jokes.Count);

            while (newJokeIndex == lastJokeIndex)
            {
                newJokeIndex = random.Next(jokes.Count);
            }

            return jokes[0];
        }

        public byte[] GetJokeByteData(Joke joke)
        {
            var path = $"{Directory.GetCurrentDirectory()}/Jokes/joke{joke.jokeIndex}";

            if (!File.Exists(path))
            {
                return null;
            }

            return File.ReadAllBytes(path);
        }
    }
}
