using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : MonoBehaviour {
  private Text label;
  public PointerController pointer;
  public bool immutable;
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
}
