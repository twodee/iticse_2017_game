using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
  public float speed;
  public float groundRadius;

  private Transform foot;
  private bool isAirborne;
  private new Rigidbody2D rigidbody;
  private GameObject otherPlayer;
  private bool isBurden;

  virtual public void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    foot = transform.Find("foot");
    isAirborne = true;
    otherPlayer = null;
  }
  
  private float oomph;
  virtual public void Update() {
    oomph = Input.GetAxis("Horizontal" + Type);
    if (Input.GetButton("Jump" + Type) && !isAirborne) {
      rigidbody.mass = 1;
      rigidbody.AddForce(Vector2.up * 300);
      isAirborne = true;
    }
  }

  void LateUpdate() {
    if (otherPlayer != null && isBurden) {
      oomph += otherPlayer.GetComponent<PlayerController>().oomph;
    }

    rigidbody.velocity = new Vector2(oomph * speed, rigidbody.velocity.y);
  }

  bool isGrounded() {
    Collider2D hit = Physics2D.OverlapCircle(foot.position, groundRadius, Utilities.GROUND_MASK | Utilities.PLAYER_MASK);
    return hit != null;
  }

  void OnCollisionEnter2D(Collision2D collision) {
    // If the player's foot touches the ground, we want to be able to jump
    // again.
    isAirborne = !isGrounded();

    // If we land on top of the other people, let's reduce our mass to 0 so we
    // don't impede that player's jump.
    if (collision.gameObject.layer == Utilities.PLAYER_LAYER) {
      otherPlayer = collision.gameObject;
      isBurden = transform.position.y > collision.gameObject.transform.position.y;
      if (isBurden) {
        rigidbody.mass = 0;
        Debug.Log(gameObject.name + " mass now 0");
      }
    }
  }

  void OnCollisionExit2D(Collision2D collision) {
    if (collision.gameObject == otherPlayer) {
      otherPlayer = null;
      rigidbody.mass = 1;
      Debug.Log(gameObject.name + " mass back to 1");
      isBurden = false;
    }
  }

  public string Type {
    get;
    set;
  }
}
