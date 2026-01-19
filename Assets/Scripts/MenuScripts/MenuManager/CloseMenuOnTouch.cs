using ProjectEnums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MenuScripts.MenuManager
{
    public class CloseMenuOnTouch : MonoBehaviour, IPointerClickHandler
    {
        public GameObject windowPanel;
        private bool _isActive = true;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isActive)
                return;
            
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    windowPanel.GetComponent<RectTransform>(),
                    eventData.position,
                    eventData.pressEventCamera) && MenuUtils.IsOpen())
            {
                MenuUtils.CloseMenu();
                // Debug.Log("Closed menu via outside click");
            }
        }

        public void Stop()
        {
            _isActive = false;
        }
    }
}
