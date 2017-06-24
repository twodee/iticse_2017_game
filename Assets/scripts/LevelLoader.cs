using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {
  public GameObject floor_platform;
  public GameObject linkedCell;
  public GameObject cell;
  public GameObject pointerCell;

  private AmpersandController ampersand;
  private StarController star;

  private LevelController progressController;

  private ArrayList levels;
  private int currentLevel;

  // Use this for initialization
  void Start () {
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
    star = GameObject.Find("/players/star").GetComponent<StarController>();
    progressController = gameObject.GetComponent<LevelController>();
    LoadAllLevelNames();
    currentLevel = 0;
    LoadNextLevel();
  }

  void EmptyLevel() {
    // Find all of our children and...eliminate them.

    while (transform.childCount > 0) {
      Transform c = transform.GetChild(0);
      c.SetParent(null); // become Batman
      Destroy(c.gameObject); // become The Joker
    }
  }

  void LoadAllLevelNames() {
    levels = new ArrayList();
    string[] fileEntries = Directory.GetFiles(Application.dataPath + "/levels/");
    foreach (string file in fileEntries) {
      if (file.EndsWith(".txt")) {
        levels.Add(file);
      }
    }
    levels.Sort();
  }

  public void LoadNextLevel() {
    LoadLevel(currentLevel++);
  }

  void LoadLevel(int index) {
    EmptyLevel();

    // Read the data from the file in assets
    string filePath = (string)levels[index];
    string text = System.IO.File.ReadAllText(filePath);
    string[] lines = Regex.Split(text, "\n");

    int width = Convert.ToInt32(lines[0]);
    int height = Convert.ToInt32(lines[1]);
    Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    cam.transform.position = new Vector3(Convert.ToInt32(lines[2]), Convert.ToInt32(lines[3]), -10);

    progressController.Current = new Level(filePath, lines[4].Trim(), lines[5].Trim());
    int offset = 6;

    for (int i = 0; i < height; i++) {
      float y = height - (10 * (float)(i + 1) / (float)height) + 0.5f;
      string s = lines[i+offset];
      for (int j = 0; j < s.Length && j < width; j++) {
        float x = j + 0.5f;

        char c = s[j];
        Vector3 pos = new Vector3(x, y, 0);

        if (c >= 'A' && c <= 'Z') {
          GameObject go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          Text t = go.GetComponentInChildren<Text>();
          t.text = c.ToString();
        }
        else if (c >= 'a' && c <= 'z') {
          GameObject go = (GameObject)Instantiate(cell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          Text t = go.GetComponentInChildren<Text>();
          t.text = c.ToString().ToUpper();
          t.enabled = false;
        }
        else if (c == '-') {
          GameObject go = (GameObject)Instantiate(floor_platform, pos, Quaternion.identity);
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
  }

	// Update is called once per frame
	void Update () {
		
	}
}
