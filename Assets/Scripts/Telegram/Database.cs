using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

namespace Telegram
{
    public class Database : MonoBehaviour
    {
        private const string DATE_KEY = "FRIENDS";
        private const string LastDayKey = "LastDayKey";
        private const string SentTodayKey = "SentTodayKey";

        public List<Friend> friends = new List<Friend>();
        private Dictionary<long, Friend> map = new Dictionary<long, Friend>();
        
        
        private DateTime todayDate = DateTime.Today.ToUniversalTime();
        private DateTime weekDate = (DateTime.Today - TimeSpan.FromDays(7)).ToUniversalTime();
        private DateTime monthDate = (DateTime.Today - TimeSpan.FromDays(30)).ToUniversalTime();

        public int newToday = 0;
        public int newWeek = 0;
        public int newMonth = 0;
        
        
        private DateTime LastDay
        {
            get
            {
                var s = PlayerPrefs.GetString(LastDayKey, null);
                if (string.IsNullOrEmpty(s))
                {
                    return DateTime.Today - TimeSpan.FromDays(1);
                }
            
                return JsonConvert.DeserializeObject<DateTime>(s);
            }
            set => PlayerPrefs.SetString(LastDayKey, JsonConvert.SerializeObject(value));
        }
        
        
        public int SentToday
        {
            get => PlayerPrefs.GetInt(SentTodayKey, 0);
            set => PlayerPrefs.SetInt(SentTodayKey, value);
        }
        
        
        public Subject<Database> OnUpdated = new Subject<Database>();
            
        
        private void Awake()
        {
            var json = PlayerPrefs.GetString(DATE_KEY, string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                friends = JsonConvert.DeserializeObject<List<Friend>>(json);
                foreach (var friend in friends)
                {
                    map.Add(friend.id, friend);
                    UpdateCounters(friend);
                }
            }
            
            if (LastDay < DateTime.Today)
            {
                SentToday = 0;
                LastDay = DateTime.Today;
            }
        }

        public Friend PickLastFriend()
        {
            foreach (var friend in friends)
            {
                if (!friend.sent)
                {
                    return friend;
                }
            }

            return null;
        }

        public void Sent(Friend user)
        {
            if (user != null)
            {
                SentToday++;
                user.sent = true;
                Save();
                
                OnUpdated.OnNext(this);
            }
        }
        

        public void AddFriends(List<Friend> toAdd)
        {
            List<Friend> unique = new List<Friend>();
            
            foreach (var friend in toAdd)
            {
                if (map.TryGetValue(friend.id, out Friend old))
                {
                    old.activity++;
                    old.dateUpdated = friend.dateAdded;
                }
                else
                {
                    unique.Add(friend);
                    map.Add(friend.id, friend);
                    
                    UpdateCounters(friend);
                }
            }
            friends.InsertRange(0, unique);
            Save();
            
            OnUpdated.OnNext(this);
        }

        private void UpdateCounters(Friend friend)
        {
            if (friend.dateAdded > todayDate)
            {
                newToday++;
            }
            
            if (friend.dateAdded > weekDate)
            {
                newWeek++;
            }
            
            if (friend.dateAdded > monthDate)
            {
                newMonth++;
            }
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(friends);
            PlayerPrefs.SetString(DATE_KEY, json);
        }
    }
}
