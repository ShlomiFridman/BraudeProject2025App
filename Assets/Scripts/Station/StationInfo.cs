using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ProjectClasses
{
    [System.Serializable]
    public class StationInfo
    {
        public int stationId;
        public string description;
        public Quiz quiz;

        public StationInfo() { }
        public StationInfo(int id, string desc) : this()
        {
            this.stationId = id;
            this.description = desc;
        }
        public StationInfo(int id, string desc, Quiz quiz) : this(id,desc)
        {
            this.quiz = quiz;
        }
        public override string ToString()
        {
            return $"StationInfo(Id={stationId}, Description='{description}', HasQuiz='{quiz!=null}')";
        }


    }
}
