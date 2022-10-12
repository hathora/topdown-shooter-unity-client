using UnityEngine;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using DataTypes.Network;

namespace Hathora {

    public class Client: MonoBehaviour {

        // Config
        //

        [SerializeField]
        string appId = "";

        [Header("Debug")]

        [SerializeField]
        string defaultRoomId = "3bm0m63cfic5m";

        [SerializeField]
        bool printDebugLogs = false;

        //
        // \Config

        private static Client self;

        private string roomId = "";
        private string token = "";
        private string userId = "";

        HttpClient httpClient;
        ClientWebSocket ws;

        private bool hasInitialized = false;
        private Task initTask = null;

        private void DebugLog(string message) {
            if (printDebugLogs) {
                Debug.Log(message);
            }
        }

        private void DebugWarn(string message) {
            if (printDebugLogs) {
                Debug.LogWarning(message);
            }
        }

        private async void Awake() {
            DebugLog("HATHORA - AWAKE");
            await Initialize();
        }

        private async Task Initialize() {
            DebugLog("INIT?");
            if (!hasInitialized) {
                if (initTask != null) {
                    DebugLog("initTask exists");
                    if (!initTask.IsCompleted) {
                        DebugLog("Waiting for initTask...");
                        await initTask;
                    } else {
                        DebugLog("xxx INIT - WIERD STATE");
                    }
                } else {
                    DebugLog("Initializing...");

                    // Make sure there is only ever one client in the scene
                    if (!self) {
                        self = this;
                        DontDestroyOnLoad(gameObject);

                        httpClient = new HttpClient();
                        ws = new ClientWebSocket();

                        initTask = GetToken();
                        await initTask;

                        DebugLog("HATHORA: Got Token: " + token);
                        hasInitialized = true;

                    } else {
                        DebugLog("Duplicate...destroying self...");
                        Destroy(gameObject);
                    }
                    DebugLog("Done");
                }
            } else {
                DebugLog("Already Initialized");
            }
        }

        private async Task GetToken() {
            if (token == "") {
                var loginResponse = await httpClient.PostAsync($"https://coordinator.hathora.dev/{appId}/login/anonymous", null);
                token = JsonUtility.FromJson<LoginResponse>(await loginResponse.Content.ReadAsStringAsync()).token;
            }
        }

        // Public Methods
        //

        public static Client GetInstance() {
            return self;
        }

        public async Task CreateNewGame() {
            await Initialize();

            var createRequest = new HttpRequestMessage(HttpMethod.Post, $"https://coordinator.hathora.dev/{appId}/create");

            createRequest.Content = new ByteArrayContent(new byte[] { });
            createRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            createRequest.Headers.Add("Authorization", token);

            var createResponse = await httpClient.SendAsync(createRequest);

            roomId = JsonUtility.FromJson<CreateResponse>(await createResponse.Content.ReadAsStringAsync()).stateId;

            DebugLog("Created Room ID: " + roomId);
        }

        public async Task JoinGame(string roomId) {
            await Initialize();
            this.roomId = roomId;
        }

        public string GetRoomId() {
            return roomId;
        }

        public async Task Connect(Action<string> contentRenderer) {
            // Ensure that there is a room before attempting to connect to it
            if (roomId == "") {
                DebugWarn("Attempted to connect without lobby. Creating...");

                if (defaultRoomId != "") {
                    await JoinGame(defaultRoomId);

                } else {
                    await CreateNewGame();
                }
            } else {
                DebugLog("Already got roomId: " + roomId);
            }

            await ws.ConnectAsync(new Uri($"wss://coordinator.hathora.dev/connect/{appId}"), CancellationToken.None);
            var bytesToSend = Encoding.UTF8.GetBytes($"{{\"token\": \"{token}\", \"stateId\": \"{roomId}\"}}");
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);

            string json = JWT.Decode(token);
            DebugLog("JSON: " + json);
            Token decoded = JsonUtility.FromJson<Token>(json);
            userId = decoded.id;

            DebugLog("USER ID: " + userId);

            while (ws.State == WebSocketState.Open) {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                string content = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

                contentRenderer(content);
            }
        }

        public async Task Disconnect() {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "quit", CancellationToken.None);
            DebugLog("Disconnected");
        }

        public async void Send(ClientMessage message) {
            await ws.SendAsync(Encoding.UTF8.GetBytes(message.ToJson()), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public string GetUserId() {
            return userId;
        }
    }

}