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
        Vector3 pos = new Vector3(data.position.x / 64, data.position.y / 64);
        transform.SetPositionAndRotation(pos, Quaternion.identity);

        float aimAngle = data.aimAngle;
        Debug.Log(id + ": AIM -> " + aimAngle);

        Quaternion rotation = transform.rotation;
        // Debug.Log("P ROT: " + rotation.z);
        // Debug.Log("T ROT: " + aimAngle);
        // Debug.Log("ROT BY: " + (aimAngle - rotation.z));

        // transform.Rotate(0, 0, 90);
        // rotation.z = aimAngle;
        // transform.rotation = rotation;
    }
}
