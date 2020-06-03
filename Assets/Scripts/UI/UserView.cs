using Telegram;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UserView : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI nickname;
        [SerializeField] public TextMeshProUGUI datetime;
        [SerializeField] public TextMeshProUGUI activity;
        [SerializeField] public Button sent;

        [SerializeField] public QRCodeView qrCode;

        [Inject] private Database database;

        private Friend currentUser;

        private void Start()
        {            
            sent.OnClickAsObservable().Subscribe(_ =>
            {
                database.Sent(currentUser);
                PickNew();
            }).AddTo(this);

            PickNew();
        }

        public void PickNew()
        {
            SetUser(database.PickLastFriend());
        }

        private void SetUser(Friend user)
        {
            if (user != null)
            {
                var s = user.id.ToString();
                while (s.Length < 12)
                {
                    s = s.Insert(0, "0");
                }
                
                qrCode.Encode(s);
                nickname.text = user.author;
                activity.text = $"{user.activity}";
                datetime.text = $"{user.dateUpdated.ToLocalTime():HH:mm\nMM/dd/yyyy}";
            }
            else
            {
                qrCode.Encode("0");
                nickname.text = string.Empty;
                activity.text = string.Empty;
                datetime.text = string.Empty;
            }
            
            currentUser = user;
        }
    }
}
