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
  private int blocked;

  private SpriteRenderer upBarrier;
  private SpriteRenderer downBarrier;

  void Awake() {
    label = transform.Find("canvas/text").GetComponent<Text>();
    upBarrier = transform.Find("UpBarrier").GetComponent<SpriteRenderer>();
    downBarrier = transform.Find("DownBarrier").GetComponent<SpriteRenderer>();

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

  public int Blocked {
    get {
      return blocked;
    }

    set {
      blocked = value;
      upBarrier.enabled = IsBlocked(UP);
      downBarrier.enabled = IsBlocked(DOWN);
    }
  }

  public bool IsBlocked(int direction) {
    return (blocked & direction) > 0;
  }
}
