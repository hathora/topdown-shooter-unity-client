using UnityEngine;

public class Game : MonoBehaviour {

    Hathora.Client hathoraClient;

    bool hasLoaded = false;

    private async void Awake() {
        hathoraClient = Hathora.Client.GetInstance();

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
