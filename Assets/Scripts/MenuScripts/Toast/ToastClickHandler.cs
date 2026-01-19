using ProjectEnums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MenuScripts.Toast
{
    public class ToastClickHandler : MonoBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ToastPanelManager.Instance.isBadgeToast)
            {
                MenuUtils.OpenMenu();
                MenuUtils.SetPage(MenuEnum.Badges);
            }
        }
    
    }
}
