using UnityEngine;
using UnityEngine.EventSystems;

public class CartSpeedDevBtnClick : MonoBehaviour
{
    public string cartId;
    public float speed;

    private SystemManager _systemManager;
    private CartBehaviour cartBehaviour;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _systemManager = EventSystem.current.GetComponent<SystemManager>();
    }

    public void OnClick()
    {
        _systemManager.UpdateCartSpeed(cartId, speed);
    }
}