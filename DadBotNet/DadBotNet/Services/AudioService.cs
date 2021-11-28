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
            using (var ffmpeg = CreateProcess())
            {
                using (var stream = audioClient.CreatePCMStream(AudioApplication.Music))
                {
                    try { await stream.WriteAsync(data); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        private Process CreateProcess()
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = "-loglevel panic -i pipe:.mp3  -ac 1 -ar 44100 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            });
        }
    }
}
