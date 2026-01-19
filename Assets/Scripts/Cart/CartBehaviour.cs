using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using ProjectClasses;
using ProjectEnums;

public class CartBehaviour : MonoBehaviour
{

    private static int globalNameId = 1;
    private int personalNameId = -1;

    private bool moveTriggered = false;
    private LinkedList<Waypoint> waypoints;
    private Waypoint targetWaypoint = null;

    public CartInfo CartInfo { get; private set; }
    private SystemManager _systemManager;

    private Transform cartTransform;
    // private GameObject cartInfoGroupUI;

    // private TMP_Text infoLabelScreenUI;
    private TMP_Text cartIdLabel;
    private TMP_Text speedLabel;

    // private TMP_Text cartInfoUILabel;
    // public TMP_Text cartInfoUILabel_prefab;
    public string Name { get; private set; }

    public void Awake()
    {
        this.personalNameId = globalNameId++;
        this.Name = $"Cart {this.personalNameId}";
        this.speedLabel = AppUtils.FindChildWithTag(transform, "cartSpeedLabel").GetComponent<TMP_Text>();
        this.cartIdLabel = AppUtils.FindChildWithTag(transform, "cartIdLabel").GetComponent<TMP_Text>();
        this.cartTransform = transform;
        // this.infoLabelScreenUI = GameObject.FindGameObjectWithTag("infoText").GetComponent<TMP_Text>();
        this._systemManager = EventSystem.current.GetComponent<SystemManager>();
        // this.cartInfoGroupUI = GameObject.FindGameObjectWithTag("worldUI_CartInfoGroup");
        // this.cartInfoUILabel = Instantiate(this.cartInfoUILabel_prefab, this.cartInfoGroupUI.transform);
    }

    private void Start()
    {
        waypoints = AppUtils.getWaypoints();
    }

    public void SetCart(CartInfo cartInfo)
    {
        this.CartInfo = cartInfo;
        this.cartIdLabel.text = cartInfo.cart_id;

        // this.cartInfoUILabel.text = $"{Name} | {cartInfo.cart_id}";
    }

    // Call this method to trigger movement
    public void StartMovement()
    {
        targetWaypoint = (targetWaypoint==null)? waypoints.First.Value : targetWaypoint.NextWaypoint;

        if (CartInfo == null)
        {
            Debug.Log($"Cart '{Name}' is without info");
            return;
        }

        if (!moveTriggered)
        {
            moveTriggered = true;
            // infoLabelScreenUI.text = $"INFO: {Name} movement triggered towards waypoint (id={targetWaypoint.Id})";
        }
        else
        {
            // infoLabelScreenUI.text = $"INFO: {Name} Already on the move";
        }
    }

    public void MoveToWaypoint(Waypoint wp, bool startMoving)
    {

        if (this.targetWaypoint == wp && this.moveTriggered == startMoving)
        {
            return;
        }
        else if (this.targetWaypoint != null && this.moveTriggered == false)
        {
            var station = AppUtils.getStationFromWaypoint(this.targetWaypoint);
            station.CurrentCart = AppConstants.EmptyStationCartId;
        }

        this.moveTriggered = false;
        this.targetWaypoint = wp;

        this.gameObject.transform.position = wp.WaypointObject.transform.position;
        this.cartTransform.LookAt(wp.NextWaypoint.WaypointObject.transform);

        if (startMoving)
        {
            this.StartMovement();
        }
    }

    void Update()
    {

        if (CartInfo == null)
        {
            Debug.Log($"Cart '{Name}' is without info");
            return;
        }

        if (moveTriggered && cartTransform != null && targetWaypoint != null)
        {
            Transform waypointTransform = targetWaypoint.WaypointObject.transform;
            // Debug.Log("Movement Updated!");
            // Move cube1 towards cube2 at constant speed.
            cartTransform.position = Vector3.MoveTowards(cartTransform.position, waypointTransform.position, this.CartInfo.speed * Time.deltaTime);
            cartTransform.LookAt(waypointTransform);

            speedLabel.text = $"{CartInfo.speed} [m/s]";
            if (Vector3.Distance(cartTransform.position, waypointTransform.position) < 0.001f)
            {
                moveTriggered = false;
                // infoLabelScreenUI.text = $"INFO: {Name} has reached waypoint {targetWaypoint.Id}";

                string targetName = waypointTransform.name;

                if (!targetWaypoint.IsStation)
                {
                    StartMovement();
                }
                else
                {
                    Station st = AppUtils.getStationFromWaypoint(targetWaypoint);
                    if (st == null)
                    {
                        Debug.Log($"Unexpected waypoint that is marked as station, waypointId='{targetWaypoint.Id}'");
                        return;
                    }
                    _systemManager.UpdateStationOccupent(st.StationId, CartInfo.cart_id);
                    AppUtils.ShowToast($"Cart '{CartInfo.cart_id}' has arrived to station '{st.StationId}'");
                }
            }
        }
        else
        {
            speedLabel.text = $"0 [m/s]";
        }
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
