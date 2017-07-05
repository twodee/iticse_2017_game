using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConsoleController : MonoBehaviour {
  public Text text;
  LevelController levelController;
  private Text instructionText;
  private Text statusText;

  // Use this for initialization
  void Start() {
    text = GetComponent<Text>();
    instructionText = GameObject.Find("/canvasHUD2/Instructions").GetComponent<Text>();
    statusText = GameObject.Find("/HUD/Status").GetComponent<Text>();
    text.text = "";
    statusText.text = "";

    levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();
  }

  // Update is called once per frame
  void Update() {
    if (levelController.Current != null) {
      text.text = levelController.Current.collected;
    }
  }

  public void LevelStart() {
    text.text = "";
    statusText.text = "";
    instructionText.text = levelController.Current.instructions;
  }

  public void LevelEnd() {

  }

  public void Status(string text) {
    statusText.text = text;
  }
}