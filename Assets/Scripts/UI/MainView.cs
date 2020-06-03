using System;
using Telegram;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class MainView : MonoBehaviour
    {
        [SerializeField] public SettingsView settingsView;
        [SerializeField] public Button settings;
        [SerializeField] public Button refresh;
        [SerializeField] public Transform loading;
        [SerializeField] public UserView userView;

        [SerializeField] private DataSource dataSource;

        [Inject] private Authenticator telegram;

        private void Start()
        {
            settings.OnClickAsObservable().Subscribe(_ =>
            {
                settingsView.gameObject.SetActive(true);
            }).AddTo(this);
            
            refresh.OnClickAsObservable().Subscribe(_ =>
            {
                dataSource.Fetch(() =>
                {
                    userView.PickNew();
                });
            }).AddTo(this);
            
            dataSource.Loading.Subscribe(l =>
            {
                loading.gameObject.SetActive(l);
                userView.gameObject.SetActive(!l);
            }).AddTo(this);
        }

        private void Update()
        {
            loading.Rotate(Vector3.forward, -300 * Time.deltaTime);
        }
    }
}
