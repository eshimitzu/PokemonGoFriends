using System.Threading.Tasks;
using Newtonsoft.Json;
using TdLib;
using UniRx;
using UnityEngine;

namespace Telegram
{
    public class Authenticator : MonoBehaviour
    {
        private TdClient client = null;
        private TdApi.AuthorizationState authorizationState = null;
    
        public string authorizationMessage = null;
        public bool haveAuthorization = false;

        public ReactiveProperty<TdApi.AuthorizationState> currentState = new ReactiveProperty<TdApi.AuthorizationState>(null);

        public TdClient Client => client;


        private void Awake()
        {
            client = new TdClient();
            client.UpdateReceived += this.OnReceived;
        }
    

        private void OnDestroy()
        {
            //        client?.Dispose();
            client = null;
        }

        private static async void WaitResults<T>(Task<T> task)
        {
            await task;
            Debug.Log($"WaitResults : {JsonConvert.SerializeObject(task.Result)}");
        }

        private void OnReceived(object _, TdApi.Object structure)
        {
//            Debug.Log($"{JsonConvert.SerializeObject(structure)}");

            if (structure is TdApi.Update.UpdateAuthorizationState update)
            {
                OnAuthorizationStateUpdated(update.AuthorizationState);
            }
        }

        public void SendPhone(string phone)
        {
            if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber)
            {
                client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
                {
                    PhoneNumber = phone,
                    Settings = new TdApi.PhoneNumberAuthenticationSettings()
                    {
                        AllowFlashCall = false,
                        IsCurrentPhoneNumber = true
                    }
                });
            }
        }
    
        public void SendCode(string code)
        {
            if (authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitCode)
            {
                client.ExecuteAsync(new TdApi.CheckAuthenticationCode
                {
                    Code = code
                });
            }
        }
    
        private void OnAuthorizationStateUpdated(TdApi.AuthorizationState authorizationState)
        {        
            if (authorizationState != null)
            {
                this.authorizationState = authorizationState;
                authorizationMessage = authorizationState.DataType;
                currentState.Value = authorizationState;
            }
        
            if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters)
            {
                var parameters = new TdApi.TdlibParameters
                {
                    DatabaseDirectory = "tdlib",
                    UseSecretChats = true,
                    UseMessageDatabase = true,
                    ApiId = 1256967,
                    ApiHash = "32e70a453f4dc8f28223b35a0a497966",
                    SystemLanguageCode = "en",
                    DeviceModel = "Desktop",
                    SystemVersion = "MacOs",
                    ApplicationVersion = "1.0.0"
                };
        
                client.ExecuteAsync(new TdApi.SetTdlibParameters {Parameters = parameters});
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitEncryptionKey)
            {
                client.ExecuteAsync(new TdApi.CheckDatabaseEncryptionKey());
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber)
            {
//            client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
//            {
//                PhoneNumber = "",
//                Settings = new TdApi.PhoneNumberAuthenticationSettings()
//                {
//                    AllowFlashCall = false,
//                    IsCurrentPhoneNumber = true
//                }
//            });
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitOtherDeviceConfirmation state)
            {
                Debug.Log("Please confirm this login link on another device: " + state.Link);
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitCode)
            {
//            string code = ReadLine("Please enter authentication code: ");
//            _client.Send(new TdApi.CheckAuthenticationCode(code), new AuthorizationRequestHandler());
            }
//        else if (_authorizationState is TdApi.AuthorizationStateWaitRegistration)
//        {
//            string firstName = ReadLine("Please enter your first name: ");
//            string lastName = ReadLine("Please enter your last name: ");
//            _client.Send(new TdApi.RegisterUser(firstName, lastName), new AuthorizationRequestHandler());
//        }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitPassword)
            {
//            string password = ReadLine("Please enter password: ");
//            client.Send(new TdApi.CheckAuthenticationPassword(password), new AuthorizationRequestHandler());
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateReady)
            {
                haveAuthorization = true;
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateLoggingOut)
            {
                haveAuthorization = false;
                Debug.Log("Logging out");
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateClosing)
            {
                haveAuthorization = false;
                Debug.Log("Closing");
            }
            else if (this.authorizationState is TdApi.AuthorizationState.AuthorizationStateClosed)
            {
                Debug.Log("Closed");
                client.Dispose(); // _client is closed and native resources can be disposed now
//            if (!_quiting)
//            {
//                _client = CreateTdClient(); // recreate _client after previous has closed
//            }
            }
            else
            {
                Debug.Log($"Unsupported authorization state:\n{this.authorizationState}");
            }
        }
    }
}
