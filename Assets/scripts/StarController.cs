using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarController : PlayerController {
  private AmpersandController ampersand;
  private Text loot;
  protected FootController head;

  private CellController targetCell;

  private LevelController levelController;

  override public void Start() {
    base.Start();
    this.Type = "D";
    head = transform.Find("head").GetComponent<FootController>();

    loot = transform.Find("canvas/loot").GetComponent<Text>();
    loot.text = "";
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
    otherPlayer = ampersand;
    levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();
  }

  override public bool IsConnectedToOther() {
    return true;
  }

  public GameObject GetOnCell() {
    Collider2D hit = Physics2D.OverlapBox(foot.position, new Vector2(foot.width, foot.height), 0,     Utilities.GROUND_MASK);
    if (hit != null && hit.gameObject.tag == "cell") {
      return hit.gameObject;
    }
    hit = Physics2D.OverlapBox(head.position, new Vector2(head.width, head.height), 0,     Utilities.GROUND_MASK);
    if (hit != null && hit.gameObject.tag == "cell") {
      return hit.gameObject;
    }
    return null;
  }

  public void Acquire(string label) {
    loot.text = label;
    levelController.OnCollect(label);
  }

  override public void Update() {
    if (isLocked) {
      return;
    }

    base.Update();

    if (Input.GetButtonDown("Dereference")) {
      // check to see if star or ampersand are on same pointer
      GameObject on = GetOnPointer();
//      bool connected = IsConnectedToOther();
//      if (on == null && connected) {
//        on = ampersand.GetOnPointer(); // if ampersand is on a pointer and they are close
//      }

      if (on != null) {
//        PointerController pointer = on.GetComponent<PointerController>();
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
      Interact(true);
    }
  }

  virtual public GameObject GetOnPointer() {
    GameObject pointer = base.GetOnPointer();
    if (pointer == null) {
      pointer = GetOnPointer(head);
    }
    return pointer;
  }

  void Interact(bool squish) {
    targetCell = null;
    GameObject cell = GetOnCell();
    if (cell != null) {
      targetCell = cell.GetComponent<CellController>();
    }
    GameObject pointer = GetOnPointer();
    if (pointer != null) {
      targetCell = pointer.GetComponent<PointerController>().Target;
    }

    if (squish) {
      isLocked = true;
      StartCoroutine(TransmitAndUnlock());
    }
    else {
      if (IsTransmittable()) {
        StartCoroutine(Transmit());
      }
    }
  }

  override protected void Jump () {
    rigidbody.AddForce(Vector2.up * 500);
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
    if (loot.text == "" || targetCell.immutable) {
      return GetValue();
  	}
  	else {
      return PutValue();
    }
  }

  protected override void OnCollisionEnter2D(Collision2D collision) {
    base.OnCollisionEnter2D(collision);
    // check to see if the head has hit a cell
    Collider2D hit = Physics2D.OverlapBox(head.position, new Vector2(head.width, head.height), 0, Utilities.GROUND_MASK);
    if (hit != null) {
      if (hit.gameObject.tag == "cell" || hit.gameObject.tag == "pointer") {
        rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        Interact(false);
      }
    }

  }

  private IEnumerator PutValue() {
    GameObject payload = Instantiate(loot.gameObject);
    payload.transform.SetParent(targetCell.gameObject.transform.Find("canvas"));
    payload.transform.position = gameObject.transform.position;
    string value = loot.text;
    loot.text = "";

    Vector2 startPosition = gameObject.transform.position;
    Vector2 endPosition = targetCell.gameObject.transform.position;

    float startTime = Time.time;
    float targetTime = (endPosition-startPosition).magnitude / 5.0f;
    float elapsedTime = 0.0f;

    while (elapsedTime <= targetTime) {
      payload.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }

    Destroy(payload);
    targetCell.Label = value;
    targetCell.GetComponentInChildren<Text>().enabled = true;
    levelController.OnTransmit();
    rigidbody.constraints = RigidbodyConstraints2D.None;

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
    float targetTime = (endPosition-startPosition).magnitude / 5.0f;
    float elapsedTime = 0.0f;

    while (elapsedTime <= targetTime) {
      payload.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }

    Acquire(targetCell.Label);
    Destroy(payload);
    levelController.OnTransmit();
    rigidbody.constraints = RigidbodyConstraints2D.None;

  }

  public Vector2 LootPosition {
    get {
      return loot.gameObject.transform.position;
    }
  }
}
