using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WaypointClickedScript : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject waypointObj;

    // Reference to the UI Text component
    private TMP_Text infoText;
    private int cntr = 0;

    private void Start()
    {
        mainCamera = Camera.main;
        waypointObj = this.gameObject;
        infoText = GameObject.FindGameObjectWithTag("worldInfoText").GetComponent<TMP_Text>();
        // Debug.Log($"Started with {waypointObj.name}");
    }

    void Update()
    {
        bool cond = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
            (Application.isEditor && Input.GetMouseButtonDown(0));
        if (cond)
        {
            Ray ray;
            if (Application.isEditor)
            {
                ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            }
            else
            {
                ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            }
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == waypointObj)
                {
                    string name = waypointObj.name;
                    infoText.text = $"Clicked on {name} ({++cntr} times)";
                }
            }

        }
    }
}
