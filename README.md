# Hathora Topdown Shooter - Unity

## Overview

Multiplayer 2D shooter demo made using [Hathora](https://hathora.dev/) + Unity

Assets from [Kenney](https://kenney.nl/assets/topdown-shooter)

Server and Phaser client available [here](https://github.com/hathora/topdown-shooter)

![lobby](https://user-images.githubusercontent.com/587136/195953691-c990baa5-5c6c-4874-91aa-9af1180a0fb6.png)

![gameplay](https://user-images.githubusercontent.com/587136/195953778-b02e8f94-7aa1-4233-993b-d6c20974e409.png)


## Getting Started

1. Open `Main` Scene in the Unity Editor
2. Locate the `HathoraClient` prefab in the object hierarchy (the text is blue in color)

![hierarchy](https://user-images.githubusercontent.com/587136/195953518-07cffc4f-3496-4d6e-ac9f-37f8815952c4.png)

3. Paste your Hathora App ID in the field provided in the Inspector

![inspector](https://user-images.githubusercontent.com/587136/195953529-0f7b71dc-6978-4f7b-9d47-471b5a5f7ad0.png)

4. Launch to play

## Interface to Hathora Client

The core loop of talking to the Hathora Client looks like this:

```c#

public class Game : MonoBehaviour {

    Hathora.Client hathoraClient;

    bool hasLoaded = false;
    bool hasQuit   = false;

    // ... other variables

    private async void Awake() {

        // Hathora Client will periodically call RenderContent
        // as long as the web socket connection is open
        //
        await hathoraClient.Connect(RenderContent);
    }

    private void RenderContent(string contentData) {
        if (!hasLoaded) {
            hasLoaded = true;

            // This is the first call from the Hathora Client
            // to RenderContent, so we can do any initializing
            // here that depends on data sent from the server
            // like drawing a map, etc.

            Debug.Log("Connected.");
        }

        // Game has quit async, don't try to access any member variables
        // because they may not exist. Handle any cleanup in OnApplicationQuit
        //
        if (hasQuit) {
            return;
        }

        // This is the message sent periodically by the Hathora Client over the websocket
        // `ServerMessage` is a type defined for your game specifically
        //
        ServerMessage message = JsonUtility.FromJson<ServerMessage>(contentData);
        if (message != null) {

            // Process the message here
            // You will probably be extracting the game state from the message
            // and redrawing your game's current view based on that updated state
        }
    }

    // A Unity hook that is invoked when the game quits
    //
    private async void OnApplicationQuit() {
        hasQuit = true;

        // Close the websocket connection
        await hathoraClient.Disconnect();
        Debug.Log("Disconnected.");
    }
}

```
