using UnityEngine;
using UnityEngine.Tilemaps;

public class Game : MonoBehaviour {

    // Config
    //

    [SerializeField]
    GameObject playerPrefab;

    [SerializeField]
    GameObject robotPrefab;

    [SerializeField]
    Tilemap tilemap;

    [SerializeField]
    MapRenderer mapRenderer;


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

        if (lobbyTextField) {
            lobbyTextField.text = lobbyTextPrefix + roomId;
        }

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
