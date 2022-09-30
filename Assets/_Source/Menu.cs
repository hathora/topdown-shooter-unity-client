using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour {

    // Config
    //

    [SerializeField]
    string gameScene;

    [SerializeField]
    TMP_InputField roomIdField;

    //
    // \Config

    HathoraClient hathoraClient;

    void Awake() {
        hathoraClient = HathoraClient.GetInstance();
    }

    private void GoToGameScene() {
        SceneManager.LoadScene(gameScene);
    }

    // Public Methods
    //

    public void CreateNewGame() {
        Debug.Log("CREATE");
        GoToGameScene();
    }

    public void JoinGame() {
        string roomId = roomIdField.text;

        if (roomId != "") {
            Debug.Log("JOIN: " + roomIdField.text);
            GoToGameScene();
        }
    }
}