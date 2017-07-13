using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootController : MonoBehaviour {
  public float width;
  public float height;

  public Vector2 position {
    get {
      return gameObject.transform.position;
    }
  }
//  virtual protected void OnCollisionEnter2D(Collision2D collision) {
////    if (!isLocked) {
//    // If the player's foot touches the ground, we want to be able to jump
//    // again.
//    if (collision.gameObject.tag == "cell" || collision.gameObject.tag == "pointer") {
//      Debug.Log("foot hit " + collision.gameObject.tag);
//    }
//  }
}
