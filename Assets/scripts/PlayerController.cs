using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
  public float speed;
  public float groundRadius;

  private new Rigidbody2D rigidbody;
  private Transform foot;
  private bool isJumping;
  private LineRenderer lineRenderer;
  private bool isTargeted;

  void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    foot = transform.Find("foot");
    isJumping = false;
    lineRenderer = GetComponent<LineRenderer>();
  }
  
  void Update() {
    float horizontal = Input.GetAxis("HorizontalR");
    rigidbody.velocity = new Vector2(horizontal * speed, rigidbody.velocity.y);

    if (Input.GetButton("Jump") && !isJumping) {
      Collider2D hit = Physics2D.OverlapCircle(foot.position, groundRadius, Utilities.GROUND_MASK);
      if (hit != null) {
        rigidbody.AddForce(Vector2.up * 300);
        isJumping = true;
      }
    }

    if (Input.GetMouseButtonDown(0)) {
      Vector3 mouseInPixels = Input.mousePosition;
      Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mouseInPixels);
      mouseInWorld.z = 0;
      lineRenderer.SetPosition(1, mouseInWorld); 
      isTargeted = true;
    }

    if (isTargeted) {
      lineRenderer.SetPosition(0, transform.position); 
    }
  }

  void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.layer == Utilities.GROUND_LAYER) {
      isJumping = false;
    }
  }
}
