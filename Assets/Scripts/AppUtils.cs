using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MenuScripts.Toast;
using UnityEngine;
using MenuScripts.Guide;
using MenuScripts.UsernamePanel;
using ProjectEnums;
using UnityEngine.Localization.Settings;
using WebSocket;

namespace ProjectClasses
{
    public static class AppUtils
    {

        private static LinkedList<Waypoint> waypoints = null;
        private static Dictionary<int, Waypoint> waypointsDic = null;
        private static Dictionary<StationEnum, Station> stations = null;

        public static string GetUsername()
        {
            return UsernameBehaviour.Instance?.GetUsername() ?? "UsernameBehaviour not initialized";
        }

        public static Touch getTouch()
        {
            
            if (Input.touchCount != 0)
            {
                return Input.GetTouch(0);
            }

            TouchPhase phase;
            if (Input.GetMouseButtonDown(0))
            {
                phase = TouchPhase.Began;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                phase = TouchPhase.Ended;
            }
            else if (Input.GetMouseButton(0))
            {
                phase = TouchPhase.Moved;
            }
            else
            {
                return new Touch
                {
                    fingerId = -1
                };
            }

            Touch touch = new Touch
            {
                fingerId = 0,
                position = Input.mousePosition,
                phase = phase,
                type = TouchType.Direct
            };

            return touch;
        }

        public static GameObject FindChildWithTag(Transform parent, string tag)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                    return child.gameObject;

