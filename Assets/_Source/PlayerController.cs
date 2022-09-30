using UnityEngine;

public class PlayerController : MonoBehaviour {

    Player self;
    bool isEnabled = false;
    Hathora.Client hathoraClient;

    private void Awake() {
        hathoraClient = Hathora.Client.GetInstance();
    }

    void Update() {
        if (isEnabled) {
            // TODO: Handle input
            // Send new position to client
            // hathoraClient.Send(someJsonString);
        }
    }


    // Public Methods
    //

    public void SetEnabled(Player data) {
        isEnabled = true;
        self = data;
    }    
}
