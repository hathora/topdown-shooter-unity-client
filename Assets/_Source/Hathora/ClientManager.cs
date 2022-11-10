using UnityEngine;

using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Hathora;

using DataTypes.Network;

namespace Hathora
{

    public class ClientManager : MonoBehaviour
    {

        // Config
        //

        [SerializeField]
        string appId = "";

        [SerializeField]
        string coordinatorHost = "coordinator.hathora.dev";

        [Header("Debug")]

        [SerializeField]
        string defaultRoomId = "3bm0m63cfic5m";

        [SerializeField]
        bool printDebugLogs = false;

        //
        // \Config

        private static ClientManager self;

        private string roomId = "";
        private string token = "";
        private string userId = "";

        private Hathora.Client hathoraClient;

        private ClientWebSocket ws;

        private void DebugLog(string message)
        {
            if (printDebugLogs)
            {
                Debug.Log(message);
            }
        }

        private void DebugWarn(string message)
        {
            if (printDebugLogs)
            {
                Debug.LogWarning(message);
            }
        }

        private async void Awake()
        {
            DebugLog("HATHORA - AWAKE");
            hathoraClient = new Hathora.Client(appId, coordinatorHost);
            await Initialize();
        }

        private async Task Initialize()
        {
            if (!self)
            {
                self = this;
                DontDestroyOnLoad(gameObject);
                this.token = await hathoraClient.LoginAnonymous();
                this.userId = Hathora.Client.GetUserFromToken(token);
            }
            else
            {
                DebugLog("Duplicate...destroying self...");
                Destroy(gameObject);
            }
        }

        // Public Methods
        //

        public static ClientManager GetInstance()
        {
            return self;
        }

        public async Task CreateNewGame()
        {
            this.roomId = await hathoraClient.Create(token, new byte[] { });
            DebugLog("Created Room ID: " + roomId);
        }

        public void JoinGame(string roomId)
        {
            this.roomId = roomId;
        }

        public string GetRoomId()
        {
            return roomId;
        }

        public async Task Connect(Action<string> contentRenderer)
        {
            // Ensure that there is a room before attempting to connect to it
            if (roomId == "")
            {
                DebugWarn("Attempted to connect without lobby. Creating...");

                if (defaultRoomId != "")
                {
                    JoinGame(defaultRoomId);
                }
                else
                {
                    await CreateNewGame();
                }
            }
            else
            {
                DebugLog("Already got roomId: " + roomId);
            }

            ws = await hathoraClient.Connect(token, roomId);

            userId = Hathora.Client.GetUserFromToken(token);

            DebugLog("USER ID: " + userId);

            while (ws.State == WebSocketState.Open)
            {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                string content = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

                contentRenderer(content);
            }
        }

        public async Task Disconnect()
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            ws = new ClientWebSocket();
            DebugLog("Disconnected");
        }

        public async void Send(ClientMessage message)
        {
            if (ws.State == WebSocketState.Open)
            {
                DebugLog("SEND: " + message.ToJson());
                await ws.SendAsync(Encoding.UTF8.GetBytes(message.ToJson()), WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }

        public string GetUserId()
        {
            return userId;
        }
    }

}
