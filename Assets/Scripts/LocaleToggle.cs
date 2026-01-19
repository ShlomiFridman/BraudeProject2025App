using MenuScripts.Badges;
using MenuScripts.Guide;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleToggle : MonoBehaviour
{
    private bool isEnglish = true;
    public bool IsEnglish => isEnglish;

    public async void ToggleLocale()
    {
        isEnglish = !isEnglish;

        var locale = isEnglish
            ? LocalizationSettings.AvailableLocales.GetLocale("en")
            : LocalizationSettings.AvailableLocales.GetLocale("he");
        
        LocalizationSettings.SelectedLocale = locale;
        
        if (QuizManager.Instance != null)
        {
            await QuizManager.Instance.FetchQuiz(QuizManager.Instance.CurrentGroup);
        }

        // Refresh guide menu
        if (GuideMenuBehaviour.Instance != null)
            GuideMenuBehaviour.Instance.RefreshGuideButtons();
        
        // Refresh badges menu
        if (BadgesManager.Instance != null)
        {
            BadgesManager.Instance.RefreshBadges();
        }
    }
}