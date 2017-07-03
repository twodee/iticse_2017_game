using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : MonoBehaviour {
  public static int UP = 1;
  public static int DOWN = 2;
  public static int LEFT = 4;
  public static int RIGHT = 8;

  private Text loot;
  private SpriteRenderer lootSprite;
  public PointerController pointer;
  public bool immutable;
  private int blocked;

  private SpriteRenderer upBarrier;
  private SpriteRenderer downBarrier;

  private LevelController levelController;

  void Awake() {
    loot = transform.Find("loot/canvas/text").GetComponent<Text>();
    lootSprite = transform.Find("loot").GetComponent<SpriteRenderer>();

    upBarrier = transform.Find("UpBarrier").GetComponent<SpriteRenderer>();
    downBarrier = transform.Find("DownBarrier").GetComponent<SpriteRenderer>();

    levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();

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
