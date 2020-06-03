using TdLib;
using Telegram;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private PhoneView phoneView;
        [SerializeField] private CodeView codeView;
        [SerializeField] private MainView mainView;

        [SerializeField] private TMP_Text statusLabel;

    
        [Inject] private Authenticator telegram;

    
        private void Awake()
        {
            telegram.currentState.ObserveOnMainThread().Subscribe( (state =>
            {
                statusLabel.text = state?.DataType;
            
                phoneView.gameObject.SetActive(state is TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber);
                codeView.gameObject.SetActive(state is TdApi.AuthorizationState.AuthorizationStateWaitCode);
                mainView.gameObject.SetActive(state is TdApi.AuthorizationState.AuthorizationStateReady);
            })).AddTo(this);
        }
    }
}
