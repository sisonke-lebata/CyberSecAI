using System;
using System.Collections.Generic;

namespace CyberSecAI.Models
{
    public class UserSession
    {
        public string UserName { get; set; } = "Agent";
        public string FavouriteTopic { get; set; }
        public string LastSentiment { get; set; }
        public DateTime SessionStart { get; } = DateTime.Now;
        public List<string> TopicsViewed { get; } = new List<string>();

        public string TimeGreeting
        {
            get
            {
                int h = DateTime.Now.Hour;
                return h >= 5 && h < 12 ? "Good morning"
                     : h >= 12 && h < 17 ? "Good afternoon"
                     : "Good evening";
            }
        }

        public void RecordTopic(string topic)
        {
            if (!string.IsNullOrWhiteSpace(topic) && !TopicsViewed.Contains(topic))
                TopicsViewed.Add(topic);
            if (string.IsNullOrEmpty(FavouriteTopic))
                FavouriteTopic = topic;
        }

        public string GetMemorySummary()
        {
            var parts = new List<string>();
            parts.Add($"Your name is {UserName}");
            if (!string.IsNullOrEmpty(FavouriteTopic))
                parts.Add($"First topic you explored: {FavouriteTopic}");
            if (TopicsViewed.Count > 0)
                parts.Add($"Topics covered: {string.Join(", ", TopicsViewed)}");
            if (!string.IsNullOrEmpty(LastSentiment))
                parts.Add($"Last mood detected: {LastSentiment}");
            parts.Add($"Session started: {SessionStart:HH:mm}");
            return string.Join("\n", parts);
        }
    }
}