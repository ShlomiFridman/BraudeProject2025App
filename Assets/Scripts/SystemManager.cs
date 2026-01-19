using System;
using UnityEngine;
using ProjectClasses;
using System.Collections.Generic;
using ProjectEnums;

public class SystemManager : MonoBehaviour
{

    private Dictionary<StationEnum, Station> stations;
    public Dictionary<string, CartBehaviour> carts { get; private set; }

    public GameObject ProdLineGroup;
    public GameObject CartPefab;

    void Start()
    {
        this.stations = AppUtils.getStations();
        this.carts = new Dictionary<string, CartBehaviour>();

        //updateStation(StationEnum.STATION_1, "Cart 1");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void UpdateStationOccupent(StationEnum stationId, string newCart)
    {
        UpdateStationOccupent(stationId, new SystemStatus.StationOccupancy(newCart, AppConstants.EmptyStationCartId));
    }
    public void UpdateStationOccupent(StationEnum stationId, SystemStatus.StationOccupancy occupancy)
    {
        if (!stations.ContainsKey(stationId))
        {
            Debug.Log($"Unkown station id='{stationId}'");
            return;
        }

        Station station = stations[stationId];
        string oldOccupentId = station.CurrentCart;

        var enteredCart = getOrCreate(occupancy.newCart);
        var enteredCartId = enteredCart ? enteredCart.CartInfo.cart_id : AppConstants.EmptyStationCartId;
        var exitedCart = getOrCreate(oldOccupentId);
        var exitCartId = exitedCart ? exitedCart.CartInfo.cart_id : AppConstants.EmptyStationCartId;
        
        if (exitedCart == enteredCart)
        {
            return;
        }

        Debug.Log($"UpdateStationOccupent: {stationId} | Old id = '{exitCartId}' | New id = ''{enteredCartId}'");

        if (enteredCart && !enteredCartId.Equals(station.CurrentCart))
        {
            enteredCart.MoveToWaypoint(station.Waypoint, false);
        }
        if (!occupancy.oldCart.Equals(AppConstants.EmptyStationCartId) &&
            occupancy.oldCart.Equals(exitCartId))
        {
            exitedCart.MoveToWaypoint(station.Waypoint, true);
        }
        // if the oldCart that was received doesn't match the one in the system
        // else if (!occupancy.oldCart.Equals(exitCartId))
        // {
        //     Debug.Log($"OldCart is out of sync: System_oldCart='{exitCartId}', Server_oldCart='{occupancy.oldCart}'");
        // }

        station.CurrentCart = occupancy.newCart;
    }

    public void UpdateCartSpeed(string cartId, float newSpeed)
    {
        CartBehaviour cartBehaviour = getOrCreate(cartId);
        float oldSpeed = cartBehaviour.CartInfo.speed;
        cartBehaviour.CartInfo.speed = newSpeed;

        if (oldSpeed != newSpeed)
            Debug.Log($"UpdateCartSpeed: {cartBehaviour.CartInfo.cart_id} | Old speed = '{oldSpeed}' | New speed = '{newSpeed}'");
    }

    public void ResetSystem()
    {

        // 1️⃣ Reset all stations to empty (no current cart)
        foreach (var kvp in stations)
        {
            Station station = kvp.Value;
            
            station.CurrentCart = AppConstants.EmptyStationCartId;
        }

        // 2️⃣ Destroy all cart GameObjects in the scene
        foreach (var kvp in carts)
        {
            CartBehaviour cart = kvp.Value;
            if (cart != null)
            {
                cart.DestroySelf();
            }
        }

        // 3️⃣ Clear cart dictionary
        carts.Clear();

        Debug.Log("ResetSystem: system fully reset");
    }

    private CartBehaviour getOrCreate(string cartId)
    {
        if (string.IsNullOrEmpty(cartId) || cartId.Equals(AppConstants.EmptyStationCartId))
        {
            return null;
        }
        else if (carts.ContainsKey(cartId))
        {
            return carts[cartId];
        }

        CartInfo cart = new CartInfo(cartId);
        GameObject cartObj = Instantiate(this.CartPefab, this.ProdLineGroup.transform);
        CartBehaviour cartBehaviour = cartObj.GetComponent<CartBehaviour>();
        cartBehaviour.SetCart(cart);
        carts.Add(cartId, cartBehaviour);

        Debug.Log($"Crated new cart with id='{cartId}'");

        //cart.Speed = 1f;

        return cartBehaviour;
    }
}
