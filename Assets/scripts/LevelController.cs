using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour {
  private Level current;
  private ConsoleController consoleController;
  private LevelLoader loader;

  private PlayerController player1;
  private PlayerController player2;


  // Use this for initialization
   void Awake() {
    consoleController = GameObject.Find("/HUD/Console").GetComponent<ConsoleController>();
    player1 = GameObject.Find("/players/star").GetComponent<StarController>();
    player2 = GameObject.Find("/players/ampersand").GetComponent<AmpersandController>();

    loader = gameObject.GetComponent<LevelLoader>();
	}
	
	// Update is called once per frame
	void Update() {
    if (Input.GetButtonDown("NextLevel")) {
      loader.LoadNextLevel();
    }
	}

  public void OnCollect(string loot) {
    current.collected += loot;
    CheckProgress();
  }

  public void OnTransmit() {
    CheckProgress();
  }

  public void OnAttach() {
    CheckProgress();
  }

  public void CheckProgress() {
    if (current.endLevelCondition.Check()) {
      loader.LoadNextLevel();
    }
  }

  public Level Current {
    get {
      return current;
    }
    set {
      if (current != null) {
        player1.LevelEnd();
        player2.LevelEnd();
        consoleController.LevelEnd();
      }
      current = value;
      player1.LevelStart();
      player2.LevelStart();
      consoleController.LevelStart();
    }
  }
}
