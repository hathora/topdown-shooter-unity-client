using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour {

    // Config
    //

    [SerializeField]
    string gameScene;

    [SerializeField]
    TMP_InputField roomIdField;

    [Header("Debug")]

    [SerializeField]
    string defaultRoomId = "";

    //
    // \Config

    Hathora.ClientManager hathoraClient;

    void Awake() {
        hathoraClient = Hathora.ClientManager.GetInstance();
        roomIdField.text = defaultRoomId;
    }

    private void GoToGameScene() {
        SceneManager.LoadScene(gameScene);
    }

    // Public Methods
    //

    public async void CreateNewGame() {
        await hathoraClient.CreateNewGame();
        GoToGameScene();
    }

    public void JoinGame() {
        string roomId = roomIdField.text;

        if (roomId != "") {
            Debug.Log("JOIN: " + roomIdField.text);
            hathoraClient.JoinGame(roomId);
            GoToGameScene();

        } else {
            Debug.LogError("Can't join. No Room ID specified!");
        }
    }
}
