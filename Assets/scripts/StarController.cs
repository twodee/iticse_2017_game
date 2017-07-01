using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarController : PlayerController {
  private AmpersandController ampersand;
  private Text loot;

  private CellController targetCell;

  private ConsoleController consoleController;

  override public void Start() {
    base.Start();
    this.Type = "D";
    loot = transform.Find("canvas/loot").GetComponent<Text>();
    loot.text = "";
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
    otherPlayer = ampersand;
    consoleController = GameObject.Find("/HUD/Console").GetComponent<ConsoleController>();
  }

  override public bool IsConnectedToOther() {
    return true;
  }

  public GameObject GetOnCell() {
    Collider2D hit = Physics2D.OverlapBox(foot.position, new Vector2(foot.width, foot.height), 0,     Utilities.GROUND_MASK);
    if (hit != null && hit.gameObject.tag == "cell") {
      return hit.gameObject;
    }
    return null;
  }

  public void Acquire(string label) {
    loot.text = label;
    consoleController.Acquire(label);
  }

  override public void Update() {
    base.Update();

    if (Input.GetButtonDown("Dereference")) {
      // check to see if star or ampersand are on same pointer
      GameObject on = GetOnPointer();
//      bool connected = IsConnectedToOther();
//      if (on == null && connected) {
//        on = ampersand.GetOnPointer(); // if ampersand is on a pointer and they are close
//      }

      if (on != null) {
        PointerController pointer = on.GetComponent<PointerController>();
//        if (pointer.Target != null) {
//          // teleport the star and ampersand (if connected) on top of the target cell
//          CellController cc = pointer.Target;
//          Vector2 pos = (Vector2)cc.transform.position;
//          BoxCollider2D b = cc.transform.GetComponent<BoxCollider2D>();
//          float sizeStar = GetComponent<CircleCollider2D>().radius;
//          float sizeAmp = ampersand.GetComponent<CircleCollider2D>().radius;
//
//          gameObject.transform.position = new Vector2(pos.x - sizeStar, pos.y + b.bounds.size.y / 2 + sizeStar);
//          if (connected) {
//            ampersand.gameObject.transform.position = new Vector2(pos.x, pos.y + b.bounds.size.y / 2 + sizeAmp);
//          }
//        }

      }
    }
    if (!isAirborne && Input.GetButtonDown("Transmit" + type)) {
      targetCell = null;
      GameObject cell = GetOnCell();
      if (cell != null) {
        targetCell = cell.GetComponent<CellController>();
      }
      GameObject pointer = GetOnPointer();
      if (pointer != null) {
		    targetCell = pointer.GetComponent<PointerController>().Target;
      }

      isLocked = true;
      StartCoroutine(TransmitAndUnlock());
    }
  }

  override protected void Jump () {
    rigidbody.AddForce(Vector2.down * 300);
  }

  override public void LevelEnd() {
    loot.text = "";
  }

  override public void LevelStart() {
  }

  override public bool IsTransmittable() {
    return targetCell != null ;
  }

  override public IEnumerator Transmit() {
    if (loot.text == "") {
      return GetValue();
  	}
  	else {
      return PutValue();
    }
  }

  private IEnumerator PutValue() {
    GameObject payload = Instantiate(loot.gameObject);
    payload.transform.SetParent(targetCell.gameObject.transform.Find("canvas"));
    payload.transform.position = gameObject.transform.position;

    Vector2 startPosition = gameObject.transform.position;
    Vector2 endPosition = targetCell.gameObject.transform.position;

    float startTime = Time.time;
    float targetTime = 1.0f;
    float elapsedTime = 0.0f;

    while (elapsedTime <= targetTime) {
      payload.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }

    Destroy(payload);
    targetCell.Label = loot.text;
    loot.text = "";
  }

  private IEnumerator GetValue() {
    GameObject cell = targetCell.gameObject.transform.Find("canvas/text").gameObject;
    GameObject payload = Instantiate(cell);
    payload.GetComponentInChildren<Text>().enabled = true;
    payload.transform.SetParent(targetCell.gameObject.transform.Find("canvas"));
    payload.transform.position = cell.transform.position;

    Vector2 startPosition = payload.transform.position;
    Vector2 endPosition = gameObject.transform.position;

    float startTime = Time.time;
    float targetTime = 1.0f;
    float elapsedTime = 0.0f;

    while (elapsedTime <= targetTime) {
      payload.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }

    Acquire(targetCell.Label);
    Destroy(payload);
  }

  public Vector2 LootPosition {
    get {
      return loot.gameObject.transform.position;
    }
  }
}
