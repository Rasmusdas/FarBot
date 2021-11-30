using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DadBotNet.Services
{
    public class AudioService
    {
        IVoiceChannel currentVoice;
        IAudioClient audioClient;
        public async Task JoinAudio(IVoiceChannel target)
        {
            currentVoice = target;
            Console.WriteLine("Starting Connection");
            audioClient = await currentVoice.ConnectAsync(false,false,false);
            Console.WriteLine("Connected");
        }

        public async Task LeaveAudio()
        {
            if(currentVoice != null)
            {
                await currentVoice.DisconnectAsync();
                audioClient = null;
            }
        }

        public async Task SendAudioAsync(IGuild guild, byte[] data)
        {
            using (var ffmpeg = CreateProcess(data))
            {
                using (var stream = audioClient.CreatePCMStream(AudioApplication.Mixed))
                {
                    int i = 0;
                    List<byte> bytes = new List<byte>();
                    await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);

                    await stream.FlushAsync();
                }
            }

            
            Console.WriteLine("Done speaking in voice");
        }

        private Process CreateProcess(byte[] data)
        {
            var argumentBuilder = new List<string>();

            var argument = "-hide_banner -loglevel panic -f wav -i pipe:0 -ar 48000 -ac 2 -f s16le pipe:1";
            argumentBuilder.Add($"-f mp3");
            argumentBuilder.Add("-i pipe:0"); //this sets the input to stdin

            // the target audio specs are as follows
            argumentBuilder.Add("-hide_banner");
            argumentBuilder.Add("-loglevel panic");
            argumentBuilder.Add("-ac 2");
            argumentBuilder.Add($"-ar 48000");
            argumentBuilder.Add($"-f wav");
            argumentBuilder.Add("pipe:1"); // this sets the output to stdout


            Process process = new Process();
            
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = argument,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true
            };

            MemoryStream dataStream = new MemoryStream();
            process.Start();

            process.StandardInput.BaseStream.WriteAsync(data);

            process.StandardInput.Close();

            return process;
        }
    }
}
