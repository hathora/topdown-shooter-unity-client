using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections.Generic;

using DataTypes.Game;
using DataTypes.Network;

public class Game : MonoBehaviour {

    // Config
    //

    [Header("Prefabs")]

    [SerializeField]
    GameObject playerPrefab;

    [SerializeField]
    GameObject robotPrefab;

    [SerializeField]
    GameObject bulletPrefab;

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
    Dictionary<string, Bullet> bulletsMap;
    HashSet<string> currentBullets;

    float HEIGHT_MULTIPLIER;
    float WIDTH_MULTIPLIER;
    float TILE_SIZE;

    bool hasLoaded = false;
    bool hasQuit   = false;

    private async void Awake() {
        hathoraClient = Hathora.Client.GetInstance();
        playersMap = new Dictionary<string, Player>();
        bulletsMap = new Dictionary<string, Bullet>();
        currentBullets = new HashSet<string>();

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

    // Render Map
    //
    private void DrawMap(TextAsset _mapDataFile) {
        MapData mapData = MapData.Parse(_mapDataFile.ToString());
        mapRenderer.Render(mapData);

        HEIGHT_MULTIPLIER = MapData.HEIGHT_MULTIPLIER;
        WIDTH_MULTIPLIER  = MapData.WIDTH_MULTIPLIER;
        TILE_SIZE         = MapData.TILE_SIZE;
    }

    private void RenderContent(string contentData) {
        if (!hasLoaded) {
            hasLoaded = true;
            currentPlayerId = hathoraClient.GetUserId();
            DrawLobbyId();

            // Can source mapDataFile from server too
            DrawMap(mapDataFile);

            Debug.Log("Connected.");
        }

        // Game has quit async, don't try to access any member variables
        // because they may not exist. Handle any cleanup in OnApplicationQuit
        //
        if (hasQuit) {
            return;
        }

        ServerMessage message = JsonUtility.FromJson<ServerMessage>(contentData);
        if (message != null) {
            GameState state = message.state;
            // Debug.Log("STATE: " + JsonUtility.ToJson(state));

            // Draw Players
            //
            PlayerData[] players = state.players;
            foreach(PlayerData playerData in players) {

                playerData.position = ConvertPosition(playerData.position);

                if (playersMap.ContainsKey(playerData.id)) {
                    Player player = playersMap[playerData.id];
                    player.Render(playerData);
                    // Debug.Log("RENDERED: " + playerData.id + "(" + playerData.position.x + ", " + playerData.position.y + ")");

                } else {
                    Position position = playerData.position;
                    Vector3 spawnPosition = new(position.x, position.y, 0);

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

            // Draw Bullets
            //
            BulletData[] bullets = state.bullets;
            HashSet<string> nextBullets = new HashSet<string>();

            for(int i = 0; i < bullets.Length; i++) {
                BulletData bulletData = bullets[i];
                bulletData.position = ConvertPosition(bulletData.position);

                string id = bulletData.id;

                if (bulletsMap.ContainsKey(id)) {
                    Bullet bullet = bulletsMap[id];
                    bullet.Render(bulletData);
                    currentBullets.Remove(id);

                } else {
                    Position position = bulletData.position;
                    Vector3 spawnPosition = new(position.x, position.y, 0);

                    // Spawn
                    GameObject go = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity, parentTransform);
                    Bullet bullet = go.GetComponent<Bullet>();
                    bullet.Init(bulletData);

                    bulletsMap.Add(bulletData.id, bullet);
                }
                nextBullets.Add(id);
            }

            // Remove bullets that don't exist in props
            foreach(string id in currentBullets) {
                if (bulletsMap.ContainsKey(id)) {
                    Bullet bullet = bulletsMap[id];
                    bulletsMap.Remove(id);

                    Destroy(bullet.gameObject);
                }
            }

            currentBullets = nextBullets;
        }
    }

    private async void OnApplicationQuit() {
        hasQuit = true;
        await hathoraClient.Disconnect();
        Debug.Log("Disconnected.");
    }

    Position ConvertPosition(Position position) {
        Position converted = new Position(0.0f, 0.0f);

        converted.x = (position.x * WIDTH_MULTIPLIER)  / 67 - 0.667f;
        converted.y = (position.y * HEIGHT_MULTIPLIER) / 67 * -1;

        return converted;
    }
}
