using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using CyberSecAI.Services;

namespace CyberSecAI.Views
{
    public partial class MainWindow : Window
    {
        // ── Services ──────────────────────────────────────────────
        private readonly ChatService _chat = new ChatService();
        private readonly MemoryService _memory = new MemoryService();
        private readonly AudioService _audio = new AudioService();
        private readonly SentimentAnalyzer _sentiment = new SentimentAnalyzer();

        // ── State ─────────────────────────────────────────────────
        private bool _awaitingName = true;
        private bool _awaitingSentiment = false;

        // ── Constructor ───────────────────────────────────────────
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        // ── Startup ───────────────────────────────────────────────
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BuildTopicChips();
            StatusLabel.Text = $"Connected  •  {DateTime.Now:HH:mm}";

            // ASCII logo
            AppendBot(Models.AsciiArt.GetLogo(), isCode: true);

            await Task.Delay(400);

            // Greeting + name prompt
            AppendBot("Initialising neural link...\nConnection established. Welcome to CyberSec AI.\n\nBefore we begin — what should I call you, Agent?");
            await _audio.PlayVoiceGreetingAsync();

            UserInput.Focus();
        }

        // ── Send logic ────────────────────────────────────────────
        private async void BtnSend_Click(object sender, RoutedEventArgs e) => await ProcessInputAsync();
        private async void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                await ProcessInputAsync();
        }

        private async Task ProcessInputAsync()
        {
            var raw = UserInput.Text.Trim();
            UserInput.Clear();

            if (string.IsNullOrWhiteSpace(raw))
            {
                AppendBot(ChatService.EmptyInputResponses[new Random().Next(ChatService.EmptyInputResponses.Count)]);
                return;
            }

            AppendUser(raw);

            // ── Step 1: collect name ──────────────────────────────
            if (_awaitingName)
            {
                _memory.SetUser(raw);
                _awaitingName = false;
                _awaitingSentiment = true;

                var greeting = _memory.Session.TimeGreeting;
                var name = _memory.Session.UserName;

                AppendBot($"{greeting}, {name}! 👋\n\nI'm CyberSec AI — your advanced cybersecurity intelligence assistant.\n\nBefore we dive in, how are you feeling today?");
                UpdateMemoryLabel();
                await _audio.SpeakAsync($"{greeting} {name}. How are you feeling today?");
                return;
            }

            // ── Step 2: sentiment check ───────────────────────────
            if (_awaitingSentiment)
            {
                _awaitingSentiment = false;
                var mood = _sentiment.Analyze(raw);
                _memory.SetSentiment(_sentiment.SentimentToString(mood));

                var sentimentReply = _sentiment.GetResponse(mood);
                var intro = string.IsNullOrEmpty(sentimentReply)
                    ? "Understood. Let's get started."
                    : sentimentReply;

                AppendBot($"{intro}\n\nType a topic, a keyword, or 'help' to see the full topic menu.\nYou can also type a number 1–13 to jump straight to a module.");
                UpdateMemoryLabel();
                await _audio.SpeakAsync(intro);
                return;
            }

            // ── Step 3: numbered shortcut (1-13) ──────────────────
            if (int.TryParse(raw, out int num) && num >= 1 && num <= 13)
            {
                var topics = _chat.GetAllTopics();
                if (num - 1 < topics.Count)
                {
                    var topic = topics[num - 1];
                    _memory.RecordTopic(topic.Name);
                    UpdateMemoryLabel();
                    var reply = _chat.GetResponse(topic.Keywords[0]);
                    AppendBot(reply);
                    await _audio.SpeakAsync(reply);
                    return;
                }
            }

            // ── Step 4: help menu ─────────────────────────────────
            if (raw.Equals("help", StringComparison.OrdinalIgnoreCase)
             || raw.Equals("menu", StringComparison.OrdinalIgnoreCase))
            {
                AppendBot(BuildHelpMenu());
                return;
            }

            // ── Step 5: memory recall ──────────────────────────────
            if (raw.ToLower().Contains("remember") || raw.ToLower().Contains("what do you know"))
            {
                AppendBot("◈ SESSION LOG\n\n" + _memory.GetMemorySummary());
                return;
            }

            // ── Step 6: normal chat ───────────────────────────────
            var response = _chat.GetResponse(raw);

            // Record topic if matched
            foreach (var topic in _chat.GetAllTopics())
                foreach (var kw in topic.Keywords)
                    if (raw.ToLower().Contains(kw))
                    {
                        _memory.RecordTopic(topic.Name);
                        break;
                    }

            UpdateMemoryLabel();
            AppendBot(response);
            await _audio.SpeakAsync(response);
        }

        // ── Sidebar buttons ───────────────────────────────────────
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ChatPanel.Children.Clear();
            AppendBot("Chat cleared. System ready. Type a topic or 'help' to continue.");
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            AppendBot(BuildHelpMenu());
        }

        private void BtnMemory_Click(object sender, RoutedEventArgs e)
        {
            AppendBot("◈ SESSION LOG\n\n" + _memory.GetMemorySummary());
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Disconnect from CyberSec AI?",
                "Confirm Disconnect",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        // ── Topic chips ───────────────────────────────────────────
        private void BuildTopicChips()
        {
            var topics = _chat.GetAllTopics();
            for (int i = 0; i < topics.Count; i++)
            {
                var topic = topics[i];
                int index = i + 1;

                var btn = new Button
                {
                    Content = $"{index}. {topic.Emoji} {topic.Name}",
                    Style = (Style)FindResource("TopicChip"),
                    Tag = topic.Keywords[0]
                };
                btn.Click += TopicChip_Click;
                TopicPanel.Children.Add(btn);
            }
        }

        private async void TopicChip_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string kw)
            {
                // Find the topic to record its name
                foreach (var topic in _chat.GetAllTopics())
                    if (Array.IndexOf(topic.Keywords, kw) >= 0)
                    {
                        _memory.RecordTopic(topic.Name);
                        break;
                    }

                var reply = _chat.GetResponse(kw);
                AppendUser($"[{btn.Content}]");
                UpdateMemoryLabel();
                AppendBot(reply);
                await _audio.SpeakAsync(reply);
            }
        }

        // ── Chat bubble helpers ───────────────────────────────────
        private void AppendUser(string text)
        {
            var name = _memory.Session.UserName;

            var wrapper = new StackPanel { Margin = new Thickness(0, 6, 0, 6) };

            var label = new TextBlock
            {
                Text = $"▶ {name}",
                Foreground = new SolidColorBrush(Color.FromRgb(0, 180, 220)),
                FontSize = 10,
                FontFamily = new FontFamily("Consolas"),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 8, 3),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(4, 20, 42)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 184, 255)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8, 2, 8, 8),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(80, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(0, 184, 255),
                    BlurRadius = 8,
                    ShadowDepth = 0,
                    Opacity = 0.25
                },
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = new SolidColorBrush(Color.FromRgb(208, 239, 255)),
                    FontSize = 13,
                    FontFamily = new FontFamily("Consolas"),
                    TextWrapping = TextWrapping.Wrap
                }
            };

            wrapper.Children.Add(label);
            wrapper.Children.Add(bubble);
            ChatPanel.Children.Add(wrapper);
            ScrollToBottom();
        }

        private void AppendBot(string text, bool isCode = false)
        {
            var wrapper = new StackPanel { Margin = new Thickness(0, 6, 0, 6) };

            var label = new TextBlock
            {
                Text = "◈ CYBERSEC AI",
                Foreground = new SolidColorBrush(Color.FromRgb(0, 229, 255)),
                FontSize = 10,
                FontFamily = new FontFamily("Consolas"),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 3),
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(0, 229, 255),
                    BlurRadius = 6,
                    ShadowDepth = 0,
                    Opacity = 0.6
                }
            };

            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(6, 15, 28)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 229, 255)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(2, 8, 8, 8),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(0, 0, 80, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Effect = new DropShadowEffect
                {
                    Color = Color.FromRgb(0, 229, 255),
                    BlurRadius = 10,
                    ShadowDepth = 0,
                    Opacity = 0.15
                },
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = new SolidColorBrush(
                                       isCode
                                       ? Color.FromRgb(0, 229, 255)
                                       : Color.FromRgb(176, 224, 240)),
                    FontSize = isCode ? 11 : 13,
                    FontFamily = new FontFamily("Consolas"),
                    TextWrapping = TextWrapping.Wrap,
                    LineHeight = 20
                }
            };

            wrapper.Children.Add(label);
            wrapper.Children.Add(bubble);
            ChatPanel.Children.Add(wrapper);
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            ChatScroll.UpdateLayout();
            ChatScroll.ScrollToBottom();
        }

        // ── Helpers ───────────────────────────────────────────────
        private void UpdateMemoryLabel()
        {
            MemoryLabel.Text = _memory.GetMemorySummary();
        }

        private string BuildHelpMenu()
        {
            var topics = _chat.GetAllTopics();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("◈ TOPIC MODULES — Type a number or keyword:\n");
            for (int i = 0; i < topics.Count; i++)
            {
                var t = topics[i];
                sb.AppendLine($"  {i + 1:D2}.  {t.Emoji}  {t.Name}");
            }
            sb.AppendLine("\n💡 Example: Type '1', 'phishing', or 'help' at any time.");
            return sb.ToString();
        }
    }
}