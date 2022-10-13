using UnityEngine;
using DataTypes.Game;

public class Player : MonoBehaviour {

    string id;
    bool isCurrentPlayer;

    // Position to move to
    Vector3 targetPosition;

    // Used to interpolate server position
    // with client's position to create smooth
    // movement
    float smoothTime = 0.05f;
    float speed = 100;
    Vector3 velocity;

    public void Init(PlayerData data, bool isCurrentPlayer = false) {
        this.id = data.id;
        this.isCurrentPlayer = isCurrentPlayer;
        targetPosition = transform.position;

        if (isCurrentPlayer) {
            PlayerController controller = GetComponent<PlayerController>();
            controller.Enable();
        }
    }

    public void Render(PlayerData data) {
        targetPosition = new Vector3(data.position.x, data.position.y);

        float rotation = -data.aimAngle * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);
    }

    void Update() {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, speed);
    }
}
