using MenuScripts;
using MenuScripts.Badges;
using ProjectClasses;
using ProjectEnums;
using RTLTMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SummaryPageBehavior : MonoBehaviour
{
    public RTLTextMeshPro quizResultsText;
    public GameObject backBtn;
    
    private bool IsEnglish =>
        LocalizationSettings.SelectedLocale.Identifier.Code == "en";

    public void DisplaySummaryPage()
    {
        BadgesManager.Instance.StopTimer();
        backBtn.SetActive(false);
        
        quizResultsText.text = QuizManager.Instance.BuildLastQuizSummary(IsEnglish);
    }

    public void onLinktreeBtnClicked()
    {
        Application.OpenURL(AppConstants.LinktreeURL);
    }
    
    public void onResetBtnClick()
    {
        backBtn.SetActive(true);
        MenuUtils.SetPage(MenuEnum.Username);
        BadgesManager.Instance.ResetTimer();
    }
}
