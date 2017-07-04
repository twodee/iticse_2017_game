using UnityEngine;

public class CellBehavior : MonoBehaviour {
  public static int UP = 1;
  public static int DOWN = 2;
  public static int LEFT = 4;
  public static int RIGHT = 8;

  public bool immutable;
  private int blocked;

  private SpriteRenderer upBarrier;
  private SpriteRenderer downBarrier;

  protected LevelController levelController;

  protected void Awake() {

    upBarrier = transform.Find("UpBarrier").GetComponent<SpriteRenderer>();
    downBarrier = transform.Find("DownBarrier").GetComponent<SpriteRenderer>();

    levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();
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


