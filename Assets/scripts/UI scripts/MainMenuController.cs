using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	#region Variables

	// Audio
	[SerializeField]
	private AudioClip[] worldMusic;
	private AudioClip   worldVictoryMusic;
	[SerializeField]
	private AudioClip   worldVictorySFX;
	[SerializeField]
	private AudioClip   levelVictory;
	[SerializeField]
	private AudioClip   simpleButtonSFX;
	[SerializeField]

	private int currentLevel;
	private int currentWorld;
	private int achievedLevel;
	private int achievedWorld;

	[SerializeField]
	public GameObject PanelLevelSelect, loadingPanel;

	[SerializeField]
	public GameObject UICanvas;

	//private LevelManager levelManager;
	#endregion

	#region Unity Event Functions
	void Awake() {
    Time.timeScale = 1;

    // Get saved world and level or assign initial world and level
    if (!(PlayerPrefs.HasKey("achievedWorld"))) {
      PlayerPrefs.SetInt("achievedWorld", 1);
      PlayerPrefs.SetInt("achievedLevel", 1);
    }
    achievedWorld = PlayerPrefs.GetInt("achievedWorld");
    achievedLevel = PlayerPrefs.GetInt("achievedLevel");
    currentWorld = 1;
    currentLevel = 1;

    string sceneName = SceneManager.GetActiveScene().name;     // "level 0-1" for example

    if (PanelLevelSelect != null) {
      // Set all levels to disabled
      int levelCount = 1;
      for (int i = 0; i < PanelLevelSelect.transform.childCount; i++) {
        //Debug.Log("currentLevel is " + currentLevel);

        if (PanelLevelSelect.transform.GetChild(i).name.Contains("Level")) {
          if (currentLevel >= levelCount) {
            PanelLevelSelect.transform.GetChild(i).GetComponent<Button>().interactable = true;
            levelCount++;
          }
          else {
            PanelLevelSelect.transform.GetChild(i).GetComponent<Button>().interactable = false;
          }
        }		
      }
    }

	}

	void Start()
	{
	}

	#endregion

	#region Public Functions
	//restarts the level
	public void restartLevel()
	{
		GameObject.Find("TheLevel").GetComponent<LevelLoader>().ResetLevel();
	}
		
	public void goToLevelSelect()  
	{ 
		// Create the buttons on the screen
		int levelNum = 1;
		goToGeneric("Level");  
	}

	public void goToAbout()        {  goToGeneric("About");  }

	//goes to the world select screen
	public void goToGeneric(string panelName)
	{
		for (int i = 0; i < UICanvas.transform.childCount; i++)
		{
			if (UICanvas.transform.GetChild(i).name.Contains(panelName))
			{ UICanvas.transform.GetChild(i).gameObject.SetActive(true); }
			else { UICanvas.transform.GetChild(i).gameObject.SetActive(false); };
		}
	}

	//advances scene to the specified level
	public void goToLevel(Button buttonSelected)
	{
		string buttonName = buttonSelected.name;
		buttonName = buttonName.Remove(0,5);					// Left with "0-1"
		string[] sceneLocale = buttonName.Split ('-'); 

		currentWorld = int.Parse (sceneLocale[0]);
		currentLevel = int.Parse (sceneLocale[1]);

		//Debug.Log(currentWorld + " " + currentLevel);
		PlayerPrefs.SetInt("currentLevel", currentLevel);
		PlayerPrefs.SetInt("currentWorld", currentWorld);
		SceneManager.LoadScene("FromLevelLoadFile");
	}
		
	public void closePanel()
	{
		goToGeneric("Main");
	}

	#endregion

	#region Private Functions

	public void playButtonSound(){
		AudioManager.instance.PlaySingle(false, simpleButtonSFX);
	}
		
	public void goToEULA()
	{
	}

	public void goToPrivacy()
	{
	}

	#endregion
}