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
        public Task<IAudioClient> JoinAudio(IVoiceChannel target)
        {
            return target.ConnectAsync(false,false,false);
        }

        public Task LeaveAudio(IVoiceChannel currentVoice)
        {
            if(currentVoice != null)
            {
                return currentVoice.DisconnectAsync();
            }
            else
            {
                Debug.Log("Tried to leave channel I wasn't in", DebugLevel.Error);
                return null;
            }
        }

        public async Task SendAudioAsync(IAudioClient audioClient, byte[] data)
        {
            using (var stream = audioClient.CreatePCMStream(AudioApplication.Mixed))
            {
                MemoryStream memStream = new MemoryStream(data);
                await memStream.CopyToAsync(stream);
                await stream.FlushAsync();
            }
        }

        public async Task SendAudioAsyncFFMPEG(IAudioClient audioClient)
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

            var argument = "-i pipe:0 -ar 48000 -f wav -ac 2 pipe:1";
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

        private Process CreateProcess(string path) => Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg.exe",
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });
    }
}
