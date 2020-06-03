using System;

namespace Telegram
{
    [Serializable]
    public class ChatMessage
    {
        public string text;
        public long id;
        public DateTime date;
        public string author;

        public override string ToString()
        {
            return $"[{date.ToLocalTime():HH:mm}] [{author}] ::: {text}";
        }
        
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
