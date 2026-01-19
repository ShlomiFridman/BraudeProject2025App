using UnityEngine;
using ProjectClasses;
using TMPro;

public class StationBehaviour : MonoBehaviour
{

    private Camera mainCamera;
    private GameObject stationObject;
    private QuizManager quizManager;

    private Station station;

    void Start()
    {
        mainCamera = Camera.main;
        stationObject = AppUtils.FindChildWithTag(this.transform, "Waypoint");

        quizManager = GameObject.FindGameObjectWithTag("QuizUI").GetComponent<QuizManager>();
    }

    // void Update()
    // {
    //     Touch touch = AppUtils.getTouch();
    //     if (touch.fingerId != -1 && touch.phase == TouchPhase.Began)
    //     {
    //         Ray ray = mainCamera.ScreenPointToRay(touch.position);
    //         if (Physics.Raycast(ray, out RaycastHit hit))
    //         {
    //             if (hit.collider.gameObject == stationObject)
    //             {
    //                 handleOnTouch();
    //             }
    //         }
    //
    //     }
    // }

    public void SetStation(Station station)
    {
        this.station = station;
        Debug.Log($"Station '{this.name}' got set with station object {station}");
    }

    // private void handleOnTouch()
    // {
    //     if (this.station == null)
    //     {
    //         Debug.Log($"Station '{this.name}' still wasn't initialized with a station object");
    //         return;
    //     }
    //     Debug.Log($"Clicked on {this.name}");
    //
    //     quizManager.SetQuiz(this.station);
    // }
}
