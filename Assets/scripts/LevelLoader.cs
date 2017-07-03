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

  private LevelController progressController;
  private ConsoleController consoleController;

  private ArrayList levels;
  private int currentLevel;

  private Dictionary<long, GameObject> objects;

  // Use this for initialization
  void Awake() {
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
    star = GameObject.Find("/players/star").GetComponent<StarController>();
    progressController = gameObject.GetComponent<LevelController>();
    consoleController = GameObject.Find("HUD/Console").GetComponent<ConsoleController>();
    objects = new Dictionary<long, GameObject>();
  }

  void Start () {
    LoadAllLevelNames();
    currentLevel = PlayerPrefs.GetInt("currentLevel")-2;
//    currentLevel = 3;
    LoadNextLevel();
  }

  void EmptyLevel() {
    // Find all of our children and...eliminate them.
    objects.Clear();

    while (transform.childCount > 0) {
      Transform c = transform.GetChild(0);
      c.SetParent(null); // become Batman
      Destroy(c.gameObject); // become The Joker
    }
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

  GameObject findAt(int x, int y) {
    return objects[Key(x, y)];
  }

  void loadObjects() {
    for (int i = 0; i < transform.childCount; i++) {
      Transform c = transform.GetChild(i);
      if (c.gameObject.tag != "ground") {
        objects[Key((int)c.gameObject.transform.position.x, (int)c.gameObject.transform.position.y)] = c.gameObject;
      }
    }
  }

  void LoadAllLevelNames() {
    levels = new ArrayList();
    string[] fileEntries = Directory.GetFiles(Application.dataPath + "/Resources/levels/");
    foreach (string file in fileEntries) {
      if (file.EndsWith(".txt")) {
        int start = Application.dataPath.Length+"/Resources/".Length;
        levels.Add(file.Substring(start, file.Length-start-4));
      }
    }
    levels.Sort();
  }

  public void LoadNextLevel() {
    currentLevel++;
    if (currentLevel == levels.Count) {
      currentLevel = 0;
    }
    LoadLevel(currentLevel);
    PlayerPrefs.SetInt("currentLevel", currentLevel+1);
  }

  void LoadLevel(int index) {
    EmptyLevel();

    // Read the data from the file in assets
    string filePath = (string)levels[index];
    TextAsset textFile = Resources.Load(filePath) as TextAsset;
    string text = textFile.text;

    Regex replaceComment = new Regex("[ ]*;.*\n");
    Regex replaceLine = new Regex("[\n]*\n");
    Regex replaceBegin = new Regex("^\n");
    text = replaceComment.Replace(text, "\n");
    text = replaceLine.Replace(text, "\n");
    text = replaceBegin.Replace(text, "");
    string[] lines = Regex.Split(text, "\n");

    int width = Convert.ToInt32(lines[0]);
    int height = Convert.ToInt32(lines[1]);
    Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    cam.transform.position = new Vector3((float)Convert.ToDouble(lines[2]), (float)Convert.ToDouble(lines[3]), -WORLD_HEIGHT);

    AndAllEndLevelCondition endLevelCondition = new AndAllEndLevelCondition();
    progressController.Current = new Level(filePath, lines[4].Trim(), endLevelCondition);
    endLevelCondition.Add(new CollectEndLevelCondition(progressController.Current, lines[5].Trim()));
    int offset = 6;
    ArrayList blockedCells = new ArrayList();

    for (int i = 0; i < height; i++) {
      float y = height - (i) - 1 + 0.5f;
      string s = lines[i + offset];
      for (int j = 0; j < s.Length && j < width; j++) {
        float x = j + 0.5f;

        char c = s[j];
        Vector3 pos = new Vector3(x, y, 0);

        if (c >= 'A' && c <= 'Z') {
          GameObject go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          Text t = go.GetComponentInChildren<Text>();
          t.text = c.ToString();
          CellController cc = go.GetComponent<CellController>();
          if (y < MIDDLE_BAR_Y) {
            cc.immutable = true;
          }
        }
        else if (c >= 'a' && c <= 'z') {
          GameObject go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          Text t = go.GetComponentInChildren<Text>();
          t.text = "";//c.ToString().ToUpper();
          t.enabled = false;
        }
        else if (c == '▔') {
          blockedCells.Add(new int[]{ (int)x, (int)y + 1, CellController.DOWN });
        }
        else if (c == '▁') {
          blockedCells.Add(new int[]{ (int)x, (int)y - 1, CellController.UP });
        }
        else if (c == '-') {
          GameObject go = (GameObject)Instantiate(cabinet, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '+') {
          GameObject go = (GameObject)Instantiate(counter, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '.') {
          GameObject go = (GameObject)Instantiate(pointerCell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '@') {
          GameObject go = (GameObject)Instantiate(linkedCell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          j++;
        }
        else if (c == '#') {
          GameObject go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == '&') {
          ampersand.transform.position = pos;
        }
        else if (c == '*') {
          star.transform.position = pos;
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
      CellController cc = target.GetComponent<CellController>();
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
      CellController cc = target.GetComponent<CellController>();
      endLevelCondition.Add(new LinkTargetEndLevelCondition(pc, cc));
    }

    foreach (int[] blockedCell in blockedCells) {
      GameObject go = findAt(blockedCell[0], blockedCell[1]);
      CellController cc = go.GetComponent<CellController>();
      cc.Blocked |= blockedCell[2];
    }
  }

	// Update is called once per frame
	void Update () {

	}
}
