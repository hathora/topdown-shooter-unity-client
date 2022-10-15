using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

using Cinemachine;

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


    [Header("Game Over")]

    [SerializeField]
    string menuScene = "Menu";

    [SerializeField]
    GameObject backToMenuUI;


    [Header("Misc")]

    [SerializeField]
    CinemachineVirtualCamera cameraFollow;

    //
    // \Config

    Hathora.Client hathoraClient;

    string currentPlayerId;
    Dictionary<string, Player> playersMap;
    HashSet<string> currentPlayers;

    Dictionary<string, Bullet> bulletsMap;
    HashSet<string> currentBullets;

    float TILE_SIZE;

    bool hasLoaded = false;
    bool hasQuit   = false;

    private async void Awake() {
        hathoraClient = Hathora.Client.GetInstance();
        playersMap = new Dictionary<string, Player>();
        currentPlayers = new HashSet<string>();

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

        TILE_SIZE = MapData.TILE_SIZE;
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
            HashSet<string> nextPlayerList = new HashSet<string>();
            foreach(PlayerData playerData in players) {

                playerData.position = ConvertPosition(playerData.position);

                string id = playerData.id;

                if (playersMap.ContainsKey(id)) {
                    Player player = playersMap[id];
                    player.Render(playerData);
                    currentPlayers.Remove(id);

                    // Debug.Log("RENDERED: " + id + "(" + playerData.position.x + ", " + playerData.position.y + ")");

                } else {
                    Position position = playerData.position;
                    Vector3 spawnPosition = new(position.x, position.y, 0);

                    bool isCurrentPlayer = (id == currentPlayerId);
                    GameObject prefab = isCurrentPlayer ? playerPrefab : robotPrefab;

                    // Debug.Log(string.Format("Spawn {0}: ({1}, {2})", isCurrentPlayer ? "Player" : "Robot", spawnPosition.x, spawnPosition.y));

                    // Spawn
                    GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity, parentTransform);
                    Player player = go.GetComponent<Player>();
                    player.Init(playerData, isCurrentPlayer);
                    if (isCurrentPlayer) {
                        cameraFollow.Follow = go.transform;
                    }

                    playersMap.Add(id, player);
                }

                nextPlayerList.Add(id);
            }

            // Remove Players that don't exist in props
            foreach(string id in currentPlayers) {
                if (playersMap.ContainsKey(id)) {
                    Player player = playersMap[id];
                    playersMap.Remove(id);

                    player.PlayDeathAnimationAndRemove();

                    if (id == currentPlayerId) {
                        DisplayGameOverMenu();
                    }
                }
            }
            currentPlayers = nextPlayerList;


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

    private void DisplayGameOverMenu() {
        if (backToMenuUI) {
            backToMenuUI.SetActive(true);
        }
    }

    private async void OnApplicationQuit() {
        hasQuit = true;
        await hathoraClient.Disconnect();
        Debug.Log("Disconnected.");
    }

    Position ConvertPosition(Position position) {
        Position converted = new Position(0.0f, 0.0f);

        converted.x = position.x / TILE_SIZE;
        converted.y = position.y / TILE_SIZE * -1;

        return converted;
    }


    // Public Methods
    //

    public void CopyLobbyId() {
        string roomId = hathoraClient.GetRoomId();
        roomId.CopyToClipboard();
    }

    public void GoBackToMenu() {
        OnApplicationQuit();
        SceneManager.LoadScene(menuScene);
    }
}
