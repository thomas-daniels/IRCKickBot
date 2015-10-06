using System.Text.RegularExpressions;

namespace IRCKickBot
{
    class KickPattern
    {
        public string Reason
        {
            get;
            private set;
        }

        public Regex RegularExpression
        {
            get;
            private set;
        }

        public KickPattern(string reason, string pattern)
        {
            Reason = reason;
            RegularExpression = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
    }
}
