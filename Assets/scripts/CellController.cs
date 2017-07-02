using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : MonoBehaviour {
  public static int UP = 1;
  public static int DOWN = 2;
  public static int LEFT = 4;
  public static int RIGHT = 8;

  private Text label;
  public PointerController pointer;
  public bool immutable;
  public int blocked;

  void Start() {
    label = transform.Find("canvas/text").GetComponent<Text>();
    if (transform.parent != null && transform.parent.gameObject.tag == "linkedCell") {
      pointer = transform.parent.GetComponentInChildren<PointerController>();
    }
    else {
      pointer = null;
    }
  }

  public string Label {
    get {
      return label.text;
    }

    set {
      label.text = value;
    }
  }

  public bool IsBlocked(int direction) {
    return (blocked & direction) > 0;
  }
}
