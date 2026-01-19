using System;
using MenuScripts.Badges;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts.UsernamePanel
{
    public class UsernameBehaviour : MonoBehaviour
    {
        public static UsernameBehaviour Instance { get; private set; }

        public static readonly string NoUsername = "N/A";
        
        
        public TMP_InputField usernameField;
        public TMP_Dropdown groupNumberField;
        public Button submitButton;
        
        private string username = NoUsername;
        private int groupNumber = -1;
        private int[] groupNumberValues = { 1, 2, 3};
        
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void OnInputChanged()
        {
            submitButton.gameObject.SetActive(!string.IsNullOrEmpty(usernameField.text));
        }

        public string GetUsername()
        {
            return usernameField.text ?? NoUsername;
        }

        public void SubmitClick()
        {
            username = usernameField.text ??  NoUsername;
            groupNumber =  groupNumberValues[groupNumberField.value];
            
            gameObject.SetActive(false);
            
            Debug.Log($"Username set to: {username}");
            
            MenuUtils.CloseMenu(true);

            OnUsernameChanged();
        }

        public void OnUsernameChanged()
        {
            BadgesManager.Instance.OnUsernameChange(username);
            _ = QuizManager.Instance.FetchQuiz(groupNumber);
        }

    }
}