using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TdLib;
using UniRx;
using UnityEngine;
using Zenject;

namespace Telegram
{
    public class DataSource : MonoBehaviour
    {
        [SerializeField] private string groupId;
        [SerializeField] private int historyHourDeep = 3;
        [SerializeField] private int batchSize = 32;

        [Inject] private Authenticator telegram;
        [Inject] private Database database;
        
        private bool hasChat;
        private TdApi.Chat chat;
        
        public List<ChatMessage> chatHistory = new List<ChatMessage>();
        public ReactiveProperty<bool> Loading = new ReactiveProperty<bool>(false);
        
        
        private long lastLoadedMessageId
        {
            get => long.Parse(PlayerPrefs.GetString("lastLoadedMessageId", "0"));
            set => PlayerPrefs.SetString("lastLoadedMessageId", value.ToString());
        }
        
        
        private void Awake()
        {
            telegram.currentState.ObserveOnMainThread().Subscribe( (state =>
            {
                if (state is TdApi.AuthorizationState.AuthorizationStateReady)
                {
                    CheckChat();
                }
            })).AddTo(this);
        }

        
        private async void CheckChat()
        {
            var request = telegram.Client.GetChatsAsync(
                new TdApi.ChatList.ChatListMain(),
                long.MaxValue, 
                0, 
                100);
            
            await request;

            if (request.IsCompleted)
            {
                long chatId = long.Parse(groupId);
                if (ContainsId(request.Result.ChatIds, chatId))
                {
                    hasChat = true;
                    chat = await telegram.Client.GetChatAsync(chatId);
                    Fetch();
                }
            }
        }

        
        public async void Fetch(Action onComplete = null)
        {
            if (!hasChat)
                return;

            Loading.Value = true;
            
            long last = lastLoadedMessageId;
            if (last == 0)
            {
                int date = DateTime.Now.ToUniversalTime().Subtract(TimeSpan.FromHours(historyHourDeep)).ToEpoch();
                var oldest = await telegram.Client.GetChatMessageByDateAsync(chat.Id, date);
                last = oldest.Id;
            }

            Debug.Log($"Load from {last}");

            bool reachEnd = false;
            while (!reachEnd)
            {
                var task = telegram.Client.GetChatHistoryAsync(chat.Id, last, -batchSize, batchSize);
                    
                if (await Task.WhenAny(task, Task.Delay(5000)) == task)
                {
                    var data = task.Result;
                    
                    List<ChatMessage> list = new List<ChatMessage>();
                    List<Friend> friends = new List<Friend>();
            
                    foreach (var m in data.Messages_)
                    {
                        if (m.Content is TdApi.MessageContent.MessageText mt)
                        {
                            if (chatHistory.All(f => f.id != m.Id))
                            {
                                ChatMessage cm = new ChatMessage();
                                cm.id = m.Id;
                                cm.text = mt.Text.Text;
                                cm.date = DateTimeEpoch.FromEpoch(m.Date);
                                list.Add( cm);

                                var ids = Parse(cm.text);

                                var author = await telegram.Client.GetUserAsync(m.SenderUserId);
                                cm.author = $"{author.FirstName} {author.LastName}";

                                if (ids.Count > 0)
                                {
                                    foreach (var id in ids)
                                    {
                                        Friend friend = new Friend();
                                        friend.id = id;
                                        friend.dateAdded = cm.date;
                                        friend.dateUpdated = cm.date;
                                        friend.author = $"{author.FirstName} {author.LastName}";
                                        friend.date = $"{cm.date.ToLocalTime():HH:mm}";
                                        friends.Add(friend);
                                    }
                                }
                            }
                        }
                    }
            
                    chatHistory.InsertRange(0, list);
                    database.AddFriends(friends);
                    
                    if (data.TotalCount < 2)
                    {
                        reachEnd = true;
                    }
                    else
                    {
                        last = data.Messages_[0].Id;
                        lastLoadedMessageId = last;
                    }
                }
                else
                {
                    var message = await telegram.Client.GetMessageAsync(chat.Id, last);
                    
                    if (message.Content is TdApi.MessageContent.MessageText mt)
                    {
                        UnityEngine.Debug.Log($"{mt.Text.Text}");
                    }
                    
                    int nextDate = message.Date + 60 * 60;
                    long next = last;

                    while (next == last)
                    {
                        var nextMessage = await telegram.Client.GetChatMessageByDateAsync(chat.Id, nextDate);
                        nextDate += 60 * 60;
                        next = nextMessage.Id;
                        
                        if (nextMessage.Content is TdApi.MessageContent.MessageText mt2)
                        {
                            UnityEngine.Debug.Log($"{mt2.Text.Text}");
                        }
                    }

                    last = next;
                }
            }
            Debug.Log($"reachEnd");
            
            Loading.Value = false;
            onComplete?.Invoke();
        }
            
        private static List<long> Parse(string text)
        {
            List<long> ids = new List<long>();

            string user_id = String.Empty;
            foreach (var c in text)
            {
                if (Char.IsDigit(c))
                {
                    user_id += c;
                }
                else if (char.IsLetter(c) || char.IsPunctuation(c))
                {
                    user_id = String.Empty;
                }
                
                if (user_id.Length == 12)
                {
                    ids.Add(long.Parse(user_id));
                    user_id = String.Empty;
                }
            }
                    
            if (user_id.Length == 12)
            {
                ids.Add(long.Parse(user_id));
            }

            return ids;
        }

        private bool ContainsId(long[] chats, long id)
        {
            if(chats == null)
            {
                return false;
            }
            
            for (int i = 0; i < chats.Length; i++)
            {
                if (chats[i] == id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}


public static class TdApiExt
{
    public static void Print(this TdApi.Message message)
    {
        if (message.Content is TdApi.MessageContent.MessageText content)
        {
            Debug.Log(content.Text.Text);
        }
    }
}


public static class DateTimeEpoch
{
    public static int ToEpoch(this DateTime date)
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int epochTime = (int)(date - epochStart).TotalSeconds;
        return epochTime;
    }
        
    public static DateTime FromEpoch(long date)
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime epoch = epochStart.AddSeconds(date);
        return epoch;
    }
}
