using DadBotNet.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DadBotNet.Modules
{
    public class DadJokeModule : ModuleBase<SocketCommandContext>
    {
        DadJokeService _dadJokeService;
        AudioService _audioService;

        public DadJokeModule(DadJokeService dadJokeService, AudioService audioService)
        {
            _dadJokeService = dadJokeService;
            _audioService = audioService;
        }

        [Command("far")]
        [Summary("Fortæller en far joke")]
        public async Task TellJokeInTextChannel()
        {
            await Context.Channel.SendMessageAsync(_dadJokeService.GetJoke());
        }

        [Command("Dad")]
        [Summary("Tells a dad Joke")]
        public async Task AliasDadTellJokeInTextChannel() => await TellJokeInTextChannel();


        [Command("fartts")]
        [Summary("Fortæller en far joke i en voice kanal")]
        public async Task TellJokeInVoiceChannel()
        {
            await TellJokeInVoice();
        }

        private async Task TellJokeInVoice()
        {
            SocketGuildUser user = ((SocketGuildUser)Context.User);
            if (user.VoiceChannel == null)
            {
                await Context.Message.ReplyAsync("Du er ikke i en voice kanal :/");

                return;
            }

            Console.WriteLine("Getting Joke");
            var dadJoke = _dadJokeService.GetJoke();

            Console.WriteLine("Processing Joke");
            var cleanedDadJoke = PrepareJokeForPowershell(dadJoke);

            Console.WriteLine("Creating Process");
            Process powershellProcess = CreateTTSProcess(cleanedDadJoke);

            Console.WriteLine("Starting Process");
            powershellProcess.Start();

            Console.WriteLine("Reading Data from Process");
            string byteResult = await powershellProcess.StandardOutput.ReadToEndAsync();

            Console.WriteLine("Parsing Data");
            byte[] voiceBytes = await Task.Run(() => ConvertJokeResultToVoiceBytes(byteResult));

            Console.WriteLine("Joining Channel");
            await _audioService.JoinAudio(user.VoiceChannel);

            Console.WriteLine("Speaking in Channel");
            await _audioService.SendAudioAsync(user.Guild, voiceBytes);

            Console.WriteLine("Leaving Channel");
            //await _audioService.LeaveAudio();
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

            var command = "Add-Type -AssemblyName System.speech;$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;";
            command += "$OutStream = [System.IO.MemoryStream]::new(100);";
            command += "$speak.SetOutputToWaveStream($OutStream);";
            command += $"$speak.Speak(\\\"{cleanedDadJoke}\\\");";
            command += "$data = $OutStream.ToArray() -join '-';";
            command += "Write-Output $data;";

            ProcessStartInfo powerShellProcessInfo = new();

            powerShellProcessInfo.FileName = @"powershell.exe";
            powerShellProcessInfo.Arguments = command;
            powerShellProcessInfo.RedirectStandardOutput = true;
            powerShellProcessInfo.RedirectStandardError = true;
            powerShellProcessInfo.UseShellExecute = false;
            powerShellProcessInfo.CreateNoWindow = true;

            Process process = new();
            process.StartInfo = powerShellProcessInfo;

            return process;

        }

        private string PrepareJokeForPowershell(string dadJoke)
        {
            return dadJoke;
        }
    }
}
