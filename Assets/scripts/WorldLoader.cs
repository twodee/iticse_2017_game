using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldLoader : MonoBehaviour {
  public ArrayList levels; // string
  public Dictionary<string, TextAsset> levelAssets;
  public Dictionary<int, int> worlds;

  void Awake() {
    levelAssets = new Dictionary<string, TextAsset>();
    worlds = new Dictionary<int, int>();
    LoadWorlds();
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
    int world = 0;
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
}