                GameObject result = FindChildWithTag(child, tag);
                if (result != null)
                    return result;
            }
            return null;
        }


        public static LinkedList<Waypoint> getWaypoints()
        {
            if (AppUtils.waypoints != null)
            {
                return AppUtils.waypoints;
            }

            GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

            List<Waypoint> list = new List<Waypoint>();
            foreach (var wp in waypoints)
            {
                string[] parts = wp.name.Split('_');
                int num = -1;
                int.TryParse(parts.Last(), out num);
                var waypoint = new Waypoint(wp, num);
                list.Add(waypoint);

            }
            AppUtils.waypoints = new LinkedList<Waypoint>(list.OrderBy(wp => wp.Id));
            AppUtils.waypointsDic = new Dictionary<int, Waypoint>();

            for (int i = 0; i < AppUtils.waypoints.Count; i++)
            {
                var wp = AppUtils.waypoints.ElementAt(i);
                wp.NextWaypoint = (i != AppUtils.waypoints.Count-1) ? AppUtils.waypoints.ElementAt(i + 1) : AppUtils.waypoints.First.Value;
                wp.PreviousWaypoint = (i != 0) ? AppUtils.waypoints.ElementAt(i - 1) : AppUtils.waypoints.Last.Value;

                AppUtils.waypointsDic.Add(wp.Id, wp);
            }

            //var stations = loadStations("bin/stationsInfo.json");
            //foreach (var st in stations)
            //{
            //    Debug.Log(st);
            //}
            //saveStationsInfo(null, "bin/stationsInfo.json");


            return AppUtils.waypoints;
        }

        public static Dictionary<StationEnum, Station> getStations()
        {
            if (AppUtils.stations == null)
            {
                loadStations(AppConstants.StationsInfoDataFile);
            }
            return AppUtils.stations;
        }

        public static Station getStationFromWaypoint(Waypoint wp)
        {
            if (wp.Id % 100 != 0)
                return null;
            
            foreach (var kv in getStations())
            {
                if (kv.Value.Waypoint == wp)
                    return kv.Value;
            }

            return null;
        }

        public static AnalyticsManager GetAnalyticsManager()
        {
            return GameObject.FindGameObjectWithTag("EventSystemTag").GetComponent<AnalyticsManager>();
        }
        
        public static SystemManager GetSystemManager()
        {
            return GameObject.FindGameObjectWithTag("EventSystemTag").GetComponent<SystemManager>();
        }

        public static WebSocketClient GetWebSocketClient()
        {
            return GameObject.FindGameObjectWithTag("EventSystemTag").GetComponent<WebSocketClient>();
        }
        
        public static List<GuideData> GetGuideDataList()
        {
            
            var textAsset = Resources.Load<TextAsset>(AppConstants.GuidesDataFile);
            var stationsGuideList =
                JsonUtility.FromJson<GuideDataList>(textAsset.text);
            
            //string json = File.ReadAllText(textAsset.text);
            return stationsGuideList.guideDataList;
        }
        
        public static Quiz GetQuiz()
        {
            
            var textAsset = Resources.Load<TextAsset>(AppConstants.AppQuizDataFile);
            var quiz =
                JsonUtility.FromJson<Quiz>(textAsset.text);
            
            //string json = File.ReadAllText(textAsset.text);
            return quiz;
        }
        
        private static Dictionary<StationEnum, Station> loadStations(string file = AppConstants.StationsInfoDataFile)
        {
            if (AppUtils.stations != null)
            {
                AppUtils.stations.Clear();
            }

            //string path = Path.Combine(Application.dataPath, file);
            //if (!File.Exists(path))
            //{
            //    Debug.Log($"Data file not found at path='{path}'");
            //    return new Dictionary<StationEnum, Station>();
            //}

            TextAsset textAsset = Resources.Load<TextAsset>(file);
            //Debug.Log(textAsset != null);

            //string json = File.ReadAllText(textAsset.text);
            StationInfoList data = JsonUtility.FromJson<StationInfoList>(textAsset.text);
            //Debug.Log(json);

            if (AppUtils.waypoints == null)
            {
                AppUtils.getWaypoints();
            }

            AppUtils.stations = new Dictionary<StationEnum, Station>();
            foreach (var info in data.stationInfoList)
            {
                int waypointId = info.stationId * 100;
                StationEnum stationId = (StationEnum)info.stationId;
                if (!AppUtils.waypointsDic.ContainsKey(waypointId))
                {
                    Debug.Log($"Waypoints do not have station with id='{stationId}'");
                    continue;
                }
                else if (AppUtils.stations.ContainsKey(stationId))
                {
                    Debug.Log($"Already loaded station with id='{stationId}");
                    continue;
                }

                var wp = AppUtils.waypointsDic[waypointId];
                Station station = new Station(info, wp);

                wp.WaypointObject.transform.parent.GetComponent<StationBehaviour>().SetStation(station);

                AppUtils.stations.Add(station.StationId, station);
            }


            return AppUtils.stations;
        }

        private static void saveDefaultStationsInfo(string file)
        {
            List<StationInfo> lst = new List<StationInfo>();
            //foreach (var kv in stations)
            //{
            //    lst.Add(new StationInfo(((int)kv.Key), $"Station {((int)kv.Key)}"));
            //}
            Quiz quiz = new Quiz();
            QuizQuestion question;
            for (int i=1; i<=5; i++)
            {
                question = new QuizQuestion(i, $"Question {i}", $"Question {i}");
                for (int j=1; j<=5; j++)
                {
                    question.answers.Add($"Answer {j}");
                }
                question.correctAnswerIndex = i-1;
                quiz.questions.Add(question);
            }
            for (int i=1; i<=4; i++)
            {
                lst.Add(new StationInfo(i, $"Station {i} description", quiz));
            }
            var data = new StationInfoList(lst);
            string json = JsonUtility.ToJson(data, true);

            string path = Path.Combine(Application.dataPath, file);
            File.WriteAllText(path, json);
            Debug.Log($"Saved data to file='{path}'");
        }

        public static void ShowToast(string message, bool isBadgeToast = false)
        {
            UnityContext.Instance.Context.Post(_ =>
            {
                ToastPanelManager.Instance.ShowToast(message, isBadgeToast);
            }, null);
        }
        
        public static string ConvertDigitsToLatin(string s)
        {
            return s
                .Replace('\u06F0', '0')
                .Replace('\u06F1', '1')
                .Replace('\u06F2', '2')
                .Replace('\u06F3', '3')
                .Replace('\u06F4', '4')
                .Replace('\u06F5', '5')
                .Replace('\u06F6', '6')
                .Replace('\u06F7', '7')
                .Replace('\u06F8', '8')
                .Replace('\u06F9', '9');
        }
    }
    
}
