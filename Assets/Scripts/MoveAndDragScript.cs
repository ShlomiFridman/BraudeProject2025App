using System;
using System.Collections.Generic;
using ProjectClasses;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MoveAndDragScript : MonoBehaviour
{
    
    public static MoveAndDragScript Instance;
    
    private static readonly List<ARRaycastHit> m_Hits = new();

    public GameObject promptTxt;
    
    private Camera mainCamera;

    private ARRaycastManager raycastManager;
    // public GameObject objectPrefab;
    private GameObject moveableObject;
    private Renderer objectRenderer;

    private string objectTag = "ProductionLineObject";
    
    private bool isDragging = false;
    private bool initialized = false;

    private bool interactable = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
    }
    
    private void Start()
    {
        moveableObject = null;
        raycastManager = GetComponent<ARRaycastManager>();
        mainCamera = Camera.main;
        moveableObject = GameObject.FindGameObjectWithTag(objectTag);
        objectRenderer =  moveableObject.GetComponent<Renderer>();
        // Debug.Log("Floor detection script started");
    }

    private void Update()
    {
        if (!interactable)
        {
            return;
        }
        
        var touch = AppUtils.getTouch();

        if (touch.fingerId == -1) return;


        if (!raycastManager.Raycast(touch.position, m_Hits, TrackableType.PlaneWithinPolygon)
            || !moveableObject)
        {
            return;
        }
        
        if (touch.phase == TouchPhase.Began)
        {
            var ray = mainCamera.ScreenPointToRay(touch.position);
            if (!initialized)
            {
                moveObject(m_Hits[0].pose.position);
                initialized = true;
                promptTxt.SetActive(false);
            }
            else if (touchedObject(m_Hits[0].pose.position))
            {
                this.isDragging = true;
            }
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            moveObject(m_Hits[0].pose.position);
        }
        else if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    public void ToggleInteractable(bool toggle)
    {
        this.interactable = toggle;
    }

    private bool touchedObject(Vector3 touchedPosition)
    {
        var objPos = moveableObject.transform.position;
        var dx = Math.Abs(touchedPosition.x - objPos.x);
        var dz = Math.Abs(touchedPosition.z - objPos.z);
        return dx <= 0.5f && dz <= 0.5f;
    }

    private void moveObject(Vector3 newPostion)
    {
        newPostion.y += newPostion.y + moveableObject.transform.localScale.y - 0.05f;
        moveableObject.transform.position = newPostion;
    }

    public void Reset()
    {
        moveableObject.transform.position = new Vector3(0, -200, 0);
        initialized = false;
        promptTxt.SetActive(true);
    }
    
    // private void SpawnPrefab(Vector3 spawnPosition)
    // {
    //     moveableObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    // }
}