using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections.Generic;

using DataTypes.Game;
using DataTypes.Network;

public class Game : MonoBehaviour {

    // Config
    //

    [Header("Characters")]

    [SerializeField]
    GameObject playerPrefab;

    [SerializeField]
    GameObject robotPrefab;

    [SerializeField]
    Transform parentTransform;


    [Header("Map")]

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    MapRenderer mapRenderer;

    [SerializeField]
    TextAsset mapDataFile;


    [Header("Lobby ID")]

    [SerializeField]
    TMPro.TMP_Text lobbyTextField;

    [SerializeField]
    string lobbyTextPrefix = "";


    [Header("Misc")]

    [SerializeField]
    Follow cameraFollow;

    //
    // \Config

    Hathora.Client hathoraClient;

    string currentPlayerId;
    Dictionary<string, Player> playersMap;

    bool hasLoaded = false;

    private async void Awake() {
        hathoraClient = Hathora.Client.GetInstance();
        playersMap = new Dictionary<string, Player>();

        // Render Map
        //
        MapData mapData = MapData.Parse(mapDataFile.ToString());

        mapRenderer.Render(mapData);


        // Hathora Client will periodically call RenderContent
        // as long as the web socket connection is open
        //
        await hathoraClient.Connect(RenderContent);
    }

    private void DrawLobbyId() {
        string roomId = hathoraClient.GetRoomId();

        // Draw Lobby ID
        //
        if (lobbyTextField) {
            lobbyTextField.text = lobbyTextPrefix + roomId;
        }
    }

    private void RenderContent(string contentData) {
        if (!hasLoaded) {
            hasLoaded = true;
            currentPlayerId = hathoraClient.GetUserId();
            DrawLobbyId();
            Debug.Log("Connected.");
        }

        ServerMessage message = JsonUtility.FromJson<ServerMessage>(contentData);
        if (message != null) {
            GameState state = message.state;
            // Debug.Log("STATE: " + JsonUtility.ToJson(state));

            PlayerData[] players = state.players;
            BulletData[] bullets = state.bullets;

            foreach(PlayerData playerData in players) {

                if (playersMap.ContainsKey(playerData.id)) {
                    Player player = playersMap[playerData.id];
                    player.Render(playerData);

                } else {
                    Position position = playerData.position;
                    Vector3 spawnPosition = ConvertPosition(position);

                    bool isCurrentPlayer = (playerData.id == currentPlayerId);
                    GameObject prefab = isCurrentPlayer ? playerPrefab : robotPrefab;

                    Debug.Log(string.Format("Spawn {0}: ({1}, {2})", isCurrentPlayer ? "Player" : "Robot", spawnPosition.x, spawnPosition.y));

                    // Spawn
                    GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity, parentTransform);
                    Player player = go.GetComponent<Player>();
                    player.Init(playerData, isCurrentPlayer);
                    if (isCurrentPlayer) {
                        cameraFollow.FollowTarget(go.transform);
                    }

                    playersMap.Add(playerData.id, player);
                }
            }
        }
    }

    Vector3 ConvertPosition(Position pos) {
        // return Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 0));
        return new Vector3(pos.x / 64, pos.y / 64, 0);
    }
}
