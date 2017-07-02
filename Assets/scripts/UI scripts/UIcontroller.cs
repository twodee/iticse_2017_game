using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIcontroller : MonoBehaviour {

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

	[SerializeField]
	private Text levelCompleteText;

	[SerializeField]
	public GameObject PanelWorldSelect, PanelLevelSelect, loadingPanel, LevelPopUpPanel;

	[SerializeField]
	public GameObject UICanvas;

	private string[] levelCompleteFeedback = { "Excellent!", "Good job!", "You did it!", "Fantastic!", "Very good!", "Superb!", "Splendid!"};

	//private LevelManager levelManager;
	#endregion

	#region Unity Event Functions
	void Awake()
	{
		Time.timeScale = 1;

		// Get saved world and level or assign initial world and level
		if (!(PlayerPrefs.HasKey("currentWorld"))) 
		{
			PlayerPrefs.SetInt("currentWorld", 1);
			PlayerPrefs.SetInt("currentLevel", 1);
		}
		currentWorld = PlayerPrefs.GetInt("currentWorld");
		currentLevel = PlayerPrefs.GetInt("currentLevel");

		string sceneName = SceneManager.GetActiveScene().name;     // "level 0-1" for example

		// Set all levels to disabled
		int levelCount = 1;
		for (int i = 0; i < PanelLevelSelect.transform.childCount; i++)
		{
			Debug.Log("currentLevel is " + currentLevel);

			if (PanelLevelSelect.transform.GetChild(i).name.Contains("Level"))
			{
				if (currentLevel >= levelCount)
				{ PanelLevelSelect.transform.GetChild(i).GetComponent<Button>().interactable=true; levelCount++;}
				else
				{ PanelLevelSelect.transform.GetChild(i).GetComponent<Button>().interactable=false; }
			}		
		}


			
		/*
		if (sceneName.Substring(0,5).Equals("world"))
		{
			// Debug.Log("world is " + sceneName.Substring(6,1));
			currentWorld = int.Parse(sceneName.Substring(6,1));
			currentLevel = 99;
		}
		else if (sceneName.Substring(0,5).Equals("level"))
		{
			sceneName = sceneName.Remove(0,5);					// Left with "0-1"
			string[] sceneLocale = sceneName.Split ('-'); 
			currentWorld = int.Parse (sceneLocale[0]);
			currentLevel = int.Parse (sceneLocale[1]);
			string temp;
			switch (currentWorld)
			{	
			case 1: temp = "Colored Shapes"; break;
			case 2: temp = "Numbers"; break;
			case 3: temp = "Letters/Words"; break;
			case 4: temp = "Something 4"; break;
			default: temp = "World " + currentWorld.ToString(); break;
			}
		}
		else
		{
			currentWorld = 99;
			currentLevel = 99;
		}
		*/
	}

	void Start()
	{
		string sceneName = SceneManager.GetActiveScene().name;     // "level 0-1" for example

		if (sceneName.Substring(0,5).Equals("level"))
		{
			sceneName = sceneName.Remove(0,5);					// Left with "0-1"
			string[] sceneLocale = sceneName.Split ('-'); 
			currentWorld = int.Parse (sceneLocale[0]);
			currentLevel = int.Parse (sceneLocale[1]);
			AudioClip clipToPlay;

			switch(currentWorld)
			{
			case 1: clipToPlay = worldMusic[0]; break;
			case 2: clipToPlay = worldMusic[1]; break;
			case 3: clipToPlay = worldMusic[2]; break;
			case 4: clipToPlay = worldMusic[3]; break;
			case 5: clipToPlay = worldMusic[4]; break;
			case 6: clipToPlay = worldMusic[5]; break;
			default: clipToPlay = worldMusic[0]; break;
			}
			AudioManager.instance.PlayNewMusic(clipToPlay);
		}
		else
		{
			AudioManager.instance.PlaySingle(false, worldVictorySFX);
			AudioManager.instance.PlayNewMusic(worldVictoryMusic);
		}

	}

	#endregion

	#region Public Functions
	//Pauses the game and timescale
	public void pauseGame()
	{
		Time.timeScale = 0.0001F;
	}

	//resumes the game from the paused state
	public void resumeGame()
	{
		Time.timeScale = 1;
	}

	//goes to main menu
	public void goToMainMenu()
	{
		loadingPanel.SetActive (true);
		SceneManager.LoadScene("mainMenu");
	}

	//restarts the level
	public void restartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void goToWorldSelect()
	{
		goToGeneric("World");
	}

	public void goToLevelSelect()
	{
		goToGeneric("Level");
	}

	public void goToAbout()
	{
		goToGeneric("About");
	}

	//goes to the world select screen
	public void goToGeneric(string panelName)
	{
		for (int i = 0; i < UICanvas.transform.childCount; i++)
		{
			Debug.Log("i is " + i);
			if (UICanvas.transform.GetChild(i).name.Contains(panelName))
			{ UICanvas.transform.GetChild(i).gameObject.SetActive(true); }
			else { UICanvas.transform.GetChild(i).gameObject.SetActive(false); };
		}
		//SceneManager.LoadScene("mainMenu");
	}

	//advances scene to the next level
	public void goToNextLevel()
	{
		StopAllCoroutines();
		SceneManager.LoadScene("FromLevelLoadFile");
		/*
		int nextLevel;
		int nextWorld;

		string sceneName = SceneManager.GetActiveScene().name;     // "level 0-1" for example

		if (sceneName.Substring(0,5).Equals("world"))
		{
			// We are in the world complete scene

			if (currentWorld == 7)
			{
				SceneManager.LoadScene("all worlds complete");
			}
			else 
			{
				nextWorld = currentWorld + 1;
				nextLevel = 1;
				// Debug.Log ("About to load world " + nextWorld + "-" + nextLevel);
				// SceneManager.LoadScene("level " + nextWorld + "-" + nextLevel);
				// SceneManager.LoadScene("level " + nextWorld + "-" + nextLevel);
				goToWorldSelect();
			}
		}
		else 
		{
			sceneName = sceneName.Remove(0,5);					// Left with "0-1"
			string[] sceneLocale = sceneName.Split ('-'); 

			currentWorld = int.Parse (sceneLocale[0]);
			currentLevel = int.Parse (sceneLocale[1]);

			if (currentLevel <= 14)
			{
				nextWorld = currentWorld;
				nextLevel = currentLevel+1;
				SceneManager.LoadScene("level " + nextWorld + "-" + nextLevel);
			}
			else if (currentLevel == 15)
			{

				if (currentWorld == 7)
				{
					SceneManager.LoadScene("all worlds complete");
				}
				else
				{
					SceneManager.LoadScene("world " + currentWorld + " complete");
					PlayerPrefs.SetInt ("newWorldUnlocked", 1); // 1 = true
				}
			}
		}
		*/
	}


	public void callNextLevel()
	{
		levelCompleteText.text = levelCompleteFeedback[Random.Range(0, levelCompleteFeedback.Length)];
		//StartCoroutine(displayFeedback());
		AudioManager.instance.PlaySingle(false, levelVictory);
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