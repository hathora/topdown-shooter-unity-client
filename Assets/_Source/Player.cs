using UnityEngine;

public class Player {

    public string id;
    public float x;
    public float y;

    public override string ToString() {
        return JsonUtility.ToJson(this);
    }

}
