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

        //
        // \Config

        private static Client self;

        private string roomId = "";
        private string token = "";
        private string userId = "";

        HttpClient httpClient;
        ClientWebSocket ws;


        private async void Awake() {

            // Make sure there is only ever one client in the scene
            if (!self) {
                self = this;
                DontDestroyOnLoad(gameObject);

                httpClient = new HttpClient();
                ws = new ClientWebSocket();

                await GetToken();

                Debug.Log("HATHORA: Got Token: " + token);

            } else {
                Destroy(gameObject);
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
            await GetToken();

            var createRequest = new HttpRequestMessage(HttpMethod.Post, $"https://coordinator.hathora.dev/{appId}/create");

            createRequest.Content = new ByteArrayContent(new byte[] { });
            createRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            createRequest.Headers.Add("Authorization", token);

            var createResponse = await httpClient.SendAsync(createRequest);

            roomId = JsonUtility.FromJson<CreateResponse>(await createResponse.Content.ReadAsStringAsync()).stateId;

            Debug.Log("Created Room ID: " + roomId);
        }

        public void JoinGame(string roomId) {
            GetToken();
            this.roomId = roomId;
        }

        public string GetRoomId() {
            return roomId;
        }

        public async Task Connect(Action<string> contentRenderer) {
            // Ensure that there is a room before attempting to connect to it
            if (roomId == "") {
                Debug.LogWarning("Attempted to connect without lobby. Creating...");

                if (defaultRoomId != "") {
                    JoinGame(defaultRoomId);

                } else {
                    await CreateNewGame();
                }
            }

            await ws.ConnectAsync(new Uri($"wss://coordinator.hathora.dev/connect/{appId}"), CancellationToken.None);
            var bytesToSend = Encoding.UTF8.GetBytes($"{{\"token\": \"{token}\", \"stateId\": \"{roomId}\"}}");
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);

            string json = JWT.Decode(token);
            Token decoded = JsonUtility.FromJson<Token>(json);
            userId = decoded.id;

            Debug.Log("USER ID: " + userId);

            while (ws.State == WebSocketState.Open) {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                string content = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

                contentRenderer(content);
            }
        }

        public async void Send(ClientMessage message) {
            await ws.SendAsync(Encoding.UTF8.GetBytes(message.ToJson()), WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public string GetUserId() {
            return userId;
        }
    }

}