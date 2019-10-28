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
  private GameObject levelButton, worldPanel, worldText;

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
		int worldY = 0;
		GameObject[] worldPanels = new GameObject[worlds.Count];
		for (int i=0; i<worlds.Count; i++)
		{
			// Create a panel
			worldPanels[i] = Instantiate(worldPanel) as GameObject;
			worldPanels[i].transform.SetParent(ScrollViewContent.transform, false);

			GameObject newText = Instantiate(worldText) as GameObject;
			newText.transform.SetParent(worldPanels[i].transform, false);
			newText.name = "worldText"+i;
			newText.GetComponent<Text>().text = "World " + (i+1);
			worldY = 50-(100*i);
			Vector2	newTextPos = new Vector2(-375,0);
			newText.transform.localPosition = newTextPos;  

			for (int j=0; j<worlds[i]; j++)
			{
				GameObject newButton = Instantiate(levelButton) as GameObject;
				newButton.transform.SetParent(worldPanels[i].transform, false);
				newButton.name = "level" + i + "-" + j; // levels[j].ToString(); This alwasy set world to 0, so not sure why
				newButton.GetComponentInChildren<Text>().text = (j+1)+"";
				Vector2	newButtonPos = new Vector2(175*j-200,0);
				newButton.transform.localPosition = newButtonPos;
			}
		}
	}
}


