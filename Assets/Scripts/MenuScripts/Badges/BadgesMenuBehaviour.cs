using System.Collections.Generic;
using System.Linq;
using MenuScripts.Badges;
using ProjectClasses;
using ProjectEnums;
using UnityEngine;

public class BadgesMenuBehaviour : MonoBehaviour
{

    public GameObject BadgesGroupPanel;
    public GameObject BadgeRowPrefab;
    
    private BadgesManager badgesManager;
    
    private Dictionary<int, BadgeRow> badgesRows = new();
    
    void Start()
    {
        badgesManager = BadgesManager.Instance;

        // if (badgesManager.IsLoaded)
        // {
        //     OnBadgesUpdated(badgesManager.Badges, badgesManager.UserBadges);
        // }
    }

    public void OnBadgesUpdated(List<Badge> badges, List<UserBadge> userBadges)
    {
        Debug.Log($"Badges count: {badges.Count},  userBadges count: {userBadges.Count}");
        foreach (Transform child in this.BadgesGroupPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        var earnedBadges = badges.ToDictionary(n => n.badge_id, n => (userBadges.Find(
            (ub)=> ub.badge_id == n.badge_id) != null));
        
        badgesRows.Clear();
        foreach (var badge in badges)
        {
            var badgeRow = Instantiate(BadgeRowPrefab, BadgesGroupPanel.transform);
            var badgeRowBehaviour = badgeRow.GetComponent<BadgeRow>();
            badgeRowBehaviour.SetBadge(badge, earnedBadges[badge.badge_id]);
            badgesRows.Add(badge.badge_id, badgeRowBehaviour);
        }
    }

    public void unlockBadge(int badgEnum)
    {
        UnityContext.Instance.Context.Post(_ =>
        {
            if (badgesRows.TryGetValue(badgEnum, out var row))
                row.ToggleBadge(true);
        }, null);
    }
    
    public void RefreshTexts()
    {
        foreach (var row in badgesRows.Values)
        {
            row.UpdateText();
        }
    }
    
}
