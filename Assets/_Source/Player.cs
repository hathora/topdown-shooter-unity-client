using UnityEngine;
using DataTypes.Game;

using System.Collections;

public class Player : MonoBehaviour {

    [SerializeField]
    float deathAnimationDuration = 0.25f;

    string id;
    bool isCurrentPlayer;
    Animator animator;

    // Position to move to
    Vector3 targetPosition;

    // Used to interpolate server position
    // with client's position to create smooth
    // movement
    float smoothTime = 0.05f;
    float speed = 100;
    Vector3 velocity;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

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

    public void PlayDeathAnimationAndRemove() {
        if (animator) {
            animator.SetTrigger("Death");
            StartCoroutine(WaitAndDestroy());
        } else {
            DestroySelf();
        }
    }

    IEnumerator WaitAndDestroy() {
        yield return new WaitForSeconds(deathAnimationDuration);
        DestroySelf();
    }

    private void DestroySelf() {
        Object.Destroy(gameObject);
    }
}
