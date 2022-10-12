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

    Hathora.Client hathoraClient;

    void Awake() {
        hathoraClient = Hathora.Client.GetInstance();
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

    public async void JoinGame() {
        string roomId = roomIdField.text;

        if (roomId != "") {
            Debug.Log("JOIN: " + roomIdField.text);
            await hathoraClient.JoinGame(roomId);
            GoToGameScene();

        } else {
            Debug.LogError("Can't join. No Room ID specified!");
        }
    }
}
