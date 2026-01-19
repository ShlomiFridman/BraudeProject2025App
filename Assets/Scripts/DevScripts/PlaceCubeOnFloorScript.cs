using System.Collections.Generic;
using ProjectClasses;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaceCubeOnFloorScript : MonoBehaviour
{
    private static readonly List<ARRaycastHit> m_Hits = new();

    [SerializeField] private GameObject spawnablePrefab;

    private ARAnchorManager anchorManager;

    private Camera mainCamera;
    private ARPlaneManager planeManager;

    private ARRaycastManager raycastManager;
    private GameObject spawnedObjected;

    private void Start()
    {
        spawnedObjected = null;
        raycastManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
        planeManager = GetComponent<ARPlaneManager>();
        mainCamera = Camera.main;
        // Debug.Log("Floor detection script started");
    }

    private void Update()
    {
        var touch = AppUtils.getTouch();

        if (touch.fingerId == -1) return;


        if (!raycastManager.Raycast(touch.position, m_Hits))
            // .Debug.Log("Raycast cond false");
            return;

        if (touch.phase == TouchPhase.Began && spawnedObjected == null)
        {
            var ray = mainCamera.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.gameObject.tag == "floorCube")
                    spawnedObjected = hit.collider.gameObject;
                else
                    SpawnPrefab(m_Hits[0].pose.position);
            }
        }
        else if (touch.phase == TouchPhase.Moved && spawnedObjected != null)
        {
            spawnedObjected.transform.position = m_Hits[0].pose.position;
        }

        if (touch.phase == TouchPhase.Ended) spawnedObjected = null;

        Debug.Log($"Touched! {touch.position}, {touch.phase}");
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObjected = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
    }
}