using System.Collections.Generic;
using MenuScripts.Badges;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using MenuScripts.Guide;
using ProjectClasses;
using ProjectEnums;
using RTLTMPro;

namespace MenuScripts.Guide
{
    public class GuideMenuBehaviour : MonoBehaviour
    {
        public static GuideMenuBehaviour Instance { get; private set; }

        public GameObject GuidePageGO;
        public GameObject GuideBtnsGroup;
        public Button GuideBtnPrefab;

        private GuidePageBehaviour _guidePageBehaviour;
        private List<GuideData> guides;

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        void Start()
        {
            _guidePageBehaviour = GuidePageGO.GetComponent<GuidePageBehaviour>();
            guides = AppUtils.GetGuideDataList();

            CreateGuideButtons();
        }

        /// <summary>
        /// Creates or recreates guide buttons according to the current locale
        /// </summary>
        public void CreateGuideButtons()
        {
            if (guides == null || GuideBtnsGroup == null || GuideBtnPrefab == null) return;

            // Clear old buttons first
            foreach (Transform child in GuideBtnsGroup.transform)
                Destroy(child.gameObject);

            bool isEnglish = LocalizationSettings.SelectedLocale.Identifier.Code == "en";

            for (int i = 0; i < guides.Count; i++)
            {
                var guide = guides[i];
                var guideBtn = Instantiate(GuideBtnPrefab, GuideBtnsGroup.transform);
                guideBtn.GetComponentInChildren<RTLTextMeshPro>().text = isEnglish ? guide.title : guide.title_he;

                int capturedIndex = i; // prevent closure issue
                guideBtn.onClick.AddListener(() => SelectGuideAction(guides[capturedIndex]));
            }
        }

        /// <summary>
        /// Refreshes button text only
        /// </summary>
        public void RefreshGuideButtons()
        {
            if (guides == null || GuideBtnsGroup == null) return;

            bool isEnglish = LocalizationSettings.SelectedLocale.Identifier.Code == "en";

            int count = Mathf.Min(guides.Count, GuideBtnsGroup.transform.childCount);

            for (int i = 0; i < count; i++)
            {
                var btn = GuideBtnsGroup.transform.GetChild(i).GetComponent<Button>();
                if (btn == null) continue;

                var guide = guides[i];
                var textComponent = btn.GetComponentInChildren<RTLTextMeshPro>();
                if (textComponent != null)
                    textComponent.text = isEnglish ? guide.title : guide.title_he;
            }
        }

        private void SelectGuideAction(GuideData guideData)
        {
            if (guideData == null)
            {
                Debug.LogWarning("SelectGuideAction :: null guide selected");
                return;
            }

            _guidePageBehaviour.SetGuide(guideData);
            MenuUtils.SetPage(MenuEnum.GuidePage);
            _ = BadgesManager.Instance.OnGuideRead(guideData.guideId);
        }
    }
}
