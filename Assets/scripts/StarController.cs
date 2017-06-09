using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarController : PlayerController {
  private AmpersandController ampersand;
  private Text loot;

  override public void Start() {
    base.Start();
    this.Type = "D";
    loot = transform.Find("canvas/loot").GetComponent<Text>();
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
  }

  override public void Update() {
    base.Update();

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
}
