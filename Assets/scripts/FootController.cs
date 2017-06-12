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
}
