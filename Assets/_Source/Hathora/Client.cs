using UnityEngine;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Hathora {

    public class Client: MonoBehaviour {

        // Config
        //

        [SerializeField]
        string appId = "";

        //
        // \Config

        private static Client self;

        private string roomId;
        private string token;

        HttpClient httpClient;
        ClientWebSocket ws;


        private async void Awake() {

            // Make sure there is only ever one client in the scene
            if (!self) {
                self = this;
                DontDestroyOnLoad(gameObject);

                httpClient = new HttpClient();
                ws = new ClientWebSocket();

                var loginResponse = await httpClient.PostAsync($"https://coordinator.hathora.dev/{appId}/login/anonymous", null);
                token = JsonUtility.FromJson<LoginResponse>(await loginResponse.Content.ReadAsStringAsync()).token;

                Debug.Log("HATHORA: Got Token: " + token);

            } else {
                Destroy(gameObject);
            }
        }

        // Public Methods
        //

        // Public Methods
        //

        public static Client GetInstance() {
            return self;
        }

        public async Task CreateNewGame() {
            var createRequest = new HttpRequestMessage(HttpMethod.Post, $"https://coordinator.hathora.dev/{appId}/create");

            createRequest.Content = new ByteArrayContent(new byte[] { });
            createRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            createRequest.Headers.Add("Authorization", token);

            var createResponse = await httpClient.SendAsync(createRequest);

            roomId = JsonUtility.FromJson<CreateResponse>(await createResponse.Content.ReadAsStringAsync()).stateId;

            Debug.Log("Created Room ID: " + roomId);
        }

        public void JoinGame(string roomId) {
            this.roomId = roomId;
        }

        public string GetRoomId() {
            return roomId;
        }

        public async Task Connect(Action<string> contentRenderer) {
            await ws.ConnectAsync(new Uri($"wss://coordinator.hathora.dev/connect/{appId}"), CancellationToken.None);
            var bytesToSend = Encoding.UTF8.GetBytes($"{{\"token\": \"{token}\", \"stateId\": \"{roomId}\"}}");
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);

            while (ws.State == WebSocketState.Open) {
                ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
                WebSocketReceiveResult result = await ws.ReceiveAsync(bytesReceived, CancellationToken.None);
                string content = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

                contentRenderer(content);
            }
        }

        public async void Send(string message) {
            await ws.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }

}