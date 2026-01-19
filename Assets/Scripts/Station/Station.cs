using System.Collections.Generic;
using UnityEngine;
using ProjectEnums;

namespace ProjectClasses
{
    public class Station
    {
        private StationInfo stationInfo;
        public StationEnum StationId { get => (StationEnum)stationInfo.stationId; }

        private string currentCart;

        public string CurrentCart
        {
            get => currentCart;
            set
            {
                IsOccupied = !value.Equals(AppConstants.EmptyStationCartId);
                currentCart = value;
            }
        }

        public bool IsOccupied { get; private set; }

        public string Description { get => stationInfo.description; }

        public Waypoint Waypoint { get; private set; }

        public Dictionary<Station, float> TimeBetweenDic { get; set; }

        public Quiz Quiz { get => stationInfo.quiz; }

        public Station(StationInfo stationInfo, Waypoint waypoint)
        {
            this.stationInfo = stationInfo;
            this.Waypoint = waypoint;
            this.currentCart = AppConstants.EmptyStationCartId;
        }
        public override string ToString()
        {
            return $"Station(Id={StationId}, Description={Description}, " +
                   $"CurrentCart={CurrentCart ?? "null"}, IsOccupied={IsOccupied}, " +
                   $"WaypointId={Waypoint?.Id.ToString() ?? "null"})";
        }

    }
} 
