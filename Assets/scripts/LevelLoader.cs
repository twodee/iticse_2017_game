using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {
  private static int WORLD_HEIGHT = 9;
  private static float MIDDLE_BAR_Y = 2;

  public GameObject floor_platform;
  public GameObject linkedCell;
  public GameObject cell;
  public GameObject pointerCell;
  public GameObject counter;
  public GameObject cabinet;

  private AmpersandController ampersand;
  private StarController star;

  private LevelController levelController;
  private ConsoleController consoleController;

  private int currentWorld;
  private int currentLevel;

  private int valueType; // 0 = images only, 1 = text only, 2 = both


  public WorldLoader worldLoader;

  private Tool pointerTool;
  private Tool valueTool;
  private Tool incrementTool;
  private Tool offsetTool;

  private ArrayList tools;

  // Use this for initialization
  void Awake() {
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
    star = GameObject.Find("/players/star").GetComponent<StarController>();
    pointerTool = GameObject.Find("/pointerTool").GetComponent<Tool>();
    valueTool = GameObject.Find("/valueTool").GetComponent<Tool>();
    incrementTool = GameObject.Find("/incrementTool").GetComponent<Tool>();
    offsetTool = GameObject.Find("/offsetTool").GetComponent<Tool>();

    levelController = gameObject.GetComponent<LevelController>();
    consoleController = GameObject.Find("HUD/Console").GetComponent<ConsoleController>();
    tools = new ArrayList();
  }

  void Start () {
    currentWorld = PlayerPrefs.GetInt("currentWorld");
    currentLevel = PlayerPrefs.GetInt("currentLevel");

    valueType = PlayerPrefs.GetInt("valueType", 0);

    LoadLevel(currentWorld, currentLevel);
  }

  void EmptyLevel() {
    // Find all of our children and...eliminate them.

    levelController.Reset();

    while (transform.childCount > 0) {
      Transform c = transform.GetChild(0);
      c.SetParent(null); // become Batman
      Destroy(c.gameObject); // become The Joker
    }
    foreach (Tool tool in tools) {
      tool.gameObject.transform.SetParent(null);
      Destroy(tool.gameObject);
    }
    tools.Clear();
    // de-loot the players
    ampersand.Reset();
    star.Reset();
  }



  GameObject findAt(int x, int y) {
    return levelController.FindAt(x, y);
  }

  void loadObjects() {
    for (int i = 0; i < transform.childCount; i++) {
      Transform c = transform.GetChild(i);
      if (c.gameObject.tag != "ground") {
        levelController.AddAt((int)c.gameObject.transform.position.x,
         (int)c.gameObject.transform.position.y, c.gameObject);
      }
    }
  }

  void setDisplayTypeOnObject(GameObject go) {
    if (go.tag == "cell" || go.tag == "player") {
      if (valueType == 0) {
        go.transform.Find("loot/canvas").gameObject.SetActive(false);
      }
      else if (valueType == 1) {
        go.transform.Find("loot").gameObject.SetActive(false);
      }
    }
  }
  void setDisplayTypeOnObjects() {
    // change type of display here
    for (int i = 0; i < transform.childCount; i++) {
      Transform c = transform.GetChild(i);
      setDisplayTypeOnObject(c.gameObject);
    }
    setDisplayTypeOnObject(ampersand.gameObject);
    setDisplayTypeOnObject(star.gameObject);
  }

  public void ResetLevel() {
    LoadLevel(currentWorld, currentLevel);
  }

  public void LoadNextLevel() {
    currentLevel++;
    if (currentLevel == worldLoader.worlds[currentWorld]) {
      currentLevel = 0;
      currentWorld++;
    }
    if (currentWorld == worldLoader.worlds.Count) {
      currentWorld = 0;
    }
    LoadLevel(currentWorld, currentLevel);
    PlayerPrefs.SetInt("currentWorld", currentWorld);
    PlayerPrefs.SetInt("currentLevel", currentLevel);
  }


  void LoadLevel(int world, int level) {
    EmptyLevel();
    int index = worldLoader.GetIndex(world, level);

    // Read the data from the file in assets
    string levelName = (string)worldLoader.levels[index];
    TextAsset textFile = (TextAsset)worldLoader.levelAssets[levelName];
    string text = textFile.text;

    Regex replaceR = new Regex("\r");
    Regex replaceComment = new Regex("[ ]*;.*\n");
    Regex replaceLine = new Regex("[\n]*\n");
    Regex replaceBegin = new Regex("^\n");
    text = replaceR.Replace(text, "");
    text = replaceComment.Replace(text, "\n");
    text = replaceLine.Replace(text, "\n");
    text = replaceBegin.Replace(text, "");
    string[] lines = Regex.Split(text, "\n");

    int width = Convert.ToInt32(lines[0]);
    int height = Convert.ToInt32(lines[1]);
    Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    cam.transform.position = new Vector3((float)Convert.ToDouble(lines[2]), (float)Convert.ToDouble(lines[3]), -WORLD_HEIGHT);

    AndAllEndLevelCondition endLevelCondition = new AndAllEndLevelCondition();
    levelController.Current = new Level(levelName, lines[4].Trim(), endLevelCondition);
    endLevelCondition.Add(new CollectEndLevelCondition(levelController.Current, lines[5].Trim()));
    int offset = 6;
    ArrayList blockedCells = new ArrayList();

    for (int i = 0; i < height; i++) {
      float y = height - (i) - 1 + 0.5f;
      string s = lines[i + offset];
      int lastCell = -2; // prevent creating right away
      CellBehavior lastCellBehaviour = null;
      for (int j = 0; j < s.Length && j < width; j++) {
        float x = j + 0.5f;
        GameObject go = null;
        char c = s[j];
        Vector3 pos = new Vector3(x, y, 0);

        if (c >= 'A' && c <= 'Z') {
          go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          CellController cc = go.GetComponent<CellController>();
          if (y < MIDDLE_BAR_Y) {
            cc.immutable = true;
          }
          cc.Loot = c.ToString();
        }
        else if (c >= 'a' && c <= 'z') {
          go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
      
          CellController cc = go.GetComponent<CellController>();
          cc.Loot = "";
        }
        else if (c == '▔') {
          blockedCells.Add(new int[]{ (int)x, (int)y + 1, CellController.DOWN });
        }
        else if (c == '▁') {
          blockedCells.Add(new int[]{ (int)x, (int)y - 1, CellController.UP });
        }
        else if (c == '-') {
          go = (GameObject)Instantiate(cabinet, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '+') {
          go = (GameObject)Instantiate(counter, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '.') {
          go = (GameObject)Instantiate(pointerCell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '@') {
          go = (GameObject)Instantiate(linkedCell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          j++;
        }
        else if (c == '#') {
          go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '&') {
          ampersand.transform.position = new Vector3(pos.x, pos.y, -5);
        }
        else if (c == '*') {
          star.transform.position = new Vector3(pos.x, pos.y, -5);
        }

        if (go != null && (go.tag == "cell" || go.tag == "pointer")) {
          CellBehavior cb = go.GetComponent<CellBehavior>();
          if (lastCell == j - 1) {
            if (lastCellBehaviour.owningArray == null) {
              // must create
              lastCellBehaviour.owningArray = new CellArray();
              lastCellBehaviour.owningArray.Add(lastCellBehaviour.gameObject);
              lastCellBehaviour.arrayIndex = 0;
            }
            cb.owningArray = lastCellBehaviour.owningArray;
            cb.owningArray.Add(go);
            cb.arrayIndex = lastCellBehaviour.arrayIndex + 1;
          }
          lastCell = j;
          lastCellBehaviour = cb;
        }
      }
    }

    loadObjects();

    offset += height;
    int links = Convert.ToInt32(lines[offset++]);
    for (int i = 0; i < links; i++) {
      string[] lineLinks = Regex.Split(lines[offset + i], "[\\s,]+");
      int x1 = Convert.ToInt32(lineLinks[0]);
      int y1 = Convert.ToInt32(lineLinks[1]);
      int x2 = Convert.ToInt32(lineLinks[2]);
      int y2 = Convert.ToInt32(lineLinks[3]);
      GameObject pointer = findAt(x1, y1);
      GameObject target = findAt(x2, y2);
      PointerController pc = pointer.GetComponent<PointerController>();
      CellBehavior cc = target.GetComponent<CellBehavior>();
      pc.Target = cc;
    }

    offset += links;
    for (int i = 0; i < height; i++) {
      int y = height - (i) - 1;
      string s = lines[i + offset];
      for (int j = 0; j < s.Length && j < width; j++) {
        int x = j;

        char c = s[j];

        if (c >= 'A' && c <= 'Z') {
          GameObject go = findAt(x, y);
          endLevelCondition.Add(new CellValueEndLevelCondition(go.GetComponentInChildren<Text>(), c.ToString()));
          go.GetComponent<CellController>().SetExpected(levelController.GetSprite(c.ToString()));
        }
      }
    }

    offset += height;
    int targetLinks = Convert.ToInt32(lines[offset++]);
    for (int i = 0; i < targetLinks; i++) {
      string[] lineLinks = Regex.Split(lines[offset + i], "[\\s,]+");
      int x1 = Convert.ToInt32(lineLinks[0]);
      int y1 = Convert.ToInt32(lineLinks[1]);
      int x2 = Convert.ToInt32(lineLinks[2]);
      int y2 = Convert.ToInt32(lineLinks[3]);
      GameObject pointer = findAt(x1, y1);
      GameObject target = findAt(x2, y2);
      PointerController pc = pointer.GetComponent<PointerController>();
      CellBehavior cc = target.GetComponent<CellBehavior>();
      endLevelCondition.Add(new LinkTargetEndLevelCondition(pc, cc));
    }
    offset += targetLinks;

    foreach (int[] blockedCell in blockedCells) {
      GameObject go = findAt(blockedCell[0], blockedCell[1]);
      if (go.tag == "cell" || go.tag == "pointer") {
        CellBehavior cc = go.GetComponent<CellBehavior>();
        cc.Blocked |= blockedCell[2];
      }
    }

    setDisplayTypeOnObjects();
    int remaining = lines.Length - offset;
    int tools = 0;
    bool code = false;
    for (int i = 0; i < remaining; i++) {
      string[] line = Regex.Split(lines[offset+i], "[\\s,]+");
      string keyword = line[0];
      if (keyword == "end") {
        code = false;
      }
      else if (code) {
        levelController.solutionCode.Add(lines[offset+i]+";");
      }
      else if (keyword == "begin") {
        code = true;
      }
      else if (keyword == "addy") {
        ampersand.ActiveTool = MakeTool(GetToolProto(line[1]));
        if (line.Length >= 3) {
          ampersand.InActiveTool = MakeTool(GetToolProto(line[2]));
        }
        tools += 1;
      }
      else if (keyword == "val") {
        star.ActiveTool = MakeTool(GetToolProto(line[1]));
        if (line.Length >= 3) {
          star.InActiveTool = MakeTool(GetToolProto(line[2]));
        }
        tools += 2;
      }
      else if (keyword == "par") {
        levelController.par = Int32.Parse(line[1]);
      }
    }

    if ((tools & 1) == 0) {
      if (world == 0) {
        ampersand.ActiveTool = MakeTool(valueTool);
      }
      else if (world >= 3) {
        ampersand.ActiveTool = MakeTool(pointerTool);
      }
      else {
        ampersand.ActiveTool = MakeTool(pointerTool);
      }
    }
    if ((tools & 2) == 0) {
      if (world == 0) {
        star.ActiveTool = MakeTool(valueTool);
      }
      else if (world >= 3) {
        star.ActiveTool = MakeTool(valueTool);
        star.InActiveTool = MakeTool(incrementTool);
      }
      else {
        star.ActiveTool = MakeTool(valueTool);
      }
    }

  }

  Tool GetToolProto(string name) {
    return GameObject.Find("/"+name).GetComponent<Tool>();
  }

  Tool MakeTool(Tool proto) {
    Tool made = Instantiate(proto);
    tools.Add(made);
    return made;
  }

	// Update is called once per frame
	void Update () {

	}
}
