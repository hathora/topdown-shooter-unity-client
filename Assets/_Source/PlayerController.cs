using UnityEngine;

using DataTypes.Game;
using DataTypes.Network.ClientMessages;

public class PlayerController : MonoBehaviour {

    bool isEnabled = false;
    Hathora.ClientManager hathoraClient;

    Direction prevDirection = Direction.None;
    float prevAngle = 0.0f;

    Camera mainCamera;

    private void Awake() {
        hathoraClient = Hathora.ClientManager.GetInstance();
        mainCamera    = Camera.main;
    }

    void Update() {
        if (isEnabled) {

            // Movement
            //
            Direction direction;

            if (Input.GetKey(KeyCode.W)) {
                direction = Direction.Up;
            } else if (Input.GetKey(KeyCode.S)) {
                direction = Direction.Down;
            } else if (Input.GetKey(KeyCode.A)) {
                direction = Direction.Left;
            } else if (Input.GetKey(KeyCode.D)) {
                direction = Direction.Right;
            } else {
                direction = Direction.None;
            }

            if (direction != prevDirection) {
                hathoraClient.Send(new SetDirectionMessage(direction));
                prevDirection = direction;
            }


            // Aim Angle
            //

            Vector2 playerPos = transform.position;
            Vector2 mousePos  = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            float aimAngle = Mathf.Atan2(playerPos.y - mousePos.y, mousePos.x - playerPos.x);

            if (aimAngle != prevAngle) {
                hathoraClient.Send(new SetAngleMessage(aimAngle));
                prevAngle = aimAngle;
            }


            // Shoot
            //
            if (Input.GetMouseButtonDown(0)) {
                hathoraClient.Send(new ShootMessage());
            }
        }
    }


    // Public Methods
    //

    public void Enable() {
        isEnabled = true;
    }
}
