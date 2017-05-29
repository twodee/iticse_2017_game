using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarController : MonoBehaviour {
  public float speed;
  public float groundRadius;

  private new Rigidbody2D rigidbody;
  private Transform foot;
  private bool isJumping;
  private AmpersandController ampersand;
  private Text loot;

  void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    foot = transform.Find("foot");
    loot = transform.Find("canvas/loot").GetComponent<Text>();
    isJumping = false;
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
  }

  void Update() {
    float horizontal = Input.GetAxis("HorizontalD");
    rigidbody.velocity = new Vector2(horizontal * speed, rigidbody.velocity.y);

    if (Input.GetButton("JumpD") && !isJumping) {
      Collider2D hit = Physics2D.OverlapCircle(foot.position, groundRadius, Utilities.GROUND_MASK);
      if (hit != null) {
        rigidbody.AddForce(Vector2.up * 300);
        isJumping = true;
      }
    }

    if (Input.GetButton("Get")) {
      if (Vector2.Distance(transform.position, ampersand.gameObject.transform.position) <= 0.5f) {
        if (ampersand.Target != null) {
          loot.text = ampersand.Target.Label;
        }
      }
    }

    if (Input.GetButton("Put")) {
      if (Vector2.Distance(transform.position, ampersand.gameObject.transform.position) <= 0.5f) {
        if (ampersand.Target != null) {
          ampersand.Target.Label = loot.text;
        }
      }
    }
  }

  void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.layer == Utilities.GROUND_LAYER) {
      isJumping = false;
    }
  }
}
