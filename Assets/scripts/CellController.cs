using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : CellBehavior {
  
  private Text loot;
  private SpriteRenderer lootSprite;
  private GameObject lootObject;
  private SpriteRenderer expectedSprite;
  private string expectedValue;
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
      if (loot.text == expectedValue) {
        expectedColor.a = 0;
      }
      else {
        expectedColor.a = Math.Abs(1.5f - (Time.time % 3.0f)) / 2.0f;
      }
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

  public void SetExpected(string expectedValue) {
    this.expectedValue = expectedValue;
    expectedSprite.sprite = levelController.GetSprite(expectedValue);
    expectedColor = new Color(1f, 1f, 1f, 0f);
    expectedSprite.color = expectedColor;
    hasExpected = true;
  }
}
