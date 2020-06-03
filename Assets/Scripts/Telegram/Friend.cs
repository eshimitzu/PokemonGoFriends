using System;

namespace Telegram
{
    [Serializable]
    public class Friend
    {
        public string date;

        public string author;
        public long id;

        public DateTime dateAdded;
        public DateTime dateUpdated;
        
        public int activity;
        public bool sent;
        

        public override string ToString()
        {
            return $"[{dateUpdated.ToLocalTime():HH:mm}] [{id:#### #### ####}] [{author}]";
        }
    }
}
