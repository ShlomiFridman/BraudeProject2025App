using UnityEngine;
using UnityEngine.EventSystems;
using ProjectClasses;
using ProjectEnums;

public class EnteredCartDevBtnClick : MonoBehaviour
{

    private SystemManager _systemManager;
    private Station station;
    private StationEnum stationId;

    public int stationId_val;
    public string cartId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this._systemManager = EventSystem.current.GetComponent<SystemManager>();
        this.stationId = (StationEnum)stationId_val;
        this.station = AppUtils.getStations()[this.stationId];
    }

    public void OnClick()
    {
        this._systemManager.UpdateStationOccupent(stationId, cartId);
    }
}
