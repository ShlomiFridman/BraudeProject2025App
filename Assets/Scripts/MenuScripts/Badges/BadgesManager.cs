using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MenuScripts.Guide;
using MenuScripts.UsernamePanel;
using ProjectClasses;
using ProjectEnums;
using TMPro;
using UnityEngine;

namespace MenuScripts.Badges
{
    public class BadgesManager : MonoBehaviour
    {
        
        public static BadgesManager Instance { get; private set; }

        public TMP_Text playtimeLabel;
        public GameObject badgesMenu;
        private BadgesMenuBehaviour _badgesMenuBehaviour;
    
        public List<Badge> Badges { get; private set; } = new List<Badge>();
        public List<UserBadge> UserBadges { get; private set; } = new List<UserBadge>();

        public bool IsLoaded => Badges != null &&  UserBadges != null && Badges.Count > 0;

        private UserProgress progress = null;
        private HashSet<BadgeEnum> earnedBadges = new();
        private DateTime startTime;
        private double initDurationMin;
        
        private bool isLoading = false;
        private bool isActive = true;

        private BadgesManager() { }
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            _badgesMenuBehaviour = badgesMenu.GetComponent<BadgesMenuBehaviour>();
        }

        void Update()
        {
            if (progress == null || isLoading || !isActive)
                return;
            
            updatePlayDuration();

            if (playtimeLabel)
            {
                var timespan = TimeSpan.FromMinutes(progress.playDurationMin);
                playtimeLabel.text = $"{timespan.Hours:D2}:{timespan.Minutes:D2}:{timespan.Seconds:D2}";
            }
            
            if (progress.playDurationMin >= 5 && !earnedBadges.Contains(BadgeEnum.Play5Min))
            {
                earnBadge(BadgeEnum.Play5Min);
            }
            
            if (progress.playDurationMin >= 10 && !earnedBadges.Contains(BadgeEnum.Play10Min))
            {
                earnBadge(BadgeEnum.Play10Min);
            }
        }

        public void StopTimer()
        {
            isActive = false;
            _ = UpdateProgress();
        }
        
        public void ResetTimer()
        {
            isActive = true;
        }

        private async void OnDestroy()
        {
            await UpdateProgress();
        }

        public void OnUsernameChange(string username)
        {
            _ = FetchBadges(username);
            _ = FetchProgress(username);
        }

        public async Task OnQuizComplete(float quizScore)
        {
            if (progress == null)
            {
                await FetchProgress(AppUtils.GetUsername());
            }

            if (progress != null && !earnedBadges.Contains(BadgeEnum.CompleteQuiz))
            {
                earnBadge(BadgeEnum.CompleteQuiz);
            }

            if (progress != null && quizScore >= 1f && !earnedBadges.Contains(BadgeEnum.PerfectQuiz))
            {
                earnBadge(BadgeEnum.PerfectQuiz);
            }
        }
        
        public async Task OnGuideRead(int guideId)
        {
            if (progress == null)
            {
                await FetchProgress(AppUtils.GetUsername());
            }
            
            if (progress != null && progress.guidesRead.Contains(guideId))
            {
                return;
            }
            
            progress.guidesRead.Add(guideId);
            
            if (progress.guidesRead.Count >= 4 && !earnedBadges.Contains(BadgeEnum.Read4Guides))
            {
                earnBadge(BadgeEnum.Read4Guides);
            }
            if (progress.guidesRead.Count == 3 && !earnedBadges.Contains(BadgeEnum.Read3Guides))
            {
                earnBadge(BadgeEnum.Read3Guides);
            }
            if (progress.guidesRead.Count == 2 && !earnedBadges.Contains(BadgeEnum.Read2Guides))
            {
                earnBadge(BadgeEnum.Read2Guides);
            }
            if (progress.guidesRead.Count == 1 && !earnedBadges.Contains(BadgeEnum.Read1Guide))
            {
                earnBadge(BadgeEnum.Read1Guide);
            }
            
        }

        public void earnBadge(BadgeEnum badgeEnum)
        {
            var badge = Badges.Find(b => b.badge_id == (int)badgeEnum);
            if (badge == null)
            {
                Debug.Log($"BadgeEnum without valid badge, badgeEnum = {nameof(badgeEnum)} ({(int)badgeEnum})");
                return;
            }
            earnedBadges.Add(badgeEnum);
            _badgesMenuBehaviour.unlockBadge(badge.badge_id);
            
            _ = HttpService.Instance.AddBadge(AppUtils.GetUsername(), badge.badge_id);
            _ = UpdateProgress();
            
            AppUtils.ShowToast($"Earned new badge: '{badge.title}'", true);
        }

        private async Task FetchBadges(string username)
        {
            try
            {
                var badgesTask = HttpService.Instance.GetBadges();
                var userBadgesTask = HttpService.Instance.GetUserBadges(username);
            
                await Task.WhenAll(badgesTask, userBadgesTask);
            
                if (badgesTask.Result != null)
                    Badges = badgesTask.Result;

                if (userBadgesTask.Result != null)
                {
                    UserBadges = userBadgesTask.Result;
                    earnedBadges.Clear();
                    foreach (var badge in UserBadges)
                        earnedBadges.Add((BadgeEnum) Enum.ToObject(typeof(BadgeEnum), badge.badge_id));
                }

                UnityContext.Instance.Context.Post(_ =>
                {
                    _badgesMenuBehaviour.OnBadgesUpdated(Badges, UserBadges);
                }, null);
            }
            catch (Exception e)
            {
                Debug.Log($"GetBadges :: Exception: {e.Message}");
            }
        }

        private async Task FetchProgress(string username)
        {
            isLoading = true;
            if (progress != null)
            {
                await UpdateProgress();
            }
            progress = await HttpService.Instance.GetProgress(username);
            if (progress == null)
            {
                Debug.Log($"Received null progress for username: {username}");
                progress = new UserProgress();
            }
            if (progress != null && progress.guidesRead == null)
            {
                progress.guidesRead = new List<int>();
            }
            
            startTime = DateTime.Now;
            initDurationMin = progress.playDurationMin;

            isLoading = false;
        }

        private void updatePlayDuration()
        {
            if (progress == null)
                return;
            var diff = DateTime.Now - startTime;
            progress.playDurationMin = diff.TotalMinutes + initDurationMin; 
        }
        
        private async Task UpdateProgress()
        {
            updatePlayDuration();
            await HttpService.Instance.UpdateProgress(progress);
        }
        
        public void RefreshBadges()
        {
            if (_badgesMenuBehaviour != null)
                _badgesMenuBehaviour.RefreshTexts();
        }
        
    }
}
