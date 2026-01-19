using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectClasses;
using ProjectEnums;
using RTLTMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace WebSocket
{
    public class WebSocketClient : MonoBehaviour
    {
        public RTLTextMeshPro connectionStatusLabel; 
    
        private ClientWebSocket _ws;

        private CancellationTokenSource _reconnectionCts;
    
        private SystemManager _systemManager;

        private SynchronizationContext _unityContext;
    
        private ConcurrentQueue<WebSocketMessage> messageQueue;
        
        private bool IsEnglish =>
            LocalizationSettings.SelectedLocale.Identifier.Code == "en";
        
        public UnityEngine.Localization.Components.LocalizeStringEvent connectionStatusLocalized;

        void Start()
        {
            messageQueue = new ConcurrentQueue<WebSocketMessage>();
        
            _unityContext = SynchronizationContext.Current;
            _systemManager = AppUtils.GetSystemManager();
        
            _reconnectionCts = new CancellationTokenSource();
            var waitTimeMs = AppConstants.ReconnectionWaitMs;
            _ = Task.Run(async () =>
            {
                while (!_reconnectionCts.Token.IsCancellationRequested)
                {
                    await SetupConnection();
                    Debug.LogWarning($"Waiting {((float)(waitTimeMs))/1000:F2}ms before reconnection attempt.");
                    await Task.Delay(waitTimeMs);
                }
                Debug.Log("Reconnection attempts stopped.");
            });
        }

        void Update()
        {
            if (messageQueue.TryDequeue(out var msg))
            {
                // Debug.Log($"topic: {msg.topic}, message: {msg.message}");
                if (msg.topic == WebSocketTopics.system_status){
                    // Debug.Log("WebSocket :: handleStatusMessage start: ");
                    handleStatusMessage(msg.message.ToObject<SystemStatus>());
                }
                else if (msg.topic == WebSocketTopics.system_reset)
                {
                    // Debug.Log("WebSocket :: handleResetMessage start: ");
                    handleResetMessage();
                }
                else
                {
                    Debug.Log("WebSocket :: Received invalid message: " + msg.topic);
                }
            }
        }

        public bool IsConnected()
        {
            return _ws != null && _ws.State == WebSocketState.Open;
        }

        private void UpdateConnectionLabel(bool isConnected)
        {
            if (connectionStatusLocalized == null || connectionStatusLabel == null) return;

            _unityContext.Post(_ =>
            {
                connectionStatusLocalized.StringReference.SetReference(
                    "LocalizationStringTableCollection", 
                    isConnected ? "connected" : "disconnected"
                );
        
                connectionStatusLocalized.RefreshString();
                connectionStatusLabel.color = isConnected ? Color.green : Color.red;
            }, null);
        }

        private async Task SetupConnection()
        {
            _unityContext.Post(_ =>
            {
                connectionStatusLocalized.StringReference.SetReference("LocalizationStringTableCollection", "loading");
                connectionStatusLabel.color = Color.white;
            }, null);
            
            if (IsConnected())
            {
                Debug.Log("WebSocket :: closing open connection");
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _ws?.Dispose();
            }
        
            _ws = new ClientWebSocket();

            try
            {
                var serverUri = new Uri(AppConstants.IsLocalServer ? 
                    $"ws://{AppConstants.ServerLocalAddress}" 
                    : $"wss://{AppConstants.ServerDeploymentAddress}");
            
                Debug.Log("WebSocket trying to connect to uri: " + serverUri);
                var connectionCts = new CancellationTokenSource();
                connectionCts.CancelAfter(1000);
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(0.5)))
                {
                    await _ws.ConnectAsync(serverUri, cts.Token);
                }

                Debug.Log("WebSocket connection successfully established");
                
                await subscribeToTopics();

                UpdateConnectionLabel(true);

                await ReceiveLoop();
            }
            catch (Exception ex)
            {
                Debug.LogError("WebSocket connection failed: " + ex.Message);
            }
        
            messageQueue.Clear();
            UpdateConnectionLabel(false);
        }
    
        private async Task ReceiveLoop()
        {
            var buffer = new byte[1024 * 4];
            var cts = new CancellationTokenSource();

            while (
                IsConnected() && 
                !cts.Token.IsCancellationRequested && 
                !_reconnectionCts.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close && !_reconnectionCts.Token.IsCancellationRequested)
                    {
                        Debug.Log("WebSocket closed by server");
                        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        cts.Cancel();
                    }
                    else
                    {
                        string msgStr = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        // var msgDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(msgStr);
                        var msg = JsonConvert.DeserializeObject<WebSocketMessage>(msgStr);
                        // Debug.Log($"Received message: {msgStr}");
                        messageQueue.Enqueue(msg);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("WebSocket ReceiveLoop error: " + ex);
                }
            }
            cts.Cancel();
        }

        private async void OnApplicationQuit()
        {
            _reconnectionCts.Cancel();
            if (_ws != null && _ws.State == WebSocketState.Open)
            {
                await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application quitting", CancellationToken.None);
            }
        }

        private void handleStatusMessage(SystemStatus status)
        {
            if (status == null)
            {
                Debug.LogWarning("handleStatusMessage :: Received null SystemStatus");
                return;
            }
            
            _unityContext.Post(_ =>
            {
                foreach (var cartInfo in status.carts)
                {
                    _systemManager.UpdateCartSpeed(cartInfo.cart_id, cartInfo.speed);
                }
            
                foreach (var entry in status.stationOccupants)
                {
                    // Debug.Log($"stationId: {entry.Key}, occupancyNew: {entry.Value.newCart}, occupancyOld: {entry.Value.oldCart}");
                    _systemManager.UpdateStationOccupent(entry.Key, entry.Value);
                }
            }, null);
        }

        private void handleResetMessage()
        {
            _unityContext.Post(_ =>
            {
                _systemManager.ResetSystem();
            }, null);
        }

        private async Task subscribeToTopics()
        {
            foreach (var topic in Enum.GetNames(typeof(WebSocketTopics)))
            {
                var topicBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(topic));
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(0.5)))
                {
                    await _ws.SendAsync(topicBytes, WebSocketMessageType.Text, endOfMessage: true,
                        cancellationToken: cts.Token);
                }
            }
            Debug.Log("Sent topics to WebSocket");
        }
    }
}