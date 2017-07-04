using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : CellBehavior {
  
  private Text loot;
  private SpriteRenderer lootSprite;
  public PointerController pointer;

  override protected void Awake() {
    base.Awake();

    loot = transform.Find("loot/canvas/text").GetComponent<Text>();
    lootSprite = transform.Find("loot").GetComponent<SpriteRenderer>();

    if (transform.parent != null && transform.parent.gameObject.tag == "linkedCell") {
      pointer = transform.parent.GetComponentInChildren<PointerController>();
    }
    else {
      pointer = null;
    }
  }

  public string Loot {
    get {
      return loot.text;
    }

    set {
      loot.text = value;
      Sprite s = levelController.GetSprite(value);
      lootSprite.sprite = s;
    }
  }

}
