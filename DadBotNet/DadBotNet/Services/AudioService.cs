using Discord.Audio;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DadBotNet.Utils;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = DadBotNet.Utils.Debug;

namespace DadBotNet.Services
{
    public class AudioService
    {
        IVoiceChannel currentVoice;
        IAudioClient audioClient;
        public async Task JoinAudio(IVoiceChannel target)
        {
            currentVoice = target;
            audioClient = await currentVoice.ConnectAsync(false,false,false);
        }

        public async Task LeaveAudio()
        {
            if(currentVoice != null)
            {
                await currentVoice.DisconnectAsync();
                audioClient = null;
                currentVoice = null;
            }
        }

        public async Task SendAudioAsync(IGuild guild, byte[] data)
        {
            using (var stream = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                MemoryStream memStream = new MemoryStream(data);
                await memStream.CopyToAsync(stream);
                await stream.FlushAsync();
            }
        }

        public async Task SendAudioAsyncFFMPEG()
        {
            using (var ffmpeg = CreateProcess(Directory.GetCurrentDirectory() + "/test.mp3"))
            using (var stream = audioClient.CreatePCMStream(AudioApplication.Music))
            {
                try { await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }
        }

        private Process CreateProcess(byte[] data)
        {
            var argumentBuilder = new List<string>();

            var argument = "-i pipe:0 -ar 44100 -f wav -ac 2 pipe:1";
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

            process.Start();

            process.StandardInput.BaseStream.WriteAsync(data);

            process.StandardInput.Close();

            return process;
        }

        private Process CreateProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
    }
}
