using System;
using System.Collections.Generic;
using CyberSecAI.Models;

namespace CyberSecAI.Services
{
    public class ChatService
    {
        private static readonly Random _rng = new Random();
        private readonly List<ChatTopic> _topics;

        public static readonly List<string> UnknownResponses = new List<string>
        {
            "That one isn't in the database. Try a topic name or type 'help' to see the menu. 🤔",
            "I didn't catch that. Try again or type 'help' to see what CyberSec AI can assist with.",
            "That's not matching any known threat vectors. Try keywords like 'phishing', 'password', or 'malware'.",
            "CyberSec AI needs more input. Type a topic or 'help' to see the full list."
        };

        public static readonly List<string> EmptyInputResponses = new List<string>
        {
            "You didn't type anything! Send a message or topic to continue. 😄",
            "Don't leave CyberSec AI waiting — type something and let's get into it!",
            "Silence is a vulnerability! Drop a topic and let's go. 🛡️"
        };

        public static readonly List<string> Acknowledgements = new List<string>
        {
            "Acknowledged — here's the intel you need:",
            "Request received. CyberSec AI has the knowledge on that:",
            "You came to the right place for this — check it:",
            "Initialising response. Here's what you need to know:"
        };

        public ChatService()
        {
            _topics = BuildTopics();
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return GetRandom(EmptyInputResponses);

            var lower = input.ToLower().Trim();

            if (lower.Contains("about") || lower.Contains("who are you") || lower.Contains("what are you"))
                return AboutCyberSec();

            foreach (var topic in _topics)
                foreach (var kw in topic.Keywords)
                    if (lower.Contains(kw))
                        return $"{GetRandom(Acknowledgements)}\n\n{GetRandom(new List<string>(topic.Responses))}";

            return GetRandom(UnknownResponses);
        }

        public List<ChatTopic> GetAllTopics() => _topics;

        private static string GetRandom(List<string> list)
        {
            if (list == null || list.Count == 0) return string.Empty;
            return list[_rng.Next(list.Count)];
        }

        private string AboutCyberSec() =>
            "🛡️ ABOUT CYBERSEC AI\n\n" +
            "I'm CyberSec AI — your advanced cybersecurity intelligence assistant, built in C# and WPF.\n\n" +
            "My mission: keep you safe and informed in the digital world.\n\n" +
            "Topics I cover:\n" +
            "• Phishing & Scams          • Password Security\n" +
            "• Two-Factor Authentication  • Ransomware\n" +
            "• Social Engineering        • Safe Browsing\n" +
            "• Malware Protection        • Cyber Hygiene\n" +
            "• Privacy & Identity Theft  • Wi-Fi Security\n" +
            "• Ethical Hacking           • Network Security\n" +
            "• AI Assistance\n\n" +
            "Type 'help' or a number 1–13 to explore any topic.";

