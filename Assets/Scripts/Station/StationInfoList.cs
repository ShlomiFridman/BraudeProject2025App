using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ProjectClasses
{

    [System.Serializable]
    public class StationInfoList
    {
        public List<StationInfo> stationInfoList;

        public StationInfoList() { }
        public StationInfoList(List<StationInfo> stationInfoList)
        {
            this.stationInfoList = stationInfoList;
        }
    }
}
