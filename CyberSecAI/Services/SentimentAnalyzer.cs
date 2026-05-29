using System.Collections.Generic;

namespace CyberSecAI.Services
{
    public enum Sentiment { Neutral, Happy, Sad, Angry, Stressed, Tired, Scared, Confused }

    public class SentimentAnalyzer
    {
        private static readonly Dictionary<Sentiment, List<string>> _keywords =
            new Dictionary<Sentiment, List<string>>
            {
                [Sentiment.Happy] = new List<string> { "happy", "great", "awesome", "good", "wonderful", "fantastic", "excited", "amazing", "love", "excellent", "glad", "fine", "okay", "ok" },
                [Sentiment.Sad] = new List<string> { "sad", "unhappy", "depressed", "miserable", "terrible", "bad", "awful", "upset", "down", "heartbroken", "not okay", "not great", "not good" },
                [Sentiment.Angry] = new List<string> { "angry", "mad", "furious", "annoyed", "frustrated", "rage", "hate", "livid", "irritated" },
                [Sentiment.Stressed] = new List<string> { "stressed", "stress", "overwhelmed", "anxious", "worried", "nervous", "panic", "pressure", "tense", "not okay" },
                [Sentiment.Tired] = new List<string> { "tired", "exhausted", "sleepy", "fatigue", "drained", "bored" },
                [Sentiment.Scared] = new List<string> { "scared", "afraid", "terrified", "hacked", "compromised", "attacked", "fear", "concerned" },
                [Sentiment.Confused] = new List<string> { "confused", "lost", "unsure", "not sure", "don't understand", "explain", "how does" }
            };

        private static readonly Dictionary<Sentiment, string> _responses =
            new Dictionary<Sentiment, string>
            {
                [Sentiment.Happy] = "That's great to hear! 😊 How can I help you today?",
                [Sentiment.Sad] = "I'm sorry to hear that. 💙 I'll do my best to assist you and make things easier. You're in the right place — let's take it step by step.",
                [Sentiment.Angry] = "I hear you — that frustration is valid. 💪 Let's channel that energy into strengthening your security posture.",
                [Sentiment.Stressed] = "I'm sorry to hear that. Take a breath — CyberSec AI has got you. 🙌 I'll break everything down simply, no stress involved.",
                [Sentiment.Tired] = "I'll keep it concise and clear so you get the knowledge without the brain drain. 😴",
                [Sentiment.Scared] = "No need to panic — knowledge is your best shield. 🛡️ CyberSec AI is here to put you back in control.",
                [Sentiment.Confused] = "No worries — CyberSec AI will break it all the way down so it makes sense. 🤔 Let's go step by step.",
                [Sentiment.Neutral] = ""
            };

        public Sentiment Analyze(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return Sentiment.Neutral;
            var lower = input.ToLower();
            foreach (var kvp in _keywords)
                foreach (var kw in kvp.Value)
                    if (lower.Contains(kw))
                        return kvp.Key;
            return Sentiment.Neutral;
        }

        public string GetResponse(Sentiment s) =>
            _responses.TryGetValue(s, out var r) ? r : string.Empty;

        public string SentimentToString(Sentiment s) => s.ToString().ToLower();
    }
}