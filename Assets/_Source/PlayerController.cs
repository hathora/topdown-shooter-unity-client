using UnityEngine;

using Hathora;
using DataTypes.Game;
using DataTypes.Network.ClientMessages;

public class PlayerController : MonoBehaviour {

    // Config
    //

    [SerializeField]
    float aimTolerance = 0.01f;

    //
    // \Config

    bool isEnabled = false;
    Hathora.Client hathoraClient;

    Direction prevDirection = Direction.None;
    float prevAngle = 0.0f;

    private void Awake() {
        hathoraClient = Hathora.Client.GetInstance();
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
                // Debug.Log("Change Direction: " + direction);
                hathoraClient.Send(new SetDirectionMessage(direction));
                prevDirection = direction;
            }


            // Aim Angle
            //

            Vector2 playerPos = transform.position;
            Vector2 mousePos  = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 targetDir = mousePos - playerPos;
            float aimAngle = Mathf.Deg2Rad * Vector3.Angle(targetDir, transform.right);

            if (Mathf.Abs(aimAngle - prevAngle) > aimTolerance) {
                // Debug.Log("ANGLE: " + aimAngle);
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
