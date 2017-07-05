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

  public int par;
  public ArrayList solutionCode;

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
    solutionCode = new ArrayList();
    Reset();
	}

  public void Reset() {
    objects.Clear();
    stack.Clear();
    heap.Clear();
    variableValue = 'a';
    variablePointer = 'p';
    solutionCode.Clear();
    par = 0;
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

  public void OnTransmit(CellBehavior cb, string player, bool read) {
    string dereferences = "";
    string variable = cb.variableName.ToString();
    if (cb.variableName == 0) {
      // might have come from permanent memory
      variable = "'" + cb.GetLoot() + "'";
    }
    while (cb.gameObject.tag == "pointer") {
      PointerController pc = (PointerController)cb;
      cb = pc.Target;
      dereferences += "*";
    }

    if (read) {
      consoleController.Status(player + " = " + dereferences + variable + ";");
    }
    else {
      consoleController.Status(dereferences + variable + " = " + player + ";");
    }
    CheckProgress();
  }

  public void OnAttach(PointerController cb, string player, bool read) {
    CheckProgress();
    CheckReachable();
  }

  public void OnIncrement() {
  }

  public void CheckProgress() {
    if (current.endLevelCondition.Check()) {
//      loader.LoadNextLevel();
//      consoleController
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
      if (current != null) {
        consoleController.LevelEnd();
      }
      current = value;
      consoleController.LevelStart();
    }
  }
}
