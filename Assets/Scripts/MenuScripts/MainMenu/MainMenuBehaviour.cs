using MenuScripts;
using UnityEngine;

namespace ProjectEnums
{
    public class MainMenuBehaviour : MonoBehaviour
    {

        public void GuidesBtnAction()
        {
            MenuUtils.SetPage(MenuEnum.Guides);
        }
        
        public void QuizBtnAction()
        {
            MenuUtils.SetPage(MenuEnum.Quiz);
        }

        public void BadgesBtnAction()
        {
            MenuUtils.SetPage(MenuEnum.Badges);
        }
        
        public void UpdateUsernameBtnAction()
        {
            MenuUtils.SetPage(MenuEnum.Username);
        }
        
        public void EndDemoBtnAction()
        {
            MenuUtils.SetPage(MenuEnum.SummaryPage);
        }
    }
}