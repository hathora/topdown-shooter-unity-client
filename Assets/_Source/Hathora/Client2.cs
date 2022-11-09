using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hathora
{
    public class Client2
    {
        private readonly string appId;
        private readonly string coordinatorHost;
        private readonly HttpClient httpClient;


        public Client2(string appId, string coordinatorHost)
        {
            this.appId = appId;
            this.coordinatorHost = coordinatorHost;
            this.httpClient = new HttpClient();
        }

        public Client2(string appId)
        {
            this.appId = appId;
            this.coordinatorHost = "coordinator.hathora.dev";
            this.httpClient = new HttpClient();
        }

        class LoginResponse
        {
            public string token;
        }

        public async Task<string> loginAnonymous()
        {
            HttpResponseMessage loginResponse = await httpClient.PostAsync($"{coordinatorHost}/{appId}/login/anonymous", null);
            string loginBody = await loginResponse.Content.ReadAsStringAsync();
            LoginResponse login = JsonConvert.DeserializeObject<LoginResponse>(loginBody);
            return login.token;
        }

        class CreateResponse
        {
            public string stateId;
        }

        public async Task<string> create(string token, byte[] body)
        {
            HttpRequestMessage createRequest = new HttpRequestMessage(HttpMethod.Post, $"{coordinatorHost}/{appId}/create");
            createRequest.Content = new ByteArrayContent(body);
            createRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            createRequest.Headers.Add("Authorization", token);
            HttpResponseMessage createResponse = await httpClient.SendAsync(createRequest);
            string createBody = await createResponse.Content.ReadAsStringAsync();
            CreateResponse create = JsonConvert.DeserializeObject<CreateResponse>(createBody);
            return create.stateId;
        }

        public async Task<ClientWebSocket> connect(string token, string stateId)
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri($"wss://{coordinatorHost}/connect/{appId}"), CancellationToken.None);
            var bytesToSend = Encoding.UTF8.GetBytes($"{{\"token\": \"{token}\", \"stateId\": \"{stateId}\"}}");
            await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Binary, true, CancellationToken.None);
            return webSocket;
        }
    }
}