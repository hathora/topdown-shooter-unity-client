using UnityEngine;
using DataTypes.Game;

public class Player : MonoBehaviour {

    string id;
    bool isCurrentPlayer;

    public void Init(PlayerData data, bool isCurrentPlayer = false) {
        this.id = data.id;
        this.isCurrentPlayer = isCurrentPlayer;

        if (isCurrentPlayer) {
            PlayerController controller = GetComponent<PlayerController>();
            controller.Enable();
        }
    }

    public void Render(PlayerData data) {
        Vector3 pos = new Vector3(data.position.x, data.position.y);
        // float rotation = data.aimAngle * Mathf.Rad2Deg;

        transform.position = pos;
        // transform.SetPositionAndRotation(pos, new Quaternion(0, 0, rotation, 0));

        // float aimAngle = data.aimAngle;
        // Debug.Log(id + ": AIM -> " + aimAngle);

        // Quaternion rotation = transform.rotation;
        // Debug.Log("P ROT: " + rotation.z);
        // Debug.Log("T ROT: " + aimAngle);
        // Debug.Log("ROT BY: " + (aimAngle - rotation.z));

        // transform.Rotate(0, 0, 90);
        // rotation.z = aimAngle;
        // transform.rotation = rotation;
    }
}
