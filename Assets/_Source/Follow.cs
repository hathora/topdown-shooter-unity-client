using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Follow : MonoBehaviour {

    Camera _camera;
    Transform target = null;

    void Awake() {
        _camera = GetComponent<Camera>();
    }

    void Update() {
        if (target != null) {
            Vector3 newPos = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = newPos;
        }
    }

    // Public Methods
    //

    public void FollowTarget(Transform target) {
        this.target = target;
    }
}
