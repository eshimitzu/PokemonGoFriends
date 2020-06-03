﻿using Telegram;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class PhoneView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button next;

        [Inject] private Authenticator telegram;

        
        private void Start()
        {
            next.OnClickAsObservable().Subscribe(_ =>
            {
                telegram.SendPhone(input.text);
            }).AddTo(this);
        }
    }
}
