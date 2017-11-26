using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour {

	#region Variables

	// Audio
	[SerializeField]
	private AudioClip   worldMusic;
	[SerializeField]
	private AudioClip   chatter;
	[SerializeField]
	private AudioClip   worldVictoryMusic;
	[SerializeField]
	private AudioClip   worldVictorySFX;
	[SerializeField]
	private AudioClip   levelVictory;
	[SerializeField]
	private AudioClip   simpleButtonSFX;
	[SerializeField]

	private Animator anim;

	private int currentLevel;
	private int currentWorld;
	private int achievedLevel;
	private int achievedWorld;

	[SerializeField]
	private Text levelCompleteText;

	[SerializeField]
	public GameObject loadingPanel, LevelPopUpPanel, PanelPause, PanelInfo, PanelToggle, ButtonToggleCode, PanelSettings, PanelStory, backwardButton;
	[SerializeField]
	public GameObject PanelStory1, PanelStory2, PanelStory3;
	public int currentStoryPanel = 1;

	private string[] levelCompleteFeedback = { "Excellent!", "Good job!", "You did it!", "Fantastic!", "Very good!", "Superb!", "Splendid!"};

	LevelController levelController;

	//private LevelManager levelManager;
	#endregion

	#region Unity Event Functions
	void Awake()
	{
		Time.timeScale = 1;

		// Get saved world and level or assign initial world and level
		if (!(PlayerPrefs.HasKey("achievedWorld"))) 
		{
			PlayerPrefs.SetInt("achievedWorld", 1);
			PlayerPrefs.SetInt("achievedLevel", 1);
		}
		achievedWorld = PlayerPrefs.GetInt("achievedWorld");
		achievedLevel = PlayerPrefs.GetInt("achievedLevel");
		levelController = GameObject.Find("/TheLevel").GetComponent<LevelController>();

	}

	void Start()
	{
		
		anim = PanelToggle.GetComponent<Animator>();
		//disable it on start to stop it from playing the default animation
		anim.enabled = false;

	}

	#endregion

	#region Public Functions
	//Pauses the game and timescale
	public void pauseGame()     
	{
		Time.timeScale = 0.0001F; 
		PanelPause.SetActive(true);
	}

	//resumes the game from the paused state
	public void resumeGame()    
	{
		Time.timeScale = 1;  
		PanelPause.SetActive(false);
	}

	//Pauses the game and timescale
	public void showPanelInfo()     
	{
		Time.timeScale = 0.0001F; 
		PanelInfo.SetActive(true);
	}

	//resumes the game from the paused state
	public void closePanelInfo()    
	{
		Time.timeScale = 1;  
		PanelInfo.SetActive(false);
		SoundManager.instance.PlayBGMusic(worldMusic);
		SoundManager.instance.PlayBGChatter (chatter);
	}

  //Pauses the game and timescale
  public void showPanelSettings()     
  {
    Time.timeScale = 0.0001F; 
    PanelSettings.SetActive(true);
  }

  //resumes the game from the paused state
  public void closePanelSettings()    
  {
    Time.timeScale = 1;  
    PanelSettings.SetActive(false);
    SoundManager.instance.PlayBGMusic(worldMusic);
    SoundManager.instance.PlayBGChatter (chatter);
  }


	//goes to main menu
	public void goToMainMenu()
	{
		// loadingPanel.SetActive (true);
		SceneManager.LoadScene("mainMenu");
	}

	//restarts the level
	public void restartLevel()
	{
		GameObject.Find("TheLevel").GetComponent<LevelLoader>().ResetLevel();
	}

	public void forwardStoryPanel() {
		currentStoryPanel += 1;
		showStoryPanel ();
	}

	public void backwardStoryPanel() {
		currentStoryPanel -= 1;
		showStoryPanel ();
	}

	private void showStoryPanel() {
		// Shows game story panels 1 through 3 depending on what is selected
		Debug.Log(currentStoryPanel);
		switch (currentStoryPanel) {
		case 1:
			PanelStory1.SetActive (true);
			PanelStory2.SetActive (false);
			PanelStory3.SetActive (false);
			backwardButton.SetActive (true);
			break;
		case 2:
			PanelStory1.SetActive(false);
			PanelStory2.SetActive(true);
			PanelStory3.SetActive(false);
			break;
		case 3:
			PanelStory1.SetActive (false);
			PanelStory2.SetActive (false);
			PanelStory3.SetActive (true);
			break;
		case 4:
			// Show the PanelInfo
			PanelInfo.SetActive (true);
			PanelStory.SetActive (false);
			break;
		default:
			PanelStory1.SetActive (true);
			PanelStory2.SetActive(false);
			PanelStory3.SetActive(false);
			break;
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
		//AudioManager.instance.PlayNewMusic(worldMusic);
	}
		
	public void callNextLevel()
	{
		levelCompleteText.text = levelCompleteFeedback[Random.Range(0, levelCompleteFeedback.Length)];
		//StartCoroutine(displayFeedback());
		//AudioManager.instance.PlaySingle(false, levelVictory);
	}

	public void closePanel()
	{
	//	goToGeneric("Main");
	}

	#endregion

	#region Private Functions

	public void playButtonSound(){
		AudioManager.instance.PlaySingle(/*false, */simpleButtonSFX);
	}
		
	public void goToEULA()
	{
	}

	public void goToPrivacy()
	{
	}


	public void toggleCodePanel()
	{

    if (!PanelToggle.activeSelf)
		{
			// slide it out
			//enable the animator component
			anim.enabled = true;
			//play the Slidein animation
			anim.Play("PauseMenuSlideIn");

      PanelToggle.SetActive(true);

			//PanelToggle.transform.Translate.Ler
			// set rotation
			//ButtonToggleCode.transform.Rotate(180,0,0);
			//position = false;
		}
		else
		{
			//ButtonToggleCode.transform.Rotate(180,0,0);
			//position = true;
      PanelToggle.SetActive(false);
		}
	}
 
	#endregion
}