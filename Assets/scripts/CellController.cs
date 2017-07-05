using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : CellBehavior {
  
  private Text loot;
  private SpriteRenderer lootSprite;
  private GameObject lootObject;
  private SpriteRenderer expectedSprite;
  private Color expectedColor;
  private bool hasExpected;

  override protected void Awake() {
    base.Awake();

    loot = transform.Find("loot/canvas/text").GetComponent<Text>();
    lootObject = transform.Find("loot").gameObject;
    lootSprite = lootObject.GetComponent<SpriteRenderer>();
    expectedSprite = transform.Find("expected").GetComponent<SpriteRenderer>();

  }

  void Update() {
    if (hasExpected) {
      expectedColor.a = Math.Abs(1.5f - (Time.time % 3.0f)) / 2.0f;
      expectedSprite.color = expectedColor;
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

  override public void SetLoot(string text) {
    Loot = text;
  }
  override public string GetLoot() {
    return Loot;
  }
  override public GameObject GetLootObject() {
    return lootObject;
  }

  public void SetExpected(Sprite sprite) {
    expectedSprite.sprite = sprite;
    expectedColor = new Color(1f, 1f, 1f, 0f);
    expectedSprite.color = expectedColor;
    hasExpected = true;
  }
}
