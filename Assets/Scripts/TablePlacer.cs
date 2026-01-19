using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class TablePlacer : MonoBehaviour
{
    public GameObject tablePrefab;
    public ARRaycastManager raycastManager;
    public ARAnchorManager anchorManager;

    private GameObject spawnedTable;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                TrackableId planeId = hits[0].trackableId;
                ARPlane plane = raycastManager.GetComponent<ARPlaneManager>().GetPlane(planeId);

                if (spawnedTable == null)
                {
                    // Anchor the table to the plane
                    ARAnchor anchor = anchorManager.AttachAnchor(plane, hitPose);

                    if (anchor != null)
                    {
                        spawnedTable = Instantiate(tablePrefab, anchor.transform);
                    }
                }
                else
                {
                    // Move the table
                    spawnedTable.transform.position = hitPose.position;
                }
            }
        }
    }
}