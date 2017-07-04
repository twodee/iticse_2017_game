using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class PlayerController : MonoBehaviour {
  public float speed;

  protected FootController foot;
  protected FootController head;

  protected bool isAirborne;
  protected new Rigidbody2D rigidbody;
  protected PlayerController otherPlayer;
//  public bool isBurden;
  private int otherMask;
  protected bool isLocked;
  private float oomph;

  protected Text loot;
  protected SpriteRenderer lootSprite;

  public CellController targetCell;
  public LevelController levelController;

  protected Tool activeTool;
  protected Tool inactiveTool;

//  private bool isSquishing;

  // loot related
  public string LootText {
    get {
      return loot.text;
    }
    set
    {
      loot.text = value;
      if (value == "") {
        lootSprite.sprite = null;
      }
      else {
        Sprite s = levelController.GetSprite(value);
        lootSprite.sprite = s;
      }
    }
  }
  public GameObject Loot {
    get {
      return lootSprite.gameObject;
    }
  }
  public Vector3 LootPosition {
    get {
      return lootSprite.transform.position;
    }
  }

  // tool related
  public Tool ActiveTool {
    set
    {
      activeTool = value;
      activeTool.Player = this;
    }
  }
  public Tool InActiveTool {
    set
    {
      inactiveTool = value;
      inactiveTool.Player = this;
    }
  }
  public void SwapTool() {
    Tool temp = activeTool;
    activeTool = inactiveTool;
    inactiveTool = temp;
  }

  virtual public void Start() {
    rigidbody = GetComponent<Rigidbody2D>();
    foot = transform.Find("foot").GetComponent<FootController>();
    loot = transform.Find("loot/canvas/text").GetComponent<Text>();
    loot.text = "";
    lootSprite = transform.Find("loot").GetComponent<SpriteRenderer>();
    levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();
    head = transform.Find("head").GetComponent<FootController>();

    targetCell = null;

    isAirborne = false;
    isLocked = false;
  }

  protected void Interact(bool squish) {
    if (squish) {
      StartCoroutine(SquishAndInteract());
    }
    else {
      activeTool.Interact();
    }
  }

  IEnumerator SquishAndInteract() {
//    isSquishing = true;

    // Squat
    Vector2 startPosition = gameObject.transform.position;
    Vector2 endPosition = (Vector2)gameObject.transform.position - Vector2.up * 0.1f;
    Vector3 startScale = gameObject.transform.localScale;
    Vector3 endScale = new Vector3(1.2f, 0.8f, 1.0f);

    float startTime = Time.time;
    float targetTime = 0.1f;
    float elapsedTime = 0.0f;

    // Squat down and widen.
    while (elapsedTime < targetTime) {
      gameObject.transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / targetTime);
      gameObject.transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }

    // And return to form...
    startTime = Time.time;
    elapsedTime = 0.0f;
    while (elapsedTime < targetTime) {
      gameObject.transform.position = Vector2.Lerp(endPosition, startPosition, elapsedTime / targetTime);
      gameObject.transform.localScale = Vector3.Lerp(endScale, startScale, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }

    gameObject.transform.position = startPosition;
    gameObject.transform.localScale = startScale;
//    isSquishing = false;

    activeTool.Interact();
  }

  public void Lock() {
    isLocked = true;
    rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
  }
  public void UnLock() {
    rigidbody.constraints = RigidbodyConstraints2D.None;
    isLocked = false;
  }

  virtual public void Update() {
    if (isLocked) {
      return;
    }

    oomph = Input.GetAxis("Horizontal" + type);
    if (Input.GetButton("Jump" + type)) {
      if (!isAirborne) {
//        rigidbody.mass = 1;
        isAirborne = true;
        Jump();
      }
    }

    if (!isAirborne && Input.GetButtonDown("Transmit" + type)) {
      Interact(true);
    }
  }

  virtual protected void Jump (){
    rigidbody.AddForce(Vector2.up * 300);
	}

  public void Acquire(string label) {
    LootText = label;
    levelController.OnCollect(label);
  }

  void LateUpdate() {
//    if (isBurden) {
//      oomph += otherPlayer.oomph;
//    }
    rigidbody.velocity = new Vector2(oomph * speed, rigidbody.velocity.y);
  }

  virtual public bool IsTransmittable() {
    return targetCell != null && !(loot.text == "" && targetCell.Loot == "");
  }

  public GameObject GetOnCell() {
    GameObject go = GetOnCell(foot, CellBehavior.UP);
    return go != null ? go : GetOnCell(head, CellBehavior.DOWN);
  }

  public GameObject GetOnCell(FootController test, int direction) {
    Collider2D hit = Physics2D.OverlapBox(test.position, new Vector2(test.width, test.height), 0, Utilities.GROUND_MASK);
    if (hit != null && hit.gameObject.tag == "cell") {
      CellController cc = hit.gameObject.GetComponent<CellController>();
      if (!cc.IsBlocked(direction)) {
        return hit.gameObject;
      }
    }
    return null;
  }

//  bool IsGrounded() {
//    Collider2D hit = Physics2D.OverlapBox(foot.position, new Vector2(foot.width, foot.height), 0, Utilities.GROUND_MASK | otherMask);
//    return hit != null;
//  }

  virtual public GameObject GetOnPointer() {
    return GetOnPointer(foot, CellBehavior.UP);
  }

  public GameObject GetOnPointer(FootController test, int direction) {
    Collider2D hit = Physics2D.OverlapBox(test.position, new Vector2(test.width, test.height), 0, Utilities.GROUND_MASK);
    if (hit != null && hit.gameObject.tag == "pointer") {
      CellBehavior cc = hit.gameObject.GetComponent<CellBehavior>();
      if (!cc.IsBlocked(direction)) {
        return hit.gameObject;
      }
    }
    else if (hit != null && hit.gameObject.tag == "cell") {
      // does it have a parent that is a linked cell?
      Transform parentTransform = hit.gameObject.transform.parent;
      if (parentTransform != null && parentTransform.gameObject.tag == "linkedCell") {
        return parentTransform.GetComponentInChildren<PointerController>().gameObject;
      }
      else {
        return null;
      }
    }
    return null;
  }

  virtual public bool IsConnectedToOther() {

	  // check if close to other player and they are on a pointer
	  CircleCollider2D otherCollider = otherPlayer.GetComponent<CircleCollider2D>();
	  ColliderDistance2D distance = rigidbody.Distance(otherCollider);

	  return distance.distance < otherCollider.radius;
  }

  virtual protected void OnCollisionEnter2D(Collision2D collision) {
//    if (!isLocked) {
    // If the player's foot touches the ground, we want to be able to jump
    // again.
    if (isAirborne) {
      Collider2D hit = Physics2D.OverlapBox(foot.position, new Vector2(foot.width, foot.height), 0, Utilities.GROUND_MASK);
      if (hit != null) {
        isAirborne = false;
      }
    }
      // If we land on top of the other people, let's reduce our mass to 0 so we
      // don't impede that player's jump.
//      if ((1 << collision.gameObject.layer) == otherMask) {
//        isBurden = transform.position.y > collision.gameObject.transform.position.y + 0.2;
//        if (isBurden) {
//          rigidbody.mass = 0;
//        }
//      }
    
  }

  void OnCollisionExit2D(Collision2D collision) {
//    if (!isSquishing) {
//      if (collision.gameObject == otherPlayer.gameObject) {
//        rigidbody.mass = 1;
//        isBurden = false;
//      }
//    }
  }

  protected string type;
  public string Type {
    get {
      return type;
    }

    set {
      type = value;
      if (type == "D") {
        otherMask = Utilities.PLAYER_R_MASK;
      } else {
        otherMask = Utilities.PLAYER_D_MASK;
      }
    }
  }
}
