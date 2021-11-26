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
        DadJokeService _service;

        public DadJokeModule(DadJokeService service)
        {
            _service = service;
        }

        [Command("far")]
        [Summary("Fortæller en far joke")]
        public async Task TellJokeInTextChannel()
        {
            await Context.Channel.SendMessageAsync(_service.GetJoke());
        }

        [Command("Dad")]
        [Summary("Tells a dad Joke")]
        public async Task AliasDadTellJokeInTextChannel() => await TellJokeInTextChannel();


        [Command("fartts")]
        [Summary("Fortæller en far joke i en voice kanal")]
        public async Task TellJokeInVoiceChannel()
        {
            if(((SocketGuildUser)Context.User).VoiceChannel == null)
            {
                await Context.Message.ReplyAsync("Du er ikke i en voice kanal :/");

                return;
            }

            var dadJoke = _service.GetJoke();

            var cleanedDadJoke = PrepareJokeForPowershell(dadJoke);

            Process powershellProcess = CreateTTSProcess(cleanedDadJoke);
            powershellProcess.Start();
            string byteResult = await powershellProcess.StandardOutput.ReadToEndAsync();

            byte[] voiceBytes = ConvertJokeResultToVoiceBytes(byteResult);





            /*
            let args = []
            let pipedData = ''

            let psCommand = `Add-Type -AssemblyName System.speech;$speak = New-Object System.Speech.Synthesis.SpeechSynthesizer;`

            psCommand += `$OutStream = [System.IO.MemoryStream]::new(100);`
            psCommand += `$speak.SetOutputToWaveStream($OutStream);`
            psCommand += `$speak.SelectVoice(\\"` + config.voice + `\\");`;
            text = text.replace("\"" , "")
            psCommand += `$speak.Speak(\\"` + text + `\\");`
            psCommand += "$data = $OutStream.ToArray() -join '-';"
            psCommand += `Write-Output $data;`
    
            args.push(psCommand)

            return [args,pipedData];
             */
        }

        private byte[] ConvertJokeResultToVoiceBytes(string byteResult)
        {
            Console.WriteLine(byteResult);

            return null;
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
