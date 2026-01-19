using System.Threading.Tasks;
using MenuScripts.Toast;
using ProjectClasses;
using ProjectEnums;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MenuScripts.MenuManager
{
    public class MenuPanelBehaviour : MonoBehaviour
    {
        public GameObject openMenuBtn;

        public GameObject menuBackgroundPanel;
        public GameObject usernamePanel;
        public GameObject mainMenu;
        public GameObject guides;
        public GameObject guidePage;
        public GameObject quiz;
        public GameObject badgesMenu;
        public GameObject summaryPage;

        private GameObject activeMenu;

        public TMP_Text menuTitleTxt;


        public bool isClosed()
        {
            return activeMenu == null;
        }

        public void toggleMenu(bool open)
        {
            menuBackgroundPanel.SetActive(open);
            openMenuBtn.SetActive(!open);
            
            // ToastPanelManager.Instance.ShowToast($"Menu open? {open}", true);

            if (open)
            {
                setMenuWindow(mainMenu);
            }
            else
            {
                MoveAndDragScript.Instance.ToggleInteractable(true);
                activeMenu = null;
                menuTitleTxt.text = $"PLARA - {AppUtils.GetUsername()}";
            }
        }

        public void BackBtnAction()
        {
            if (activeMenu == mainMenu)
            {
                toggleMenu(false);
            }
            else if (activeMenu == guidePage)
            {
                setMenuWindow(guides);
            }
            else
            {
                setMenuWindow(mainMenu);
            }
        }

        public void OpenMenuBtnAction()
        {
            toggleMenu(true);
        }

        public void ResetPlacementBtnAction()
        {
            toggleMenu(false);
            MoveAndDragScript.Instance.Reset();
        }
        
        public void setMenuWindow(MenuEnum selectedMenu)
        {
            switch (selectedMenu)
            {
                case MenuEnum.Closed:
                    toggleMenu(false);
                    break;
                case MenuEnum.MainMenu:
                    setMenuWindow(mainMenu);
                    break;
                case MenuEnum.Guides:
                    setMenuWindow(guides);
                    break;
                case MenuEnum.GuidePage:
                    setMenuWindow(guidePage);
                    break;
                case MenuEnum.Badges:
                    setMenuWindow(badgesMenu);
                    break;
                case MenuEnum.Quiz:
                    setMenuWindow(quiz);
                    QuizManager.Instance.SetQuiz();
                    break;
                case MenuEnum.Username:
                    menuBackgroundPanel.SetActive(false);
                    openMenuBtn.SetActive(false);
                    usernamePanel.SetActive(true);
                    break;
                case MenuEnum.SummaryPage:
                    setMenuWindow(summaryPage);
                    summaryPage.GetComponent<SummaryPageBehavior>().DisplaySummaryPage();
                    break;
                default:
                    Debug.Log("Invalid menu selected: " + selectedMenu);
                    break;
            }
        }

        private void setMenuWindow(GameObject selected)
        {
            mainMenu.SetActive(selected == mainMenu);
            guides.SetActive(selected == guides);
            guidePage.SetActive(selected == guidePage);
            quiz.SetActive(selected == quiz);
            badgesMenu.SetActive(selected == badgesMenu);
            summaryPage.SetActive(selected == summaryPage);

            activeMenu = selected;
            MoveAndDragScript.Instance.ToggleInteractable(false);
        }
    }
}