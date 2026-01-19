using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ProjectEnums;

namespace ProjectClasses
{

    [System.Serializable]
    public class SystemStatus
    {
        public List<CartInfo> carts;
        public Dictionary<StationEnum, StationOccupancy> stationOccupants;
        public string timestamp;

        public SystemStatus()
        {
            this.carts = new List<CartInfo>();
            this.stationOccupants = new Dictionary<StationEnum, StationOccupancy>();
        }
        public SystemStatus(List<CartInfo> carts, Dictionary<StationEnum, StationOccupancy> stationOccupants, string timestamp)
        {
            this.carts = carts;
            this.stationOccupants = stationOccupants;
            this.timestamp = timestamp;
        }
        
        public override string ToString()
        {
            var cartsString = carts != null 
                ? string.Join(", ", carts.Select(c => c.ToString())) 
                : "null";

            var occupantsString = stationOccupants != null
                ? string.Join(", ", stationOccupants.Select(kv => $"{kv.Key}: {kv.Value}"))
                : "null";

            return $"SystemStatus(Carts: [{cartsString}], StationOccupants: {{{occupantsString}}}, Timestamp: {timestamp})";
        }

        [System.Serializable]
        public class StationOccupancy
        {
            public string oldCart;
            public string newCart;

            public StationOccupancy()
            {
            }

            public StationOccupancy(string newCart, string oldCart)
            {
                this.newCart = newCart;
                this.oldCart = oldCart;
            }
        }
    }
}
