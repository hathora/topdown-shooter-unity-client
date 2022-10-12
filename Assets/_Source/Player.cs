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
        transform.position = pos;

        float rotation = -data.aimAngle * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);
    }
}
