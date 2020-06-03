using Telegram;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UserStats : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI today;
        [SerializeField] public TextMeshProUGUI week;
        [SerializeField] public TextMeshProUGUI month;
        [SerializeField] public TextMeshProUGUI sent;

        [Inject] private Database database;

        private void Awake()
        {
            database.OnUpdated.Subscribe(_ =>
            {
                sent.text = $"Sent\n{database.SentToday}";
                today.text = $"Today\n{database.newToday}";
                week.text = $"Week\n{database.newWeek}";
                month.text = $"Month\n{database.newMonth}";
            }).AddTo(this);
        }
    }
}
