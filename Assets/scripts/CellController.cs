using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : CellBehavior {
  
  private Text loot;
  private SpriteRenderer lootSprite;
  private GameObject lootObject;

  override protected void Awake() {
    base.Awake();

    loot = transform.Find("loot/canvas/text").GetComponent<Text>();
    lootObject = transform.Find("loot").gameObject;
    lootSprite = lootObject.GetComponent<SpriteRenderer>();

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

  override public void SetLoot(string text) {
    Loot = text;
  }
  override public string GetLoot() {
    return Loot;
  }
  override public GameObject GetLootObject() {
    return lootObject;
  }

}
