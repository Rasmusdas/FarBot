using DadBotNet.Services;
using System;
using System.Collections.Generic;
using DadBotNet.Utils;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = DadBotNet.Utils.Debug;

namespace DadBotNet.Modules
{
    public class DadJokeModule : ModuleBase<SocketCommandContext>
    {
        DadJokeService _dadJokeService;
        AudioService _audioService;
        IConfigService _configService;

        bool isTalking;

        public DadJokeModule(DadJokeService dadJokeService, AudioService audioService, IConfigService configService)
        {
            _dadJokeService = dadJokeService;
            _audioService = audioService;
            _configService = configService;
        }

        [Command("far")]
        [Summary("Fortæller en far joke")]
        [Alias("dad")]
        public async Task TellJokeInTextChannel()
        {
            await Context.Channel.SendMessageAsync(_dadJokeService.GetJoke().ToString());
        }



        [Command("fartts", RunMode = RunMode.Async)]
        [Summary("Fortæller en far joke i en voice kanal")]
        [Alias("dadtts")]
        public async Task TellJokeInVoiceChannel()
        {
            await TellJokeInVoiceChannel();
        }

        [Command("fartts", RunMode = RunMode.Async)]
        [Summary("Fortæller en far joke i en voice kanal")]
        [Alias("dadtts")]
        public async Task TellJokeInVoiceChannel(string arguments = "")
        {
            if(int.TryParse(arguments, out int rate))
            {
                if(rate < -10)
                {
                    rate = -10;
                }

                if(rate > 10)
                {
                    rate = 10;
                }
                await TellJokeInVoice(rate);
            }
            else
            {
                await TellJokeInVoice();
            }
            
        }

        private async Task TellJokeInVoice(int talkRate = 0)
        {
            SocketGuildUser user = ((SocketGuildUser)Context.User);
            if (user.VoiceChannel == null)
            {
                await Context.Message.ReplyAsync("Jeg ved ikke hvordan man sender lyd gennem tekst. Beklager!");

                return;
            }

            Debug.Log("Getting Joke");
            var dadJoke = _dadJokeService.GetJoke();

            byte[] voiceBytes = _dadJokeService.GetJokeByteData(dadJoke);

            if (!dadJoke.AlreadyProcessed)
            {
                Debug.Log("Joke bytes were not found. Generating new ones",DebugLevel.Warning);
                Debug.Log("Processing Joke");
                var cleanedDadJoke = PrepareJokeForPowershell(dadJoke.ToString());

                Debug.Log("Creating Process");
                Process powershellProcess = CreateTTSProcess(cleanedDadJoke);

                Debug.Log("Starting Process");
                powershellProcess.Start();

                Debug.Log("Reading Data from Process");
                string byteResult = await powershellProcess.StandardOutput.ReadToEndAsync();

                Debug.Log("Parsing Data");
                voiceBytes = ConvertJokeResultToVoiceBytes(byteResult);

                File.WriteAllBytes($"{Directory.GetCurrentDirectory()}/Jokes/joke{dadJoke.jokeIndex}",voiceBytes);
            }

            Debug.Log("Joining Channel");
            var audioConnection = await _audioService.JoinAudio(user.VoiceChannel);

            Debug.Log("Speaking in Channel");
            await _audioService.SendAudioAsync(audioConnection, voiceBytes);

            Debug.Log("Leaving Channel");
            await _audioService.LeaveAudio(user.VoiceChannel);
        }

        private byte[] ConvertJokeResultToVoiceBytes(string byteResult)
        {
            string[] splitBytes = byteResult.Split('-');

            byte[] bytes = new byte[splitBytes.Length];

            for (int i = 0; i < splitBytes.Length; i++)
            {
                bytes[i] = byte.Parse(splitBytes[i]);
            }

            return bytes;
        }

        private Process CreateTTSProcess(string cleanedDadJoke)
        {
            string voiceType = _configService.GetField("voice");
            var command = "Add-Type -AssemblyName System.speech;$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;";
            command += $"$speak.SelectVoice(\\\"{voiceType}\\\");";
            command += " $format = New-Object System.Speech.AudioFormat.SpeechAudioFormatInfo(96000,[System.Speech.AudioFormat.AudioBitsPerSample]::Sixteen,[System.Speech.AudioFormat.AudioChannel]::Mono);";
            command += "$OutStream = [System.IO.MemoryStream]::new(1000);";
            command += "$speak.SetOutputToAudioStream($OutStream,$format);";
            command += $"$speak.Speak(\\\"{cleanedDadJoke}\\\");";
            command += "$speak.Dispose();";
            command += "$data = $OutStream.ToArray() -join '-';";
            command += "Write-Output $data;";

            Debug.Log(command);
            ProcessStartInfo powerShellProcessInfo = new();

            powerShellProcessInfo.FileName = @"powershell.exe";
            powerShellProcessInfo.Arguments = command;
            powerShellProcessInfo.RedirectStandardOutput = true;
            powerShellProcessInfo.UseShellExecute = false;
            powerShellProcessInfo.CreateNoWindow = false;

            Process process = new();
            process.StartInfo = powerShellProcessInfo;

            return process;

        }

        private string PrepareJokeForPowershell(string dadJoke)
        {
            dadJoke = dadJoke.Replace("\"", "\\\"");

            return dadJoke;
        }
    }
}
