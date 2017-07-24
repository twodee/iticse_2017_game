using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
  private Level current;
  private ConsoleController consoleController;
  private LevelLoader loader;

  private Dictionary<long, GameObject> objects;
  private Dictionary<char, GameObject> stack;
  private Dictionary<long, GameObject> heap;

  private char variableValue;
  private char variablePointer;


//  private PlayerController player1;
//  private PlayerController player2;

  public Sprite[] food;

  // Use this for initialization
   void Awake() {
    consoleController = GameObject.Find("/HUD/Console").GetComponent<ConsoleController>();
//    player1 = GameObject.Find("/players/star").GetComponent<StarController>();
//    player2 = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();

    loader = gameObject.GetComponent<LevelLoader>();
    objects = new Dictionary<long, GameObject>();
    stack = new Dictionary<char, GameObject>();
    heap = new Dictionary<long, GameObject>();
    Reset();
	}

  public void Reset() {
    objects.Clear();
    stack.Clear();
    heap.Clear();
    variableValue = 'a';
    variablePointer = 'p';
  }

  static long Key(int x, int y) {
    //implicit conversion of left to a long
    long res = x;

    //shift the bits creating an empty space on the right
    // ex: 0x0000CFFF becomes 0xCFFF0000
    res = (res << 32);

    //combine the bits on the right with the previous value
    // ex: 0xCFFF0000 | 0x0000ABCD becomes 0xCFFFABCD
    res = res | (long)(uint)y; //uint first to prevent loss of signed bit

    //return the combined result
    return res;
  }

  public GameObject FindAt(int x, int y) {
    return objects[Key(x, y)];
  }

  public bool HasAt(int x, int y) {
    return objects.ContainsKey(Key(x, y));
  }

  public void AddAt(int x, int y, GameObject go) {
    long key = Key(x, y); 
    objects[key] = go;
    if (go.tag == "pointer" || go.tag == "cell") {
      CellBehavior cb = go.GetComponent<CellBehavior>();
      if (y == 3) {
        cb.variableName = go.tag == "pointer" ? variablePointer++ : variableValue++;
        stack[cb.variableName] = go;
        cb.reachable = true;
      }
      else if (y > 3) {
        heap[key] = go;
        cb.heap = true;
      }
    }
  }

  public GameObject Malloc(int count, bool pointer) {
    // search entire heap for contiguous
    int startX = current.heapArea[0];
    int endX = current.heapArea[2];
    int startY = current.heapArea[1];
    int endY = current.heapArea[3];
    for (int y = startY; y <= endY; y++) {
      for (int x = startX; x < endX; x++) {
        if (x + count <= endX) {
          bool canAlloc = true;
          for (int i = -1; i <= count; i++) {
            if (HasAt(x+i, y)) {
              canAlloc = false;
            }
          }
          if (canAlloc) {
            GameObject go = null;
            for (int i = count - 1; i >= 0; i--) {
              Vector3 pos = new Vector3(x+i, y, 0);

              go = (GameObject)Instantiate(loader.cell, pos, Quaternion.identity);
              go.transform.SetParent(loader.transform);
          
              CellController cc = go.GetComponent<CellController>();
              cc.Loot = "";

              AddAt(x+i, y, go);
            }
            return go;
          }
        }
      }
    }
    return null;
  }

  public Sprite GetSprite(string text) {
    if (text.Length == 1) {
      char c = text[0];
      int distance = c - 'A';
      if (distance < food.Length) {
        return food[distance];
      }
    }
    return null;
  }
	
	// Update is called once per frame
	void Update() {
    if (Input.GetButtonDown("NextLevel")) {
      loader.LoadNextLevel();
    }
    if (Input.GetButtonDown("ResetLevel")) {
      loader.ResetLevel();
    }
	}

  public void OnCollect(string loot) {
    current.collected += loot;
    CheckProgress();
  }

  public void OnTransmit(CellBehavior cb, PointerController bp, PlayerController pc, bool read) {
    string player = pc.avatar;
    if (pc.CountTools() > 1) {
      player += pc.ActiveTool.id;
    }
    string variable = cb.variableName.ToString();
    if (bp != null) {
      variable = bp.variableName.ToString();
      cb = bp;
    }
    else if (cb.variableName == 0) {
      // might have come from permanent memory
      variable = "'" + cb.GetLoot() + "'";
    }

    while (cb.gameObject.tag == "pointer") {
      PointerController pointer = (PointerController)cb;
      cb = pointer.Target;
      // was it an array access using an offset or a "direct" dereference?
      if (pointer.CurrentOffset == null) {
        variable = "*" + variable;
      }
      else {
        string offsetVariable = pointer.CurrentOffset.Player.avatar + pointer.CurrentOffset.id;
        variable = variable + "[" + offsetVariable + "]";
      }

    }

    if (read) {
      consoleController.Status(player + " = " + variable + ";");
    }
    else {
      consoleController.Status(variable + " = " + player + ";");
    }
    CheckProgress();
  }

  public void OnAttach(PointerController bp, PlayerController pc, bool read) {
    string player = pc.avatar;
    if (pc.CountTools() > 1) {
      player += pc.ActiveTool.id;
    }
    string variable = bp.variableName.ToString();

    if (read) {
      consoleController.Status(player + " = " + variable + ";");
    }
    else {
      consoleController.Status(variable + " = " + player + ";");
    }

    CheckReachable();
    CheckProgress();
  }

  public void OnMalloc(MallocTool mallocTool, PlayerController pc, bool isValue, bool isArray, int count) {
    string player = pc.avatar + mallocTool.id;
    string op = isArray ? "new char["+count+"]" : "new char()";
    consoleController.Status(player + " = " + op + ";");
  }

  public void OnFree(PointerController bp, PlayerController pc, bool isArray) {
    string variable = bp.variableName.ToString();
    string op = isArray ? "delete[] " : "delete ";
    consoleController.Status(op + variable + ";");

    CheckReachable();
    CheckProgress();
  }

  public void OnIncrement(PointerController bp, PlayerController pc, int value) {
    string player = pc.avatar;
    string variable = bp.variableName.ToString();

    string av = Mathf.Abs(value).ToString();
    string inc = value == 1 ? "++" : value == -1 ? "--" : (value < 0 ? " -= " : " += ") + av;
    consoleController.Status(variable + inc + ";");
  }

  public void OnOffsetChange(PlayerController pc, OffsetTool offsetTool, int value, bool increment) {
    string player = pc.avatar + offsetTool.id;
    string av = Mathf.Abs(value).ToString();
    string rhs;
    if (increment) {
      rhs = value == 1 ? "++" : value == -1 ? "--" : (value < 0 ? " -= " : " += ") + av;
    }
    else {
      rhs = " = " + value;
    }
    consoleController.Status(player + rhs + ";");

    
  }

  public void CheckProgress() {
    if (current.endLevelCondition.Check()) {
      consoleController.LevelEnd();
    }
  }

  public void CheckReachable() {
    // mark all heap as unreachable
    foreach (GameObject go in heap.Values) {
      CellBehavior cb = go.GetComponent<CellBehavior>();
      cb.reachable = false;
    }
    foreach (GameObject go in stack.Values) {
      CellBehavior cb = go.GetComponent<CellBehavior>();
      if (cb.tag == "pointer") {
        PointerController pc = (PointerController)cb;
        if (pc.Target != null) {
          MarkReachable(pc.Target);
        }
      }
    }
    int reached = 1;
    while (reached > 0) {
      reached = 0;
      foreach (GameObject go in heap.Values) {
        CellBehavior cb = go.GetComponent<CellBehavior>();
        if (cb.reachable) {
          if (cb.tag == "pointer") {
            PointerController pc = (PointerController)cb;
            if (pc.Target != null) {
              reached += MarkReachable(pc.Target);
            }
          }
        }
      }
    }
    foreach (GameObject go in heap.Values) {
      CellBehavior cb = go.GetComponent<CellBehavior>();
      if (!cb.reachable) {
        cb.GetComponent<SpriteRenderer>().color = new Color(0,0,0,1);
      }
    }
  }

  int MarkReachable(CellBehavior cb) {
    if (!cb.reachable) {
      int reached = 1;
      cb.reachable = true;
      if (cb.tag == "pointer") {
        PointerController pc = (PointerController)cb;
        reached += MarkReachable(pc.Target);
      }
      return reached + ReachArray(cb);
    }
    else {
      return 0;
    }
  }

  int ReachArray(CellBehavior cb) {
    if (cb.owningArray != null) {
      int reached = 0;
      foreach (GameObject sibling in cb.owningArray.objects) {
        reached += MarkReachable(sibling.GetComponent<CellBehavior>());
      }
      return reached;
    }
    else {
      return 0;
    }
  }

  public Level Current {
    get {
      return current;
    }
    set {

      current = value;
      consoleController.LevelStart();
    }
  }
}
