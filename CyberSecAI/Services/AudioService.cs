using System;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CyberSecAI.Services
{
    public class AudioService : IDisposable
    {
        private SpeechSynthesizer _synth;
        private bool _enabled = true;

        public AudioService()
        {
            try
            {
                _synth = new SpeechSynthesizer();
                try { _synth.SelectVoice("Microsoft David Desktop"); }
                catch { /* use default */ }
                _synth.Rate = 2;
                _synth.Volume = 90;
            }
            catch { _enabled = false; }
        }

        public async Task PlayVoiceGreetingAsync()
        {
            await SpeakAsync(
                "Hello, welcome to CyberSec A.I. I am your advanced cybersecurity intelligence assistant. " +
                "I am here to keep you safe in the digital world. " +
                "How are you feeling today?");
        }

        public async Task SpeakAsync(string text)
        {
            if (!_enabled || _synth == null || string.IsNullOrWhiteSpace(text)) return;
            try
            {
                var clean = Regex.Replace(text, @"[^\u0000-\u007F\s]", "");
                var idx = clean.IndexOfAny(new[] { '.', '!', '?' });
                var spoken = idx > 0 ? clean.Substring(0, idx + 1).Trim()
                           : clean.Length > 120 ? clean.Substring(0, 120)
                           : clean;
                await Task.Run(() => _synth.SpeakAsync(spoken));
            }
            catch { }
        }

        public void Dispose() => _synth?.Dispose();
    }
}