        private List<ChatTopic> BuildTopics() => new List<ChatTopic>
        {
            new ChatTopic
            {
                Name = "Phishing", Emoji = "🎣",
                Keywords = new[] { "phishing", "phish", "fake email", "email scam", "spoofed" },
                Responses = new[]
                {
                    "🎣 PHISHING\n\nPhishing is when cybercriminals impersonate trusted entities — your bank, your email provider, even your employer — to trick you into surrendering passwords or personal details.\n\nThese attacks arrive via fake emails and deceptive links designed to look legitimate.\n\n✅ Golden Rules:\n• Never click unexpected links — verify the source first\n• Check the actual sender address, not just the display name\n• Never submit credentials or OTPs in response to an email\n• Look for HTTPS and inspect the full URL carefully\n\n🔴 Phishing Variants:\n• Email Phishing — mass-distributed fake emails\n• Spear Phishing — targeted attacks using your real details\n• Smishing — phishing via SMS\n• Vishing — phishing via phone call",
                    "🎣 PHISHING — Intel Report\n\nOver 3.4 billion phishing emails are sent every single day. It is the #1 attack vector globally.\n\n🛡️ Critical Rule: No legitimate organisation will EVER request your password, OTP, or PIN via email or phone.\n\nWhen something feels off — trust your instincts. Navigate directly to the official website and call their verified number."
                }
            },
            new ChatTopic
            {
                Name = "Password Security", Emoji = "🔑",
                Keywords = new[] { "password", "passwords", "credentials", "passphrase" },
                Responses = new[]
                {
                    "🔑 PASSWORD SECURITY\n\nYour password is the primary barrier between attackers and your digital life. A weak password is an open door.\n\n✅ Build It Strong:\n• Mix uppercase, lowercase, numbers, and symbols\n• Minimum 12–16 characters\n• Never reuse passwords — one breach becomes many\n• Use a password manager to generate and store unique passwords\n\n❌ Never use: your name, birthday, 'password123', or your pet's name\n\n💡 Check haveibeenpwned.com to see if your credentials have been compromised.",
                    "🔑 PASSWORD SECURITY — Level Up\n\n81% of data breaches involve weak or stolen passwords.\n\n🧪 Password Manager Advantage:\n• Generates unique strong passwords for every site\n• You only remember ONE master password\n• Alerts you when credentials appear in breaches\n\nFree options: Bitwarden, KeePass\n⚠️ Never share your password with anyone — including IT support."
                }
            },
            new ChatTopic
            {
                Name = "Two-Factor Authentication", Emoji = "🔐",
                Keywords = new[] { "two-factor", "2fa", "mfa", "multi-factor", "authentication", "otp" },
                Responses = new[]
                {
                    "🔐 TWO-FACTOR AUTHENTICATION (2FA)\n\n2FA adds a second verification layer beyond your password. Even if attackers steal your credentials, they cannot access your account without the second factor.\n\n✅ Enable 2FA everywhere possible:\n1. Hardware Key (YubiKey) — maximum security\n2. Authenticator App (Google Authenticator, Authy) — highly recommended\n3. Email code — moderate security\n4. SMS code — better than nothing, but vulnerable to SIM-swapping\n\n💡 Prefer authenticator apps over SMS — app-generated codes are far harder to intercept.\n\n⚠️ Never share your OTP with anyone, ever."
                }
            },
            new ChatTopic
            {
                Name = "Ransomware", Emoji = "🔒",
                Keywords = new[] { "ransomware", "ransom", "encrypted files", "locked files" },
                Responses = new[]
                {
                    "🔒 RANSOMWARE\n\nRansomware is digital extortion. Malicious software infiltrates your device, encrypts your files, and demands payment — typically in cryptocurrency — for the decryption key.\n\n✅ Stay Protected:\n• Back up data regularly using the 3-2-1 rule (3 copies, 2 media types, 1 offsite)\n• Avoid suspicious links and unauthorised downloads\n• Keep antivirus and OS fully updated\n\n⚠️ If Attacked — DO NOT PAY. Payment does not guarantee file recovery.\nDisconnect from the network immediately and contact a cybersecurity professional."
                }
            },
            new ChatTopic
            {
                Name = "Social Engineering", Emoji = "🎭",
                Keywords = new[] { "social engineering", "social", "manipulation", "pretexting", "baiting" },
                Responses = new[]
                {
                    "🎭 SOCIAL ENGINEERING\n\nSocial engineering is psychological manipulation. Attackers exploit fear, urgency, or misplaced trust to pressure you into divulging sensitive information or performing actions that compromise security.\n\n🔴 Common Techniques:\n• Pretexting — 'Hi, I'm from IT. I need your password'\n• Baiting — infected USB drives left in public hoping someone plugs them in\n• Tailgating — following someone through a secured entry point\n• Urgency — 'Act now or your account will be suspended!'\n\n✅ Always pause and verify before acting on any unexpected request — even if it appears to come from someone you know. One verification call can prevent a serious breach."
                }
            },
            new ChatTopic
            {
                Name = "Safe Browsing", Emoji = "🌐",
                Keywords = new[] { "safe browsing", "browsing", "browser", "website", "https", "vpn", "web", "internet" },
                Responses = new[]
                {
                    "🌐 SAFE BROWSING\n\nSafe browsing requires vigilance at every step online. Always verify HTTPS in the address bar — the padlock confirms your connection is encrypted.\n\n✅ Core Practices:\n• Keep browser and extensions updated\n• Avoid downloading files from untrusted sources\n• Use a VPN on public Wi-Fi to prevent traffic interception\n• Install an ad blocker (uBlock Origin is free and excellent)\n\n🛡️ Privacy Stack:\n• Browser: Firefox or Brave\n• Search: DuckDuckGo\n• VPN: ProtonVPN (free tier available)"
                }
            },
            new ChatTopic
            {
                Name = "Malware Protection", Emoji = "🦠",
                Keywords = new[] { "malware", "virus", "trojan", "spyware", "adware", "worm", "keylogger", "antivirus" },
                Responses = new[]
                {
                    "🦠 MALWARE PROTECTION\n\nMalware is a broad category of malicious software: viruses that corrupt files, spyware that monitors activity, Trojans disguised as legitimate software, and more.\n\n✅ Defence Protocol:\n• Run a reputable antivirus at all times\n• Only download from verified, trusted sources\n• Apply OS security updates promptly\n• Never connect unknown USB drives\n\n🔴 Incident Response:\n• Run a full system scan immediately\n• Disconnect from the network\n• Use Malwarebytes for deep malware removal"
                }
            },
            new ChatTopic
            {
                Name = "Cyber Hygiene", Emoji = "🧹",
                Keywords = new[] { "cyber hygiene", "hygiene", "digital habits", "security habits" },
                Responses = new[]
                {
                    "🧹 CYBER HYGIENE\n\nCyber hygiene is your daily security routine — consistent habits that keep your digital environment clean and protected.\n\n✅ Daily:\n• Lock your screen when stepping away\n• Think before clicking any link\n• Log out of accounts on shared devices\n\n📅 Weekly/Monthly:\n• Check for OS and application updates\n• Back up important files\n• Review bank statements for anomalies\n\n📅 Annually:\n• Rotate all passwords\n• Check haveibeenpwned.com for breaches\n• Audit social media privacy settings\n\nSmall, consistent habits compound into serious long-term protection."
                }
            },
            new ChatTopic
            {
                Name = "Privacy Protection", Emoji = "🔏",
                Keywords = new[] { "privacy", "private", "personal data", "tracking", "data collection" },
                Responses = new[]
                {
                    "🔏 PRIVACY PROTECTION\n\nYour personal data is a high-value asset — both for legitimate businesses and malicious actors.\n\n✅ Lock It Down:\n• Review privacy settings on all social media platforms\n• Audit app permissions — Location, Camera, Microphone\n• Use a VPN on public Wi-Fi\n• Use Signal for encrypted messaging\n• Use a private email alias for sign-ups\n\n💡 Golden Rule: If a service is free, your data is the product. Act accordingly."
                }
            },
            new ChatTopic
            {
                Name = "Scam Awareness", Emoji = "⚠️",
                Keywords = new[] { "scam", "scams", "fraud", "fake", "lottery", "prize", "romance scam" },
                Responses = new[]
                {
                    "⚠️ SCAM AWARENESS\n\nScammers deploy increasingly sophisticated tactics — leveraging urgency, trust, and emotion to extract money or data.\n\n🔴 Common Attack Vectors:\n• Lottery Scams — 'You've won! Pay a fee to claim'\n• Romance Scams — fabricated relationships targeting finances\n• Tech Support Scams — 'Your device is infected, call us now!'\n• Investment/Crypto Scams — guaranteed returns that never materialise\n• Impersonation — criminals posing as your bank or government agencies\n\n✅ Red Flags:\n• Requests for gift card or crypto payments\n• Extreme urgency and fear tactics\n• Anyone requesting your OTP, PIN, or banking credentials\n\n💡 If Compromised: Contact your bank immediately and report to SAPS Cybercrime."
                }
            },
            new ChatTopic
            {
                Name = "Wi-Fi Security", Emoji = "📶",
                Keywords = new[] { "wifi", "wi-fi", "wireless", "public wifi", "hotspot", "router" },
                Responses = new[]
                {
                    "📶 WI-FI SECURITY\n\n🏠 Home Router Hardening:\n• Change the default admin username AND password\n• Use WPA3 or WPA2 encryption — never WEP\n• Keep router firmware updated\n• Create a separate guest network for visitors and IoT devices\n\n☕ Public Wi-Fi Protocol:\n• Never access banking or sensitive accounts on public networks\n• Always tunnel through a VPN on public connections\n• Disable auto-connect to open networks on all devices"
                }
            },
            new ChatTopic
            {
                Name = "Identity Theft", Emoji = "🪪",
                Keywords = new[] { "identity theft", "identity", "stolen identity", "id theft" },
                Responses = new[]
                {
                    "🪪 IDENTITY THEFT PREVENTION\n\nIdentity theft occurs when an attacker steals your personal information to commit fraud — opening accounts, taking out loans, or making purchases in your name.\n\n✅ Defence Measures:\n• Shred sensitive documents before disposal\n• Monitor bank statements regularly for anomalies\n• Review your credit report for unauthorised activity\n• Only carry your ID document when strictly necessary\n• Place a fraud alert immediately if compromise is suspected"
                }
            },
            new ChatTopic
            {
                Name = "Ethical Hacking", Emoji = "💻",
                Keywords = new[] { "ethical hacking", "ethical hacker", "penetration testing", "pen test", "pentest", "white hat", "bug bounty", "hacking" },
                Responses = new[]
                {
                    "💻 ETHICAL HACKING\n\nEthical hacking — also called penetration testing — is the authorised practice of probing systems for vulnerabilities before malicious actors can exploit them.\n\n🔍 Core Methodology:\n• Reconnaissance — gather information about the target\n• Scanning — identify open ports, services, and weaknesses\n• Exploitation — attempt to leverage identified vulnerabilities\n• Reporting — document findings and recommend mitigations\n\n✅ Key Disciplines:\n• Web Application Testing (OWASP Top 10)\n• Network Penetration Testing\n• Social Engineering Simulations\n• Mobile Security Testing\n\n🎓 Certifications to Pursue:\nCEH · OSCP · CompTIA PenTest+ · eJPT\n\n💡 Always obtain written authorisation before testing any system. Hacking without permission is a criminal offence.",
                    "💻 ETHICAL HACKING — Career Path\n\nThe cybersecurity industry faces a global shortage of over 3.5 million professionals — ethical hackers are in extremely high demand.\n\n🛠️ Tools of the Trade:\n• Kali Linux — penetration testing OS\n• Metasploit — exploitation framework\n• Nmap — network scanner\n• Burp Suite — web application testing\n• Wireshark — network traffic analyser\n\n🌐 Practice Platforms:\n• HackTheBox · TryHackMe · PentesterLab\n\n⚖️ Legal Reminder: Only test systems you own or have explicit written permission to test."
                }
            },
            new ChatTopic
            {
                Name = "Network Security", Emoji = "🔗",
                Keywords = new[] { "network security", "network", "firewall", "ids", "ips", "intrusion", "packet", "dns", "ddos", "port" },
                Responses = new[]
                {
                    "🔗 NETWORK SECURITY\n\nNetwork security encompasses the policies, tools, and practices that protect the integrity, confidentiality, and availability of your network and data.\n\n🛡️ Core Defences:\n• Firewall — filters incoming and outgoing traffic based on rules\n• IDS/IPS — detects and prevents intrusion attempts in real time\n• VPN — encrypts traffic between endpoints\n• Network Segmentation — isolates sensitive systems to limit breach spread\n\n✅ Best Practices:\n• Disable unused ports and services\n• Use strong WPA3 encryption on wireless networks\n• Monitor logs for anomalous traffic patterns\n• Apply patches and firmware updates promptly\n\n🔴 Common Threats:\n• DDoS — flooding a network with traffic to cause downtime\n• Man-in-the-Middle — intercepting communications between two parties\n• DNS Spoofing — redirecting traffic to malicious destinations",
                    "🔗 NETWORK SECURITY — Zero Trust Architecture\n\nThe Zero Trust model operates on the principle: never trust, always verify. Every device and user — even inside your network — must authenticate before accessing resources.\n\n✅ Zero Trust Pillars:\n• Verify every identity, every time\n• Grant minimum necessary access (Least Privilege)\n• Assume breach and limit blast radius\n• Monitor and log all activity continuously\n\n💡 Zero Trust is now the gold standard for enterprise security and is rapidly becoming the norm across all sectors."
                }
            },
            new ChatTopic
            {
                Name = "AI Assistance", Emoji = "🤖",
                Keywords = new[] { "ai", "artificial intelligence", "machine learning", "ai security", "deepfake", "ai threat", "chatgpt", "llm", "ai help" },
                Responses = new[]
                {
                    "🤖 AI & CYBERSECURITY\n\nArtificial Intelligence is transforming both the attack and defence landscape of cybersecurity.\n\n🛡️ AI in Defence:\n• Anomaly detection — identifying unusual patterns in real time\n• Threat intelligence automation — processing millions of indicators instantly\n• Automated incident response — containing threats faster than any human\n• Behavioural analysis — detecting insider threats\n\n⚠️ AI-Powered Threats:\n• Deepfakes — AI-generated fake audio and video for social engineering\n• AI-generated phishing — highly personalised, convincing lure emails\n• Automated vulnerability discovery — attackers using AI to find exploits faster\n• LLM abuse — manipulating AI models to generate malicious content\n\n✅ Stay Ahead:\n• Be sceptical of unexpected audio/video communications\n• Verify AI-generated information from authoritative sources\n• Understand that AI tools can be manipulated — always apply human judgement",
                    "🤖 HOW CYBERSEC AI CAN HELP YOU\n\nI am an AI-powered cybersecurity assistant. Here is how I can assist:\n\n✅ What I Do:\n• Explain complex cybersecurity concepts in plain language\n• Guide you through security best practices\n• Help you understand threats relevant to your situation\n• Provide actionable steps to protect your digital life\n\n💡 Tips for Best Results:\n• Ask specific questions: 'How do I set up 2FA on Gmail?'\n• Type a topic number (1–13) for a focused briefing\n• Type 'help' to see the full topic menu\n\n🛡️ Remember: CyberSec AI is an educational tool. For active incidents or legal matters, contact certified cybersecurity professionals."
                }
            }
        };
    }
}