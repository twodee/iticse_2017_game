using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LevelLoader : MonoBehaviour {
  public GameObject floor_platform;
  public GameObject linkedCell;
  public GameObject cell;
  public GameObject pointerCell;

  private AmpersandController ampersand;
  private StarController star;

  // Use this for initialization
  void Start () {
    ampersand = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();
    star = GameObject.Find("/players/star").GetComponent<StarController>();

    LoadMap();
  }

  void EmptyMap() {
    // Find all of our children and...eliminate them.

    while (transform.childCount > 0) {
      Transform c = transform.GetChild(0);
      c.SetParent(null); // become Batman
      Destroy(c.gameObject); // become The Joker
    }
  }

  void LoadAllLevelNames() {
    // Read the list of files from StreamingAssets/Levels/*.png
    // The player will progess through the levels alphabetically
  }

  void LoadMap() {
    EmptyMap();

    // Read the data from the file in assets
    string filePath = Application.dataPath + "/levels/level1.txt";

    string text = System.IO.File.ReadAllText(filePath);
    string[] lines = Regex.Split(text, "\n");

    int width = Convert.ToInt32(lines[0]);
    int height = Convert.ToInt32(lines[1]);
    

    for (int i = 2; i < 2 + height; i++) {
      float y = height - (10 * (float)(i - 1) / (float)height) + 0.5f;
      string s = lines[i];
      for (int j = 0; j < s.Length; j++) {
        float x = j + 0.5f;

        char c = s[j];
        Vector3 pos = new Vector3(x, y, 0);

        if (c == '-') {
          GameObject go = (GameObject)Instantiate(floor_platform, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == 'P') {
          GameObject go = (GameObject)Instantiate(pointerCell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
        }
        else if (c == 'L') {
          GameObject go = (GameObject)Instantiate(linkedCell, pos, Quaternion.identity);
          go.transform.SetParent(this.transform);
          j++;
        }
        else if (c == 'C') {
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
