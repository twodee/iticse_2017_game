using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConsoleController : MonoBehaviour {
  public Text text;
  LevelController levelController;
  private Text instructionText;

  // Use this for initialization
  void Start() {
    text = GetComponent<Text>();
    instructionText = GameObject.Find("/HUD/Instructions").GetComponent<Text>();

    text.text = "";
    levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();
  }

  // Update is called once per frame
  void Update() {
    text.text = levelController.Current.collected;
  }

  public void LevelStart() {
    text.text = "";
    instructionText.text = levelController.Current.instructions;
  }

  public void LevelEnd() {

  }
}