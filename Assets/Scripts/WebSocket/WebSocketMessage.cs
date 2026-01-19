using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using ProjectClasses;
using ProjectEnums;

namespace WebSocket
{
    [Serializable]
    public class WebSocketMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public WebSocketTopics topic;
        
        public JObject message;
        
    }
}