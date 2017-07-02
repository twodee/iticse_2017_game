using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarController : PlayerController {
  private AmpersandController ampersand;
  protected FootController head;



  override public void Start() {
    base.Start();
    this.Type = "D";
    head = transform.Find("head").GetComponent<FootController>();

    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
    otherPlayer = ampersand;
  }

  override public bool IsConnectedToOther() {
    return true;
  }

  public GameObject GetOnCell() {
    Collider2D hit = Physics2D.OverlapBox(foot.position, new Vector2(foot.width, foot.height), 0, Utilities.GROUND_MASK);
    if (hit != null && hit.gameObject.tag == "cell") {
      CellController cc = hit.gameObject.GetComponent<CellController>();
      if (!cc.IsBlocked(CellController.UP)) {
        return hit.gameObject;
      }
    }
    hit = Physics2D.OverlapBox(head.position, new Vector2(head.width, head.height), 0, Utilities.GROUND_MASK);
    if (hit != null && hit.gameObject.tag == "cell") {
      CellController cc = hit.gameObject.GetComponent<CellController>();
      if (!cc.IsBlocked(CellController.DOWN)) {
        return hit.gameObject;
      }
    }
    return null;
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

  }

  override public GameObject GetOnPointer() {
    GameObject pointer = base.GetOnPointer();
    if (pointer == null) {
      pointer = GetOnPointer(head);
    }
    return pointer;
  }

  override protected void Interact(bool squish) {
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
      else {
        rigidbody.constraints = RigidbodyConstraints2D.None;
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



  public Vector2 LootPosition {
    get {
      return loot.gameObject.transform.position;
    }
  }
}
