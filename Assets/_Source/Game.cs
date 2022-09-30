using UnityEngine;
using UnityEngine.Tilemaps;

using Hathora.DataTypes;

public class Game : MonoBehaviour {

    // Config
    //

    [Header("Characters")]

    [SerializeField]
    GameObject playerPrefab;

    [SerializeField]
    GameObject robotPrefab;

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

    //
    // \Config


    Hathora.Client hathoraClient;

    bool hasLoaded = false;

    private async void Awake() {
        hathoraClient = Hathora.Client.GetInstance();
        string roomId = hathoraClient.GetRoomId();

        // Draw Lobby ID
        //
        if (lobbyTextField) {
            lobbyTextField.text = lobbyTextPrefix + roomId;
        }


        // Render Map
        //
        MapData mapData = MapData.Parse(mapDataFile.ToString());

        mapRenderer.Render(mapData);


        // Place Camera
        //
        //int middleX = (mapData.left + mapData.right) / 2;
        //int middleY = (mapData.top + mapData.bottom) / 2;
        //Camera.main.transform.position = new Vector3(middleX, middleY);


        // Hathora Client will periodically call RenderContent
        // as long as the web socket connection is open
        //
        await hathoraClient.Connect(RenderContent);
    }

    private void RenderContent(string content) {
        if (!hasLoaded) {
            hasLoaded = true;
            Debug.Log("Connected.");
        }

        //Debug.Log("RENDER: " + content);
    }

    void Update() {
        if (hasLoaded) {
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log("Click");
            }
        }
    }
}
