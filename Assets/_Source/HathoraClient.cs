using UnityEngine;
using UnityEngine.SceneManagement;

public class HathoraClient : MonoBehaviour {

    // Config
    //

    [SerializeField]
    string appId = "";

    //
    // \Config

    private static HathoraClient self;

    private string roomId;
    private string token;


    private void Awake() {

        // Make sure there is only ever one client in the scene
        if (!self) {
            self = this;
            DontDestroyOnLoad(gameObject);

        } else {
            Destroy(gameObject);
        }
    }

    // Public Methods
    //

    public void CreateNewGame() {

    }

    public void JoinGame() {

    }

    public static HathoraClient GetInstance() {
        return self;
    }
}
