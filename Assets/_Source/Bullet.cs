using UnityEngine;
using DataTypes.Game;

public class Bullet : MonoBehaviour {

    string id;

    // Position to move to
    Vector3 targetPosition;

    // Used to interpolate server position
    // with client's position to create smooth
    // movement
    float smoothTime = 0.05f;
    float speed = 100;
    Vector3 velocity;

    public void Init(BulletData data) {
        this.id = data.id;
        targetPosition = transform.position;
    }

    public void Render(BulletData data) {
        targetPosition = new Vector3(data.position.x, data.position.y);
    }

    void Update() {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, speed);
    }
}