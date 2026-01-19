using ProjectClasses;
using RTLTMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class BadgeRow : MonoBehaviour
{

    private static readonly string badgeLockedSpritePath = "Sprites/Star-empty";
    private static readonly string badgeUnlockedDefaultSpritePath = "Sprites/Star-full";

    public Image image;
    public RTLTextMeshPro title;
    public RTLTextMeshPro description;
    public string spritePath;
    
    private Badge badge;
    
    private bool IsEnglish =>
        LocalizationSettings.SelectedLocale.Identifier.Code == "en";

    public void SetBadge(Badge badgeData, bool earned=false)
    {
        badge = badgeData;
        UpdateText();
        spritePath = $"Sprites/Badges/badge{badge.badge_id}";

        ToggleBadge(earned);
    }
    
    public void UpdateText()
    {
        title.text = IsEnglish ? badge.title : badge.title_he;
        description.text = IsEnglish ? badge.description : badge.description_he;
    }

    public void ToggleBadge(bool earned = false)
    {
        if (earned){
            image.sprite = Resources.Load<Sprite>(spritePath);
            if (!image.sprite){
                image.sprite = Resources.Load<Sprite>(badgeUnlockedDefaultSpritePath);
            }
        }
        else{
        image.sprite = Resources.Load<Sprite>(badgeLockedSpritePath);
        }
    }
}
