using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConsoleController : MonoBehaviour {
  public Text text;
  LevelController levelController;
	private Text instructionText, instructionTextOld, instructionHeaderText, statusText;
 // private InputField statusText;
  public Text code;
  private Text scoreText;
  private int instructionCount;
  private GameObject panelAllCode;

  private ArrayList allCode;
	private string[] positiveAffirmations = new string[] {"Great work!", "Excellent!", "Order Up!", "Smooth Server!"};

  public bool condensedCode;
	public GameObject PanelPopUp, HeaderPopUp, BodyPopUp; 

  // Use this for initialization
  void Start() {
    text = GetComponent<Text>();
   // instructionTextOld = GameObject.Find("/canvasHUD2/Instructions").GetComponent<Text>();
	instructionHeaderText = GameObject.Find("/canvasHUD2/PanelInfo/Panel/TextHeader").GetComponent<Text>();
	instructionText = GameObject.Find("/canvasHUD2/PanelInfo/Panel/TextBody").GetComponent<Text>();

    statusText = GameObject.Find("/canvasHUD2/PanelHUD/PanelCode/Text").GetComponent<Text>();
    panelAllCode = GameObject.Find("/canvasHUD2/PanelHUD/PanelAllCode");
    allCode = new ArrayList();
    code = GameObject.Find("/canvasHUD2/PanelHUD/PanelAllCode/Text").GetComponent<Text>();
    scoreText = GameObject.Find("/canvasHUD2/PanelHUD/Score").GetComponent<Text>();
    text.text = "";
    statusText.text = "";
    code.text = "";
    instructionCount = 0;


    condensedCode = false;

    levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();
  }

  void updateScoreText() {
    scoreText.text = instructionCount.ToString() + " / " + levelController.Current.par.ToString();
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
    code.text = "";
    instructionCount = 0;
		instructionHeaderText.text = "World " + (levelController.Current.world+1) + ", Level " + (levelController.Current.level+1);

    instructionText.text = levelController.Current.instructions;
	//	instructionTextOld.text = instructionText.text;
    panelAllCode.SetActive(false);
    allCode.Clear();
    updateScoreText();

  }

  public void LevelEnd() {
		PanelPopUp.SetActive(true);
		HeaderPopUp.GetComponent<Text>().text = positiveAffirmations[Random.Range(0,positiveAffirmations.Length)];
		instructionText.text = ""; //levelController.Current.world.ToString() + "-"
   			 //+ levelController.Current.level.ToString() + ": Press = for next level!";

    if (condensedCode) {
      // condense
      int count = allCode.Count;
      for (int i = 0; i < count; i++) {
        string s = (string)allCode[i];
        if (i + 1 < count) {
          string next = (string)allCode[i + 1];
          if (s.IndexOf('=') > 0 && next.IndexOf('=') > 0) {
            string[] sParts = s.Split('=');
            string[] nextParts = next.Split('=');
            string sTrimmed = sParts[0].Trim();
            string nextTrimmed = nextParts[1].Trim().TrimEnd(';');
            if (sTrimmed == nextTrimmed) {
              s = nextParts[0] + "=" + sParts[1];
              i++;
            }
          }
        }
        code.text += s + "\n";
      }
    }
    else {
      foreach (string s in allCode) {
        code.text += s + "\n";
      }
    }
    panelAllCode.SetActive(true);
  }

  public void Status(string text) {
    statusText.text = text;
    allCode.Add(text);
    instructionCount++;
    updateScoreText();
  }
}