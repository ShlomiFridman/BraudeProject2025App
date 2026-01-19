using ProjectClasses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts.Toast
{
    public class ToastPanelManager : MonoBehaviour
    {
        public static ToastPanelManager Instance { get; private set; }

        public bool isBadgeToast;
        
        public GameObject outerToastPanel;
        public GameObject innerToastPanel;
        public TMP_Text toastText;

        private float fadeDuration = AppConstants.ToastFadeDurationSec;
        private float defaultVisibleDuration = AppConstants.ToastDefaultVisibleDurationSec;
        private float visibleDuration = 2f;
        
        private float timer;

        private Image outerImage;
        private Image innerImage;

        private float hideTime;

        private enum ToastState
        {
            Idle,
            FadingIn,
            Visible,
            FadingOut
        };
        private ToastState state = ToastState.Idle;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            outerImage = outerToastPanel.GetComponent<Image>();
            innerImage = innerToastPanel.GetComponent<Image>();
        }

        void Update()
        {
            if (state == ToastState.Idle)
                return;

            timer += Time.deltaTime;

            switch (state)
            {
                case ToastState.FadingIn:
                case ToastState.FadingOut:
                    fadeAction();
                    break;
                case ToastState.Visible:
                    if (timer >= visibleDuration)
                    {
                        timer = 0f;
                        state = ToastState.FadingOut;
                    }
                    break;
            }
        }

        private void fadeAction()
        {

            var alpha = 0.5f;

            if (state == ToastState.FadingIn)
            {
                alpha = Mathf.Clamp01(timer / fadeDuration);
            }
            else if (state == ToastState.FadingOut)
            {
                alpha = Mathf.Clamp01(1f - (timer / fadeDuration));
            }
            else
            {
                return;
            }
            
            setAlpha(alpha);

            if (timer >= fadeDuration)
            {
                timer = 0f;
                if (state == ToastState.FadingOut)
                {
                    state = ToastState.Idle;
                    outerToastPanel.SetActive(false);
                }
                else if (state ==  ToastState.FadingIn)
                    state =  ToastState.Visible;
            }
        }

        private void setAlpha(float alpha)
        {
            var innerColor = innerImage.color;
            innerColor.a = alpha;
            innerImage.color = innerColor;
            
            var outerColor = outerImage.color;
            outerColor.a = innerColor.a;
            outerImage.color = outerColor;
            
            var textColor = toastText.color;
            textColor.a = outerColor.a;
            toastText.color = textColor;
        }

        public void ShowToast(string message, bool isBadgeToast = false, float duration = -1f)
        {
            visibleDuration = duration <= 0f? defaultVisibleDuration : duration;
            this.isBadgeToast = isBadgeToast;

            Debug.Log($"Toast message: \"{message}\"");
            toastText.text = message;
            outerToastPanel.SetActive(true);
            timer = 0f;
            state = ToastState.FadingIn;
            
            setAlpha(0f);
        }
    }
}