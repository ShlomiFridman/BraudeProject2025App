using ProjectClasses;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace MenuScripts.Guide
{
    public class GuidePageBehaviour : MonoBehaviour
    {
        public RTLTextMeshPro title;
        public RTLTextMeshPro text;
        public Image image;

        public void SetGuide(GuideData guideData)
        {
            Sprite sprite = Resources.Load<Sprite>(guideData.imageSpritePath);
            if (sprite != null)
            {
                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("Sprite not found for guide at path: " + guideData.imageSpritePath);
            }

            bool isEnglish = LocalizationSettings.SelectedLocale.Identifier.Code == "en";

            title.text = isEnglish ? guideData.title : guideData.title_he;
            text.text = isEnglish ? guideData.text : guideData.text_he;
        }
    }
}