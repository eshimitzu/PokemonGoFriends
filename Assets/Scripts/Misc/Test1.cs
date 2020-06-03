//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using UnityEngine;
//using Td = TdLib;
//
//public class Test1 : MonoBehaviour
//{
//    private Td.TdClient client = null;
////    private Td.Hub hub = null;
////    private Td.Dialer dialer = null;
//    
//    private Td.TdApi.AuthorizationState _authorizationState = null;
//    
//    public string authorizationMessage = null;
//    public bool haveAuthorization = false;
//    public string code = "";
//
//    private Hyab hyab = null;
//
//    
//    private void Awake()
//    {
////        Td.Client.Log.SetVerbosityLevel(5);
////        client = new Td.Client();
////        hub = new Td.Hub(client);
////        dialer = new Td.Dialer(client, hub);
//        
////        hyab = new Hyab(client);
//        
//        var task = Task.Factory.StartNew(
//            () => hyab.Start(),
//            TaskCreationOptions.LongRunning);
//                    
//        task.ContinueWith(t =>
//        {
//            var exception = t.Exception;
//            if (exception != null)
//            {
//                Debug.Log($"{exception}");
//                // TODO: handle exception and shutdown
//            }
//        });
//        
////        var task = Task.Factory.StartNew(
////            () => hub.Start(),
////            TaskCreationOptions.LongRunning);
////                    
////        task.ContinueWith(t =>
////        {
////            var exception = t.Exception;
////            if (exception != null)
////            {
////                Debug.Log($"{exception}");
////                // TODO: handle exception and shutdown
////            }
////        });
//
//        hyab.Received += this.OnReceived;
//    }
//
//    private void OnDestroy()
//    {
//        dialer?.Dispose();
//        hub?.Stop();
//    }
//
//    private void OnGUI()
//    {
//        code = GUI.TextField(new Rect(100, 1000, 400, 50), code);
//        
//        GUILayout.BeginArea(new Rect(100, 100, 400, 1000));
//        {
//            if(GUILayout.Button("GetAuthorizationState"))
//            {
//                var r = dialer.ExecuteAsync(new Td.TdApi.GetAuthorizationState());
//                Debug.Log($"{JsonConvert.SerializeObject(r)}");
//            }
//            
//            if(GUILayout.Button("CheckDatabaseEncryptionKey"))
//            {
//                dialer.ExecuteAsync(new Td.TdApi.CheckDatabaseEncryptionKey());
//            }
//            
//            if(GUILayout.Button("AuthorizationSendCode"))
//            {
//                if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateWaitCode)
//                {
//                    dialer.ExecuteAsync(new Td.TdApi.CheckAuthenticationCode
//                    {
//                        Code = code
//                    });
//                    code = "";
//                }
//            }
//            
//            if(GUILayout.Button("GetMe"))
//            {
//                var t = dialer.ExecuteAsync(new Td.TdApi.GetMe()
//                {
//                    
//                });
//
//                WaitResults(t);
//            }
//            
//            if(GUILayout.Button("Chats"))
//            {
//                var t = dialer.ExecuteAsync(new Td.TdApi.GetChats()
//                {
//                    OffsetOrder = 0,
//                    OffsetChatId = 0,
//                    Limit = 100
//                });
//
//                WaitResults(t);
//            }
//            
//            
//            if(GUILayout.Button("LogOut"))
//            {
//               var r = dialer.ExecuteAsync(new Td.TdApi.LogOut());
//               Debug.Log($"{JsonConvert.SerializeObject(r)}");
//            }
//        }
//        GUILayout.EndArea();
//    }
//
//    private static async void WaitResults<T>(Task<T> task)
//    {
//        while (!task.IsCompleted)
//        {
//            await Task.Yield();
//        }
//
//        UnityEngine.Debug.Log($"{task.Result}");
//    }
//
//    private void OnReceived(object _, Td.TdApi.Object structure)
//    {
//        Debug.Log($"{JsonConvert.SerializeObject(structure)}");
//
//        if (structure is Td.TdApi.Update.UpdateAuthorizationState update)
//        {
//            OnAuthorizationStateUpdated(update.AuthorizationState);
//        }
//    }
//    
//    private void OnAuthorizationStateUpdated(Td.TdApi.AuthorizationState authorizationState)
//    {        
//        if (authorizationState != null)
//        {
//            _authorizationState = authorizationState;
//            authorizationMessage = authorizationState.DataType;
//        }
//        
//        if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters)
//        {
//            var parameters = new Td.TdApi.TdlibParameters
//            {
//                DatabaseDirectory = "tdlib",
//                UseSecretChats = true,
//                UseMessageDatabase = true,
//                ApiId = 1256967,
//                ApiHash = "32e70a453f4dc8f28223b35a0a497966",
//                SystemLanguageCode = "en",
//                DeviceModel = "Desktop",
//                SystemVersion = "MacOs",
//                ApplicationVersion = "1.0.0"
//            };
//        
//            dialer.ExecuteAsync(new Td.TdApi.SetTdlibParameters {Parameters = parameters});
//        }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateWaitEncryptionKey)
//        {
//            dialer.ExecuteAsync(new Td.TdApi.CheckDatabaseEncryptionKey());
//        }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber)
//        {
//            string phoneNumber = "+375298709514";
//            dialer.ExecuteAsync(new Td.TdApi.SetAuthenticationPhoneNumber
//            {
//                PhoneNumber = phoneNumber,
//                AllowFlashCall = false,
//                IsCurrentPhoneNumber = true
//            });
//        }
////        else if (_authorizationState is TdApi.AuthorizationState.AuthorizationStateWaitOtherDeviceConfirmation state)
////        {
////            Console.WriteLine("Please confirm this login link on another device: " + state.Link);
////        }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateWaitCode)
//        {
////            string code = ReadLine("Please enter authentication code: ");
////            _client.Send(new TdApi.CheckAuthenticationCode(code), new AuthorizationRequestHandler());
//        }
////            else if (_authorizationState is TdApi.AuthorizationStateWaitRegistration)
////            {
////                string firstName = ReadLine("Please enter your first name: ");
////                string lastName = ReadLine("Please enter your last name: ");
////                _client.Send(new TdApi.RegisterUser(firstName, lastName), new AuthorizationRequestHandler());
////            }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateWaitPassword)
//        {
////            string password = ReadLine("Please enter password: ");
////            _client.Send(new TdApi.CheckAuthenticationPassword(password), new AuthorizationRequestHandler());
//        }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateReady)
//        {
//            haveAuthorization = true;
////                _gotAuthorization.Set();
//        }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateLoggingOut)
//        {
//            haveAuthorization = false;
//            Debug.Log("Logging out");
//        }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateClosing)
//        {
//            haveAuthorization = false;
//            Debug.Log("Closing");
//        }
//        else if (_authorizationState is Td.TdApi.AuthorizationState.AuthorizationStateClosed)
//        {
//            Debug.Log("Closed");
//            dialer.Dispose();
//            client.Dispose(); // _client is closed and native resources can be disposed now
////            if (!_quiting)
////            {
////                _client = CreateTdClient(); // recreate _client after previous has closed
////            }
//        }
//        else
//        {
//            Debug.Log($"Unsupported authorization state:\n{_authorizationState}");
//        }
//    }
//}
