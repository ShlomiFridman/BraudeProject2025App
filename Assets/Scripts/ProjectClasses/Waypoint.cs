using UnityEngine;

namespace ProjectClasses
{
    public class Waypoint
    {

        public GameObject WaypointObject { get; private set; }
        public Waypoint PreviousWaypoint { get; set; }
        public Waypoint NextWaypoint { get; set; }
        private int id;
        public int Id
        {
            get => id;
            set
            {
                this.IsStation = value % 100 == 0;
                id = value;
            }
        }
        public bool IsStation { get; private set; }

        public Waypoint(GameObject gameObject, int id)
        {
            this.WaypointObject = gameObject;
            this.Id = id;
            this.IsStation = id % 100 == 0;
        }

        public override string ToString()
        {
            return $"Waypoint(Id={Id}, IsStation={IsStation}, " +
                   $"Previous={PreviousWaypoint?.Id.ToString() ?? "null"}, " +
                   $"Next={NextWaypoint?.Id.ToString() ?? "null"})";
        }

    }
} 
