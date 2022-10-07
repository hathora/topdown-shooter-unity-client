using UnityEngine;
using DataTypes.Game;

public class Bullet : MonoBehaviour {

    string id;

    public void Init(BulletData data) {
        this.id = data.id;
    }

    public void Render(BulletData data) {
        Vector3 pos = new Vector3(data.position.x, data.position.y);
        transform.position = pos;
    }
}