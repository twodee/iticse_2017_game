using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : MonoBehaviour {
  private AmpersandController ampersand;
  private Text label;

  void Start() {
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>(); 
    label = transform.Find("canvas/text").GetComponent<Text>();
  }

  void OnMouseDown() {
    ampersand.Target = this;
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
