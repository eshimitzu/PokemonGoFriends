using TdLib;
using Telegram;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class SettingsView : MonoBehaviour
    {
        [SerializeField] public Button close;
        [SerializeField] public Button load;
        [SerializeField] public Button save;
        [SerializeField] public Button clean;
        [SerializeField] public TextMeshProUGUI cleanText;
        [SerializeField] public Button logout;

        [Inject] private Authenticator telegram;
        [Inject] private Database database;

        private bool confirm = false;
        

        private void Awake()
        {
            close.OnClickAsObservable().Subscribe(_ =>
            {
                confirm = false;
                cleanText.text = "Clean";
                
                gameObject.SetActive(false);
            }).AddTo(this);
            
            load.OnClickAsObservable().Subscribe(_ =>
            {
            }).AddTo(this);
            
            save.OnClickAsObservable().Subscribe(_ =>
            {
            }).AddTo(this);
            
            clean.OnClickAsObservable().Subscribe(_ =>
            {
                if (confirm == false)
                {
                    confirm = true;
                    cleanText.text = "Sure?";
                }
                else
                {
                    confirm = false;
                    cleanText.text = "Clean";
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                }
            }).AddTo(this);
        
            logout.OnClickAsObservable().Subscribe(_ =>
            {
                telegram.Client.LogOutAsync();
            }).AddTo(this);
        }
    }
}
