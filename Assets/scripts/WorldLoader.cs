using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorldLoader : MonoBehaviour {
  public ArrayList levels; // string
  public Dictionary<string, TextAsset> levelAssets;
  public Dictionary<int, int> worlds;
	private int world;

  [SerializeField]
  private GameObject levelButton, worldText;

  [SerializeField]
	public GameObject ScrollViewContent;

  void Awake() {
    levelAssets = new Dictionary<string, TextAsset>();
    worlds = new Dictionary<int, int>();
    LoadWorlds();
    if (ScrollViewContent != null) {
	    LoadLevelSelect();
    }
  }

  // translates from 2-3 to 13 if there are 5 sublevels per world starting at 0
  public int GetIndex(int world, int sublevel) {
    int level = sublevel;
    for (int i = 0; i < world; i++) {
      level += worlds[i];
    }
    return level;
  }

  void LoadWorlds() {
    levels = new ArrayList();
    world = 0;
    int level = 0;
    bool hasMoreWorlds = true;
    while (hasMoreWorlds) {
      bool hasMoreLevels = true;
      level = 0;
      int levelsThisWorld = 0;
      while (hasMoreLevels) {
        string name = "level" + world + "-" + level;
        TextAsset ta = Resources.Load<TextAsset>("levels/"+name);
        if (ta == null) {
          hasMoreLevels = false;
        }
        else {
          levels.Add(name);
          levelAssets[name] = ta;
          levelsThisWorld++;
          level++;
        }
      }
      if (level == 0) {
        hasMoreWorlds = false;
      }
      else {
        worlds[world] = levelsThisWorld;
        world++;
      }
    }
  }

	// This loads (dynamically) the grid on the level select screen.
	public void LoadLevelSelect() {
		int level = 0;
		int worldY = 0;
		for (int i=0; i<worlds.Count-1; i++)
		{
			GameObject newText = Instantiate(worldText) as GameObject;
			newText.transform.SetParent(ScrollViewContent.transform, false);
			newText.name = "worldText"+i;
			newText.GetComponent<Text>().text = "World " + (i+1);
			worldY = 50-(100*i);
			Vector2	newTextPos = new Vector2(0,worldY);
			newText.transform.position = Camera.main.ScreenToWorldPoint(newTextPos);  

			for (int j=0; j<worlds[i]; j++)
			{
				//string name = "level" + i + "-" + level;
				GameObject newButton = Instantiate(levelButton) as GameObject;
				newButton.transform.SetParent(ScrollViewContent.transform, false);
				newButton.name = levels[j].ToString();
				newButton.GetComponentInChildren<Text>().text = (j+1)+"";
				Vector2	newButtonPos = new Vector2(100*(j+1),worldY);
				newButton.transform.position = Camera.main.ScreenToWorldPoint(newButtonPos);
			}
		}
	}
}


