using UnityEngine;
using System.Collections;

public class AmpersandController : MonoBehaviour {
  public float speed;
  public float groundRadius;

  private new Rigidbody2D rigidbody;
  private Transform foot;
  private bool isJumping;
  private LineRenderer lineRenderer;
  private bool targetedThisFrame;
  private CellController target;

  void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    foot = transform.Find("foot");
    isJumping = false;
    lineRenderer = GetComponent<LineRenderer>();
    targetedThisFrame = false;
    target = null;
  }
  
  void Update() {
    float horizontal = Input.GetAxis("HorizontalR");
    rigidbody.velocity = new Vector2(horizontal * speed, rigidbody.velocity.y);

    if (Input.GetButton("JumpR") && !isJumping) {
      Collider2D hit = Physics2D.OverlapCircle(foot.position, groundRadius, Utilities.GROUND_MASK);
      if (hit != null) {
        rigidbody.AddForce(Vector2.up * 300);
        isJumping = true;
      }
    }

    if (!targetedThisFrame && Input.GetMouseButtonDown(0)) {
      lineRenderer.enabled = false;
      target = null;
    }
    targetedThisFrame = false;

    if (lineRenderer.enabled) {
      lineRenderer.SetPosition(0, transform.position); 
    }
  }

  void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.layer == Utilities.GROUND_LAYER) {
      isJumping = false;
    }
  }

  public CellController Target {
    get {
      return target;
    }

    set {
      target = value;
      lineRenderer.enabled = true;
      lineRenderer.SetPosition(1, target.gameObject.transform.position); 
      targetedThisFrame = true;
    }
  }
}
