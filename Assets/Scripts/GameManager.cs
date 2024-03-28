using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;
	public float FadeInTime = 1.0f;
	public float FadeOutTime = 1.0f;
	public PlayfieldCreator prefab;
	public TurnKeeper tk;
	public ScoreKeeper sk;
	public GameObject[] backgrounds;

	// settings game objects (buttons in panel)
	public GameObject soundButton;
	public GameObject sfxButton;
	public GameObject[] playMode;
	public GameObject yellowVictory;
	public GameObject purpleVictory;
	public GameObject drawVictory;

	public GameObject KingXContainer;
	public GameObject KingOContainer;
	public GameObject LoadLevelObj;
	public GameObject menuButton;
	private PlayfieldCreator currentPC;
	private PlayfieldCreator prevPC;
	private int currentWinner = -1;
	private int prevWinner = -1;
	GameObject victory = null;
	public  GameObject[] sounds;
	public  GameObject[] sfx;
	private bool sfxEnabled = true;
	private bool soundEnabled = true;
	private GameObject currentBackground;
	public float newGameTransitionTime = 5.0f;
	private int levelCounter = 2;
	public bool debugWin = false;
	public bool winTransition = false;
	public static bool singlePlayer = true;
	public LoadGame audioManager;
	public int[] LevelDifficultyStart = new int[]{ 40, 60, 80 };
	// start range likelyhood that AI will do the correct move.
	public int[] LevelDifficultyEnd = new int[]{ 80, 90, 100 };
	// end range    --- || --
	public int[] LevelDontFollowPlayer = new int[]{ 20, 10, 0 };
	// likelyhood in percent that AI will not follow player

	public int LevelDifficultyIndex = 1;
	// between 0 and 2. 0 easy, 1 medium 2 hard.
	private int levelGameDifficulty;
	// increases by one per level the player plays no maximum (will be throttled at 100% in PlayField)
	private Color buttonOriginalColor;
	private int started = 0;
	private int numberOfGamesPlayed = 0;
	private int wonAgainstAI = 0;
	private bool showStartedRate = false;
	private bool[] showCredits = new bool[2] { false, false };
	public Button activeButton;
	public UnityEngine.EventSystems.EventSystem system;
	void Awake ()
	{

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);    

		soundEnabled = true;
		sfxEnabled = true;
		if (PlayerPrefs.HasKey ("music")) {
			soundEnabled = PlayerPrefs.GetInt ("music") == 1 ? true : false;
			 
		} 

		if (PlayerPrefs.HasKey ("sfx")) {
			
			sfxEnabled = PlayerPrefs.GetInt ("sfx") == 1 ? true : false;
		} 


		if (PlayerPrefs.HasKey ("started") == false) {
			PlayerPrefs.SetInt ("started", 0);
			started = 1;
		} else {
			started = PlayerPrefs.GetInt ("started");
			started++;
			SavePrefs ();
		}
		if (PlayerPrefs.HasKey ("playedRateShown") == false) {
			PlayerPrefs.SetInt ("playedRateShown", 0);

		}
		if (PlayerPrefs.HasKey ("wonAIRateShown") == false) {
			PlayerPrefs.SetInt ("wonAIRateShown", 0);
		}

	}

	public bool ShowCredits ()
	{
		if (showCredits [0] && showCredits [1])
			return true;
		return false;
	}

	public void RegiserCredits (int index)
	{
		showCredits [index] = true;
	}

	public void SetLevelHard ()
	{
		LevelDifficultyIndex = 2;
		SetMenuState ();
	}

	public void SetLevelMedium ()
	{
		LevelDifficultyIndex = 1;
		SetMenuState ();
	}

	public void SetLevelEasy ()
	{
		LevelDifficultyIndex = 0;
		SetMenuState ();
	}

	public int GetLevelDifficultyStart ()
	{
		return LevelDifficultyStart [LevelDifficultyIndex];
	}

	public int GetLevelDifficulltyEnd ()
	{
		return LevelDifficultyEnd [LevelDifficultyIndex];
	}

	public int GetLevelDontFollow ()
	{
		return LevelDontFollowPlayer [LevelDifficultyIndex];
	}

	public void SavePrefs ()
	{
		SavePrefs (soundEnabled, sfxEnabled);	
	}

	public void SetAI ()
	{
		tk.SetAI ();
	}

	void SavePrefs (bool music, bool sfx)
	{
		PlayerPrefs.SetInt ("music", music == true ? 1 : 0);
		PlayerPrefs.SetInt ("sfx", sfx == true ? 1 : 0);
		PlayerPrefs.SetInt ("level", LevelDifficultyIndex);
		PlayerPrefs.SetInt ("started", started);
		PlayerPrefs.Save ();
	}

	void SetButtonState (GameObject obj, bool active)
	{
		int activeIndex = active == true ? 0 : 1;
		int inactiveIndex = active == true ? 1 : 0;
		Image img = obj.transform.GetChild (inactiveIndex).gameObject.GetComponent<Image> ();
		img.enabled = false;
		img = obj.transform.GetChild (activeIndex).gameObject.GetComponent<Image> ();
		img.enabled = true;
	}

	public void SetPlayModeButton (GameObject obj, bool active)
	{
		Image img = obj.GetComponent<Image> ();
		if (active) {
			img.color = new Color (1, 1, 1);
		} else {
			img.color = buttonOriginalColor;
		}
	}

	public void SetMenuState ()
	{
		
		SetButtonState (soundButton, soundEnabled);
		SetButtonState (sfxButton, sfxEnabled);
		bool[] b = new bool[] {
			LevelDifficultyIndex == 0 ? true : false,
			LevelDifficultyIndex == 1 ? true : false,
			LevelDifficultyIndex == 2 ? true : false
		};
		int i = 0;
		foreach (GameObject obj in playMode) {
			SetPlayModeButton (obj, b [i++]);
		}
		if (currentPC != null) {
			currentPC.SetLevelDifficulty (LevelDifficultyIndex);
		} else {
			Debug.Log ("WTF!");
		}

		SavePrefs (soundEnabled, sfxEnabled);

	}

	public void ReturnToMainMenu ()
	{
		SavePrefs ();
		LoadLevelObj.GetComponent<LoadGame> ().LoadMenu ();

	}


	// Use this for initialization
	void Start ()
	{
		Application.targetFrameRate = 60;
		Image img = playMode [0].GetComponent<Image> ();
		buttonOriginalColor = img.color;
		InitGame ();
		int size = backgrounds.Length;
	
		levelCounter = Random.Range (0, size - 1);
		backgrounds [levelCounter].SetActive (true);
		ShowRate ();
	}

	public bool SFXEnabled ()
	{
		return sfxEnabled;
	}

	public bool SoundEnabled ()
	{
		return soundEnabled;
	}

	public void InitGame ()
	{
		levelGameDifficulty = 0;
		victory = null;
		winTransition = false;
		if (currentPC) {
			DestroyImmediate (currentPC.gameObject);
		}
		currentPC = Instantiate (prefab);
		currentPC.enabled = true;
		currentPC.CreatePlayField (currentWinner, singlePlayer);
		soundEnabled = true;
		if (PlayerPrefs.HasKey ("level")) {
			LevelDifficultyIndex = PlayerPrefs.GetInt ("level");
		
		} else {
			LevelDifficultyIndex = 0;
			SavePrefs ();
		}
		if (PlayerPrefs.HasKey ("music")) {
			soundEnabled = PlayerPrefs.GetInt ("music") == 1 ? true : false;
			 
		} 
		if (PlayerPrefs.HasKey ("sfx")) {
			 
			sfxEnabled = PlayerPrefs.GetInt ("sfx") == 1 ? true : false;
		} else {
			sfxEnabled = true;
			soundEnabled = true;
			SavePrefs ();
		}

		SetSFX (sfxEnabled);
		SetSound (soundEnabled);
	}

	public void Restart ()
	{
		if (victory != null) {
			victory.SetActive (false);
		}
		levelGameDifficulty = 0;
		winTransition = false;
		currentWinner = -1;
		prevWinner = -1;
		tk.Restart ();
		sk.ResetScore ();
		InitGame ();
	}

	public int CurrentPlayer ()
	{
		return tk.getCurrentTurn ();
	}

	public bool WinAnimPlaying ()
	{
		return 	winTransition;
	}

	public void SetSFX (bool b)
	{
		foreach (GameObject obj in sfx) {
			if (obj != null)
				obj.GetComponent<AudioSource> ().enabled = b;
		}
	}

	public void SFX ()
	{ 
		sfxEnabled = !sfxEnabled;
		SetSFX (sfxEnabled);
		SetMenuState ();
	}

	void SetSound (bool b)
	{
		foreach (GameObject obj in sounds) {
			obj.SetActive (b);
			obj.GetComponent<AudioSource> ().enabled = b;
			if (b == true) {
				obj.GetComponent<AudioSource> ().volume = 1.0f; //0.4 works much better for the volume for the song.
				if (obj.GetComponent<AudioSource> ().enabled) {
					obj.GetComponent<AudioSource> ().Play ();
				}
			} else {
				obj.GetComponent<AudioSource> ().volume = 0.0f;
			}
		
		}
		SetMenuState ();

	}
	public bool IsMenuShowing() {
		return LoadLevelObj.GetComponent<ShowMenuOnTouch> ().isMenuShowing ();
	}
	public void ToggleMenu ()
	{	
		LoadLevelObj.GetComponent<ShowMenuOnTouch> ().ToggleMenu ();
		system.SetSelectedGameObject (activeButton.gameObject);
	}

	public void Sound ()
	{
		soundEnabled = !soundEnabled;
		SetSound (soundEnabled);
	}



	public void SetInteractionEnabled (bool status)
	{
		currentPC.SetIntercationEnabled (status);
		if (status == true && ShowCredits ()) {
			showCredits [0] = false;
			showCredits [1] = false;
		}
	}

	public void KissKing (int kingIndex)
	{
		GameObject kingContainer = kingIndex == 0 ? KingXContainer : KingOContainer;
		GameObject king = kingContainer.transform.GetChild (0).gameObject;
		king.GetComponent<CallTurnKeeperOnClick> ().Kiss ();
	}

	public void KingClicked (GameObject king)
	{
		if (winTransition == false) {
			tk.KingClicked (king);
		}
	}

	public void MuteCurrentSongForWin ()
	{
		winTransition = true;
		LevelSounds[] levelsound = backgrounds [levelCounter].GetComponentsInChildren<LevelSounds> ();
		sfx = levelsound [0].sfx;

		foreach (GameObject s in sounds) {
			AudioSource source = s.GetComponent<AudioSource> ();
			source.Stop ();
			//StartCoroutine (audioManager.FadeAudio (source, 0.5f, LoadGame.Fade.Out, false));
		}
		sounds = levelsound [0].sounds;
		SetSound (false);
	}

	public  void Rate ()
	{

		UniRate r = GameObject.FindObjectOfType<UniRate> ();
		r.ShowPrompt ();
	}

	public void ShowRate ()
	{

		if (singlePlayer == false && numberOfGamesPlayed == 3 && PlayerPrefs.GetInt ("playedRateShown") == 0) {
			PlayerPrefs.SetInt ("playedRateShown", 1);
			PlayerPrefs.Save ();
			Rate ();
			return;
		}
		if (singlePlayer == true && wonAgainstAI == 2 && PlayerPrefs.GetInt ("wonAIRateShown") == 0) {
			PlayerPrefs.SetInt ("wonAIRateShown", 1);
			PlayerPrefs.Save ();
			Rate ();
			return;
		}
		if ((started == 5 || started == 10) && showStartedRate == false) {
			showStartedRate = true;
			Rate ();
		}

	}

	public void NewGame (int winner)
	{
		if (singlePlayer == false) {
			numberOfGamesPlayed++;
		}
		if (singlePlayer == true && winner == 11) {
			wonAgainstAI++;
		}

		menuButton.SetActive (false);
		StartCoroutine (NewGameEnumerator (winner));
	}

	int SetBackground (int oldbackround, int newB)
	{
		backgrounds [oldbackround].SetActive (false);
		if (newB == -1) {
			
			int newbackground = oldbackround + 1;
	
			if (newbackground >= backgrounds.Length) {
				newbackground = 0;
			}
			backgrounds [newbackground].SetActive (true);
			return newbackground;
		} else {
			backgrounds [newB].SetActive (true);
		}
		return -1;
	}

	public IEnumerator NewGameEnumerator (int winner)
	{	
		yield return new WaitForSeconds (2);
		levelCounter = SetBackground (levelCounter, -1);
		if (winner == 12) {
			int bajs = tk.getCurrentTurn ();
			currentWinner = 12;
			//currentWinner = -1;
			victory = drawVictory;
			victory.SetActive (true);

		} else {
			currentWinner = (winner == 11 ? 1 : 0);
			victory = currentWinner == 1 ? yellowVictory : purpleVictory;
			victory.SetActive (true);
			tk.ActivateVictory (currentWinner);
			GameObject winnerKing = (winner == 11 ? KingXContainer : KingOContainer);
			GameObject looserKing = (winner == 11 ? KingOContainer : KingXContainer);
			float x = winner == 11 ? 8.8f : -8.8f;
			iTween.MoveBy (winnerKing, new Vector3 (x, 4.8f, 0f), 1f);
			iTween.ScaleTo (winnerKing, new Vector3 (1.25f, 1.25f, 1.25f), 1f);
			float y = winner == 11 ? -7.5f : -7.5f;
			iTween.MoveBy (looserKing, new Vector3 (0, y, 0f), 1f);
			//iTween.ScaleBy (looserKing, new Vector3 (0.5f, 0.5f, 0.5f), 1f);
		}
		prevPC = currentPC;
		prevPC.FadeTheBricks ();
	
		Invoke ("DoTransitionToNewGame", newGameTransitionTime);


	}

	public void IncreaseTurn ()
	{
		tk.IncreateseTurn ();
	}

	void DoTransitionToNewGame ()
	{
		currentPC = Instantiate (prefab);
		//currentPC.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
		currentPC.enabled = true;
		//	currentPC.SFX (sfxEnabled);
		int winner = currentWinner == 12 ? -1 : currentWinner;
		currentPC.SetLevelDifficulty (levelGameDifficulty++);
		currentPC.CreatePlayField (winner, singlePlayer);

		if (victory != null) {
			victory.SetActive (false);
		}

		if (currentWinner != 12) {

			tk.newGameTurn (currentWinner);
		} else {

		}

		iTween.FadeUpdate (currentPC.gameObject, 1.0f, FadeInTime);



		sounds [0].SetActive (true);
		AudioSource newSong = sounds [0].GetComponent<AudioSource> ();

		if (soundEnabled) {
			newSong.enabled = true;
			newSong.Play ();
			StartCoroutine (audioManager.FadeAudio (newSong, 0.5f, LoadGame.Fade.In, false));
		}
		if (currentWinner != 12) {
			GameObject winnerKing = (currentWinner == 1 ? KingXContainer : KingOContainer);
			GameObject looserKing = (currentWinner == 0 ? KingXContainer : KingOContainer);
			float x = currentWinner == 1 ? -8.8f : 8.8f;
			iTween.MoveBy (winnerKing, new Vector3 (x, -4.8f, 0f), 1f);
			iTween.ScaleTo (winnerKing, new Vector3 (1.0f, 1.0f, 1.0f), 1f);
			float y = currentWinner == 1 ? -5.0f : -5.0f;
			iTween.MoveBy (looserKing, new Vector3 (0, 7.5f, 0f), 1f);
		}
		if (prevPC != null) {
			Destroy (prevPC.gameObject, FadeOutTime + 0.1f);
			prevPC = null;
		}
		winTransition = false;
		menuButton.SetActive (true);
		ShowRate ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (debugWin) {
			//currentWinner =Random.Range (10, 13);
			currentPC.DoWin ();
			debugWin = false;
		}

	}
}
