using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldCreator : MonoBehaviour
{

	// Use this for initialization

	enum OCCUPATION
	{
		NONE,
		X,
		O,
		CHILD,
		WONX,
		WONO}

	;

	enum CURSORDIRECTION
	{
		UP,
		DOWN,
		LEFT,
		RIGHT,
		IDLE
	}
	 	
	private bool AIActive = false;
	public float counterMoveLow = 0.5f;
	public float counterMoveHigh = 2.0f;
	public GameObject brick;
	float gridX = 9f;
	float gridY = 9f;
	public float spacing_vertical = 2f;
	public float spacing_horizontal = 2f;
	public float gridspacing = 0.2f;
	public ScoreKeeper sk;
	public Vector2 gridstart = new Vector2 ((9 / 2) + 0.5f, (9 / 2) + 0.5f);
 
	public GameObject smallVictorySoundObject;
	//public GameObject bigVictoryClapObject;
	public GameObject bigVictoryFanfareObject;

	private PlayRandomSound smallVictorySound;
	private PlayRandomSound clapSound;
	private PlayRandomSound fanfareSound;
	private GameObject[,] bricks = new GameObject[9, 9];

	public AudioClip[] voices;
 
	public bool allowTie = true;
	private bool bigWin = false;
	private bool addedBigScore = false;

	private bool playInteractionEnabled = true;
	float interval = 0.5f;
	float nextTime = 0;
	public bool fadeBricks = false;
	int singX = 0;
	public bool doTest = false;
	public float emptyCoinDistance = 1.3f;
	public float emptyCoinTime = 2.5f;
	private bool SinglePlayer = false;
	private TicTacAI AI;
	private Dictionary <string,Vector2> brickLookup = null;
	private int winner = -1;
	public int levelDifficulty = 0;
	private int LevelStartDifficulty = 80;
	private int LevelEndDifficulty = 90;
	private int LevelDontFollow = 10;
	private GameObject keyBrick = null;
	private int keyIndexY = 4;
	private int keyIndexX = 4;
	public bool useKeyboard = true;
	public GameObject cursor;
	private CURSORDIRECTION cursorDir = CURSORDIRECTION.IDLE;
	private BrickInteraction selectedBrick = null;
	private PlayRandomSound cursorClickSound;
	class CounterMover {
		public Vector2 move;
		public bool enab;
	}
	private CounterMover counterMove = null;
	public void FadeTheBricks ()
	{
		for (int y = 0; y < gridY; y++) {
			for (int x = 0; x < gridX; x++) {
				bricks [y, x].GetComponent<BrickInteraction> ().fade = true;
			}
		}
	}

	void Start ()
	{
		if (AI == null) {
			AI = new TicTacAI ();
		}
	
		bigWin = false;
		addedBigScore = false;
		SinglePlayer = GameManager.singlePlayer;
		playInteractionEnabled = true;
		interval = 1f;
		nextTime = 0;
		singX = 0;
		if (useKeyboard) {
			cursor.SetActive (true);
			cursorClickSound = cursor.GetComponent<PlayRandomSound> ();
		}

	}
	public void GetDifficulty() {
		LevelStartDifficulty = GameManager.instance.GetLevelDifficultyStart ();
		LevelEndDifficulty = GameManager.instance.GetLevelDifficulltyEnd ();
		LevelDontFollow = GameManager.instance.GetLevelDontFollow ();

		LevelStartDifficulty =  LevelStartDifficulty + levelDifficulty;
		LevelEndDifficulty = LevelEndDifficulty + levelDifficulty;
		if (LevelStartDifficulty > 100) {
			LevelStartDifficulty = 98;
		}
		if (LevelEndDifficulty > 100) {
			LevelEndDifficulty = 100;
		}


	}
	public bool FadeBricks ()
	{
		return fadeBricks;
	}

	public IEnumerator CounterMove(float delay, int board,int y, int x) {
		int outBoard;
		int outPos;
	
		bool valid = false;
		yield return new WaitForSeconds (delay);
		counterMove = new CounterMover ();
		counterMove.move = new Vector2 (board, x);

		


	}
	public int GetLevelDifficulty() {
		return levelDifficulty;
	}
	public void SetLevelDifficulty(int l) {
		levelDifficulty = l;
	}

	public void BrickPlayReport(int board, int y, int x) {

		if (SinglePlayer ) {
			//valid = AI.MakeMove(y, v2 - 1, out outBoard, out outPos, 100,  100,0);
			if (GameManager.instance.CurrentPlayer () == 1) {
				GetDifficulty ();
				playInteractionEnabled = false;
				AIActive = true;
				StartCoroutine (CounterMove (Random.Range (counterMoveLow, counterMoveHigh), board, y, x));

			}

		}


	}
	public void Test ()
	{
		for (int y = 0; y < gridY - 3; y++) {




			for (int x = 0; x < gridX - 3; x++) {
				if (x % 3 == 0 && x != 0) {
					

				}

			
				bricks [y, x].GetComponent<BrickInteraction> ().DoOnClick (false);
			}
		}
	}

	public void CreatePlayField (int player, bool singlePlayer)
	{
		AIActive = false;
		playInteractionEnabled = true;
		addedBigScore = false;
		SinglePlayer = singlePlayer;
		smallVictorySound = smallVictorySoundObject.GetComponent<PlayRandomSound> ();
		//clapSound = bigVictoryClapObject.GetComponent<PlayRandomSound> ();
		fanfareSound = bigVictoryFanfareObject.GetComponent<PlayRandomSound> ();
		#if UNITY_TVOS
		useKeyboard = true;
		#endif
		iTween.ScaleTo (cursor, new Vector3 (0.2f,0.2f, 1.0f), 0.5f);
		if (useKeyboard) {

			cursor.SetActive (true);
			cursor.transform.position = new Vector3 (0.0f, 0.0f, 1.0f);
		}
		winner = -1;
		bigWin = false;
		float positionY = 0;
		if (brickLookup == null) {
			brickLookup = new Dictionary<string, Vector2> ();
		}
		int i = 0;
		int startColorIndex = 1;
		if (player != -1) {
			startColorIndex = player == 0 ? 1 : 0;
		} else {
			startColorIndex = GameManager.instance.CurrentPlayer () == 0 ? 1 : 0;
		}
		sk.SetColor (startColorIndex);
		int internalBoardY = 0;
		int internalBoardX = 0;
		int internalBoard = -3;
		int k = 0;
		for (int y = 0; y < gridY; y++) {
			float positionX = 0;

			if (y % 3 == 0 && y != 0) {
				positionY += gridspacing;
			
			}
			int brickY = y % 3;
			if (y % 3 == 0) {
			//	internalBoard++;
			}
			if (k % 27  == 0) {
				internalBoard+=3;
			}
			for (int x = 0; x < gridX; x++) {
				if (x % 3 == 0 && x != 0) {
					positionX += gridspacing;

				}

				int brickX = x % 3;
					
				Vector3 pos = new Vector3 (positionX - gridstart.x, positionY - gridstart.y, 0);
				i++;
				bricks [y, x] = Instantiate (brick, pos, Quaternion.identity);
				bricks [y, x].gameObject.name = "brick" + y + " " + x;
				bricks [y, x].transform.parent = gameObject.transform;
				int board = (x / 3) + ((8 - y) / 3) * 3; // transform coordinates to ai coordinates.
				int xb = (x % 3) + ((8 - y) % 3) * 3; //

	//			internalBoard  = k % 27 /3 ;
				k++;
				brickLookup.Add ("b" + board + "p" + xb, new Vector2( x,y));
				int padding = 0;
				if(x>2 && x<6) padding = 1;
				if (x > 5 && x < 9)
					padding = 2; 
				bricks [y, x].GetComponent<BrickInteraction> ().internalBoard = internalBoard+padding;
				if (brickY == 1 && brickX == 1) {
					bricks [y, x].GetComponent<BrickInteraction> ().isCenter = true;
				}
				bricks [y, x].GetComponent<BrickInteraction> ().SetBoardInfo (board, y, xb);
				if (i == 41) {
					selectedBrick = bricks [y, x].GetComponent<BrickInteraction>();
				}
				if (player > -1 && i == 41) {
					bool valid = false;
					if (singlePlayer && player == 0) {
						if (AI == null)
							AI = new TicTacAI ();
						AIActive = true;
						valid = AI.ForceComputerMove (4, 4);
					//	bricks [y, x].GetComponent<BrickInteraction> ().SetTaken (player);	
					//	GameManager.instance.IncreaseTurn ();
				//		bricks [4, 4].GetComponent<BrickInteraction> ().DoOnClick (true);
						playInteractionEnabled = true;
						GameManager.instance.tk.SetPlayerTurn ();
						AIActive = false;
					} else {
						if (SinglePlayer) {
							StartCoroutine (CounterMove (Random.Range (counterMoveLow, counterMoveHigh), board, y, x));
						}
					}
					bricks [y, x].GetComponent<BrickInteraction> ().SetTaken (player);

					
				} else if(singlePlayer && player == -1 && i == 41 && GameManager.instance.CurrentPlayer() == 1) {
					if (AI == null)
						AI = new TicTacAI ();
					AIActive = true;
					AI.ForceComputerMove (4, 4);

					bricks [y, x].GetComponent<BrickInteraction> ().DoOnClick (true);
					GameManager.instance.tk.newGameTurn (0);
				
				}

				positionX += spacing_horizontal;


			}
			positionY += spacing_vertical;

		}//
		#if UNITY_TVOS
		UnityEngine.Apple.TV.Remote.touchesEnabled = true;
	//	UnityEngine.Apple.TV.Remote.reportAbsoluteDpadValues = true;
		UnityEngine.Apple.TV.Remote.allowExitToHome = false;
		#endif
		GetDifficulty ();
		keyBrick = bricks [keyIndexY,keyIndexX];

		GeneratedTick.OnTick += Sing;
 
	}

	void OnDestroy ()
	{
		GeneratedTick.OnTick -= Sing;
	}


	void Sing ()
	{
		
		if (GameManager.instance.SoundEnabled () == false)
			return;
		if (GameManager.instance.WinAnimPlaying () == true)
			return;
		//bricks [3, singX].transform.localScale -= new Vector3 (1.1F, .1F, 0);
		singX++;
		if (singX > 8)
			singX = 0;

		for (int y = 1; y < gridY; y += 3) {

		 

			if (singX % 3 == 0 && (bricks [y, singX+1].GetComponent<BrickInteraction> ().canSing)) {
				if (bricks [y, singX].GetComponent<BrickInteraction> ().isTaken == 11) {
					

						bricks [y, singX + 1].GetComponent<BrickInteraction> ().singlow (voices [y]);
				
					bricks [y, singX + 1].GetComponent<BrickInteraction> ().SetAnimTrigger ("goSingXBig");
				}


				if (bricks [y, singX].GetComponent<BrickInteraction> ().isTaken == 10) {
					
						bricks [y, singX + 1].GetComponent<BrickInteraction> ().singlow (voices [y]);

					bricks [y, singX + 1].GetComponent<BrickInteraction> ().SetAnimTrigger ("goSingOBig");

				}
				if (bricks [y, singX].GetComponent<BrickInteraction> ().isTaken == 12) {

						bricks [y, singX + 1].GetComponent<BrickInteraction> ().singlow (voices [y]);

					bricks [y, singX + 1].GetComponent<BrickInteraction> ().SetAnimTrigger ("goSingDBig");


				}
			
			}
		}		
		//bricks [3, singX].transform.localScale += new Vector3 (1.1F, .1F, 0);

		for (int y = 0; y < gridY; y++) {
			if (bricks [y, singX].GetComponent<BrickInteraction> ().isTaken != -1) {

				if (bricks [y, singX].GetComponent<BrickInteraction> ().isTaken == 0) {
					bricks [y, singX].GetComponent<BrickInteraction> ().SetAnimTrigger ("goSingO");

					bricks [y, singX].GetComponent<BrickInteraction> ().sing (voices [y]);

	 
				} else if (bricks [y, singX].GetComponent<BrickInteraction> ().isTaken == 1) {
					bricks [y, singX].GetComponent<BrickInteraction> ().SetAnimTrigger ("goSingX");

					bricks [y, singX].GetComponent<BrickInteraction> ().sing (voices [y]);

				}
			} else {
				if(playInteractionEnabled) {
				bricks [y, singX].GetComponent<BrickInteraction> ().TriggerEmptySing (emptyCoinDistance, emptyCoinTime);
					}
			}
		}
	}




	int getStartY (int board)
	{
		switch (board) {
		case 0:
			return 0;
			 
		case 1:
			return 0;
		 
		case 2:
			return 0;
			 
		case 3:
			return 3;
			 
		case 4:
			return 3;
			 
		case 5:
			return 3;
		 
		case 6:
			return 6;
			 
		case 7:
			return 6;
			 
		case 8:
			return 6;
			 
		}
		return 0;
	}

	int getStartX (int board)
	{
		switch (board) {
		case 0:
			return 0;
		 
		case 1:
			return 3;
			 
		case 2:
			return 6;
		 
		case 3:
			return 0;
		 
		case 4:
			return 3;
			 
		case 5:
			return 6;
			 
		case 6:
			return 0;
		 
		case 7:
			return 3;
		 
		case 8:
			return 6;
			 
		}
		return 0;
	}



	int taken (int x, int y)
	{
		return bricks [x, y].GetComponent<BrickInteraction> ().isTaken;
	}

	int checkline (int[] results)
	{   
		if (results [0] < 10 || results [1] < 10 || results [2] < 10) {
			return -1;
		}
		if (results [0] != results [1] && results [1] != results [2]) {
			int w = -1;

			// check for lines that are 12 x 12 or 12 o 12 (accept those) 12 o x or similar should be rejected.
			for (int i = 0; i < 3; i++) {
				if (results [i] != 12) {
					if (w == -1) {
						w = results [i];
					} else if (w != results [i]) {
						return -1;
					}
				}
			}
			return w;
		}
		if ((results [0] == results [1] || (results [0] == 12 || results [1] == 12))
		    &&
		    (results [1] == results [2] || (results [1] == 12 || results [2] == 12))
		    && ((results [0] == 11 || results [0] == 10)
		    || (results [1] == 11 || results [1] == 10)
		    || (results [2] == 11 || results [2] == 10))) {
			for (int i = 0; i < 3; i++) {
				if (results [i] == -1) {
			
					return -1;
				}
				if (results [i] == 11 || results [i] == 10) {
					
					return results [i];
				}

			}

		}
		if (results [0] == results [1] && results [1] == results [2] && results [2] == 12) {
			return -1;
		}
		return -1;
	}
	int [] calculateScore() {

		int[] points = new int [4]{ 0,0,0,0};
		for (int i = 0; i < 9; i++) {
			int startX = getStartX (i);
			int startY = getStartY (i);
			int looser = (winner == 1 ? 0 : 1);
			int changeIndex = winner == 0 ? 2 : 3; // we save the loose change wins in a seperate index.
			for (int xpos = 0; xpos < 3; xpos++) {
				for (int ypos = 0; ypos < 3; ypos++) {

					int takenIndex = bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().isTaken;
					if (takenIndex > -1 && takenIndex < 2) {
						points [changeIndex] += 1;
					}
					if (takenIndex >=0  && bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ()._isBig) {
						int index = takenIndex;
						if (takenIndex > 2) {
							index = takenIndex == 10 ? 0 : 1;
						}
						points [index] += 9;
					}
	

				}
			}
		}
	//	Debug.Log ("points for x  " + points [0] + " points for o " + points [1]);
		return points;

	}
	int checkWinBig ()
	{
		// since we allow ties games it conceivible that two players win so we need to count every scenario
		// the tie object can be used as a joker and some game scenarios allow for two different scenarios using the same joker.
		// the rule is that if there is more than one winner we do a tie on the whole game.


		int[] scenarios = new int[4]{ -1, -1, -1, -1 };  
		int[] results = new int [3]{ 0, 0, 0 };
		int w = -1;

		//rule 1
		for (int ypos = 0; ypos < 3; ypos++) {

			results [0] = taken (0, ypos * 3);
			results [1] = taken (4, ypos * 3);
			results [2] = taken (6, ypos * 3);
			w = checkline (results);
			if (w > -1) {
				winner = w;
				scenarios [0] = winner;
				
			}
		}


		//rule 2
		for (int xpos = 0; xpos < (3); xpos++) {

			results [0] = taken (xpos * 3, 0);
			results [1] = taken (xpos * 3, 4);
			results [2] = taken (xpos * 3, 6);
			w = checkline (results);
			if (w > -1) {
				winner = w;
				scenarios [1] = winner;
			}
				
		}
		results [0] = taken (0, 0);
		results [1] = taken (4, 4);
		results [2] = taken (6, 6);
		w = checkline (results);
		//rule 3
		if (w > -1) {
			winner = w;
			scenarios [2] = winner;
		}
		results [0] = taken (0, 6);
		results [1] = taken (4, 4);
		results [2] = taken (6, 0);
		w = checkline (results);
		if (w > -1) {
			winner = w;
			scenarios [3] = winner;
		}
		int owin = 0;
		int xwin = 0;
		// since we can have two players using the same joker we need to sum up each win and if both people won it's a tie.
		for (int i = 0; i < 4; i++) {
			if (scenarios [i] == 11) {
				owin++;
			}
			if (scenarios [i] == 10) {
				xwin++;
			}
		}



		//rule 4
		int occupied = 0;

		if (owin > 0 && xwin > 0) {
			winner = 12; // both players used a joker so tie.
			return -1;
		}
		if (xwin > 0)
			winner = 10;
		if (owin > 0)
			winner = 11;
		
		for (int xpos = 0; xpos <= 8; xpos++) {
			for (int ypos = 0; ypos <= 8; ypos++) {
				if (bricks [xpos, ypos].GetComponent<BrickInteraction> ().isTaken >= 10)
					occupied++;
			}
		}

		if (winner < 0 && occupied == 81) {
			winner = 12;
			return 12;
		}

		if (winner > 0 && addedBigScore == false) {
			addedBigScore = true;
			int wi = winner == 11 ? 0 : 1;
			int looser = winner == 11 ? 1 : 0;

			int []score = calculateScore ();
			int winlooseChangeIndex = wi == 0 ? 2 : 3;
			int looseLooseChangeIndex = wi == 0 ? 3 : 2;
			int winsum = 18 + score [winlooseChangeIndex];

			int winLoose = score [winlooseChangeIndex];
			int looseLoose = score [looseLooseChangeIndex];
			int wA = score [0];
			int wB = score [1];

			sk.AddScore (18, wi);
			sk.RemoveScore (score [looseLooseChangeIndex], looser);
			if (GameManager.instance.SFXEnabled ()) {
				//	clapSound.Play ();
			}
		}


		return -1;


	}

	void win (int board, int winner)
	{
		int[] points = new int [3]{ 0, 0, 0 };
		int startX = getStartX (board);
		int startY = getStartY (board);
		int looser = (winner == 1 ? 0 : 1);
		if (GameManager.instance.SFXEnabled () && winner != 12) {
			smallVictorySound.Play ();
		}

		for (int xpos = 0; xpos < 3; xpos++) {
			for (int ypos = 0; ypos < 3; ypos++) {
				Vector3 target = bricks [startX + 1, startY + 1].transform.position;
				int takenIndex = bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().isTaken;
				if (takenIndex > -1 && takenIndex < 2) {
					points [takenIndex] += 1;
				}
				if (winner == 1) {
					if (xpos == 1 && ypos == 1) {
						bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().setWin (11, true, target);
			
					} else {
						bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().setWin (11, false, target);
					
					}
				} else if (winner == 0) {
					if (xpos == 1 && ypos == 1) {
						bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().setWin (10, true, target);
					} else {
						bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().setWin (10, false, target);
					}

				} else if (winner == 2) {
					if (xpos == 1 && ypos == 1) {
						bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().setWin (12, true, target);
					} else {
						bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().setWin (12, false, target);
					}			
				}
			}

		}

		if (winner < 2 && winner > -1) {
			int score = 9 - points [winner];
			if (score > 0) {
				sk.AddScore (score +9, looser);
			}
			sk.RemoveScore (points [looser], winner);
		}
		int bigWin = checkWinBig ();
		if (bigWin > -1) {
			winner = bigWin;
			//Debug.Log ("Big win!" + bigWin);
			//	checkWinBig ();
		}
	}
 
	//The rules are:
	//1: if you get 3 in a row vertically, you win.
	//2: if you get 3 in a row horizontally, you win.
	// 3: if you get 3 in a horizontally.
	//4: if the a board is full, the one with most boards win.
 

	void checkBoard (int board)
	{
		
		int startX = getStartX (board);
		int startY = getStartY (board);

 
 
		if (bricks [startX, startY].GetComponent<BrickInteraction> ().isTaken == 11)
			return;
		if (bricks [startX, startY].GetComponent<BrickInteraction> ().isTaken == 10)
			return;
 

		//rule 1
		for (int ypos = startY; ypos < startY + 3; ypos++) {
			if (bricks [startX, ypos].GetComponent<BrickInteraction> ().isTaken == bricks [startX + 1, ypos].GetComponent<BrickInteraction> ().isTaken &&
			    bricks [startX + 1, ypos].GetComponent<BrickInteraction> ().isTaken == bricks [startX + 2, ypos].GetComponent<BrickInteraction> ().isTaken &&
			    (bricks [startX, ypos].GetComponent<BrickInteraction> ().isTaken != -1)) {
				win (board, bricks [startX, ypos].GetComponent<BrickInteraction> ().isTaken);
				return;
			}
		}


		//rule 2
		for (int xpos = startX; xpos < (startX + 3); xpos++) {
			if (bricks [xpos, startY].GetComponent<BrickInteraction> ().isTaken == bricks [xpos, startY + 1].GetComponent<BrickInteraction> ().isTaken &&
			    bricks [xpos, startY + 1].GetComponent<BrickInteraction> ().isTaken == bricks [xpos, startY + 2].GetComponent<BrickInteraction> ().isTaken &&
			    (bricks [xpos, startY].GetComponent<BrickInteraction> ().isTaken != -1)) {
				win (board, bricks [xpos, startY].GetComponent<BrickInteraction> ().isTaken);
				return;
			}

		}

		//rule 3
		if (bricks [startX, startY].GetComponent<BrickInteraction> ().isTaken == bricks [startX + 1, startY + 1].GetComponent<BrickInteraction> ().isTaken &&
		    bricks [startX + 1, startY + 1].GetComponent<BrickInteraction> ().isTaken == bricks [startX + 2, startY + 2].GetComponent<BrickInteraction> ().isTaken &&
		    (bricks [startX, startY].GetComponent<BrickInteraction> ().isTaken != -1)) {
			win (board, bricks [startX, startY].GetComponent<BrickInteraction> ().isTaken);
			return;
		}


		if (bricks [startX, startY + 2].GetComponent<BrickInteraction> ().isTaken == bricks [startX + 1, startY + 1].GetComponent<BrickInteraction> ().isTaken &&
		    bricks [startX + 1, startY + 1].GetComponent<BrickInteraction> ().isTaken == bricks [startX + 2, startY].GetComponent<BrickInteraction> ().isTaken &&
		    (bricks [startX, startY + 2].GetComponent<BrickInteraction> ().isTaken != -1)) {
			win (board, bricks [startX, startY + 2].GetComponent<BrickInteraction> ().isTaken);
			return;
		}

		//rule 4
		bool full = true;
		for (int xpos = 0; xpos < 3; xpos++) {
			for (int ypos = 0; ypos < 3; ypos++) {
				if (bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().isTaken == -1) {
					full = false;
				}
			}
		}

		if (full) {
			if (allowTie == false) {
				int count0 = 0;
				int countX = 0;
				for (int xpos = 0; xpos < 3; xpos++) {
					for (int ypos = 0; ypos < 3; ypos++) {
						if (bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().isTaken == 0)
							count0++;
						if (bricks [startX + xpos, startY + ypos].GetComponent<BrickInteraction> ().isTaken == 1)
							countX++;
					}
				}
				if (count0 > countX)
					win (board, 0);
				else
					win (board, 1);

			} else {
				win (board, 2);
			}
		}
	}

	void CheckForWin ()
	{
		for (int i = 0; i < 9; i++) {
			checkBoard (i);
		 
		}
	}

	public void SetIntercationEnabled (bool status)
	{
		playInteractionEnabled = status;
	}

	public bool CanBricksInteract ()
	{
		if (playInteractionEnabled == false)
			return false;
		if (AIActive)
			return false;
		if (SinglePlayer && GameManager.instance.CurrentPlayer () == 0 && bigWin == false) {
			return true;
		}
			
		nextTime = 0;
		return !bigWin;
	}
	IEnumerator NewGame() {
		yield return new WaitForSeconds (0.2f);
		GameManager.instance.NewGame (winner);
		if (GameManager.instance.SFXEnabled ()) {
			fanfareSound.Play ();
		}
		winner = -1;
	}
	public void DoWin() {
		SetIntercationEnabled (false);
		cursor.SetActive (false);
		//Trigger total win here!!
		//Instatiate.
		bigWin = true;
		GameManager.instance.MuteCurrentSongForWin ();
		StartCoroutine (NewGame());
		//delete.
	}


	float time;

	KeyCode GetKeyCode ()
	{
		KeyCode result = KeyCode.Underscore;
		#if !UNITY_TVOS || UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			return KeyCode.RightArrow;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			return KeyCode.UpArrow;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			return KeyCode.DownArrow;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			return KeyCode.RightArrow;
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			return KeyCode.LeftArrow;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			return KeyCode.Space;
		}
		#endif
		#if UNITY_TVOS
		if (Input.GetKeyDown (KeyCode.JoystickButton0)) { //click on "Menu"
			return KeyCode.Menu;
		}
		if (Input.GetKeyDown (KeyCode.JoystickButton14)) {
		return KeyCode.Space;
		}
	//	time = 0;
		if (time < Time.time) { // wait between samplings so we don't get to much repeat key.
		float y = Input.GetAxis("VerticalGame");
			float x = Input.GetAxis ("HorizontalGame");
		

			if (y == 0 && x == 0) {
				time = Time.time + 0.05f;
				return  result;
			}
			float tempX = x;
			float tempY = y;
			if(tempX <0) {
				tempX = -1*tempX;
			}

			time = Time.time + 0.15f;
			if(tempY <0) {
				tempY = -1*tempY;
			}
			if(tempX > tempY) {
				if (x < 0) {
					return KeyCode.LeftArrow;
				}  else if (x > 0) {
					return KeyCode.RightArrow;
				}	
			}
			if(tempY > tempX) {
				if (y > 0) {
					return KeyCode.UpArrow;
				}  else if (y < 0) {
					return KeyCode.DownArrow;
				}
			}
		
				
		
			Debug.Log(""+x + "     "+y);
		
		

		}

		#endif
		return result;
	}
	Vector2 GetCoordinates(int x, int y, CURSORDIRECTION dir) {
		int horizontalPad = 1;
		int verticalPad = 1;

		int board = bricks [y, x].GetComponent<BrickInteraction> ().internalBoard;

		int xb = getStartX (board)+horizontalPad;
		int yb=   getStartY (board) +verticalPad;

		return new Vector2 (xb, yb);
	}


	Vector2 GetCoordinates2(int x, int y, CURSORDIRECTION dir) {
		int horizontalPad = 1;
		int verticalPad = 1;

		if (dir == CURSORDIRECTION.LEFT)
			horizontalPad = 2;
		if (dir == CURSORDIRECTION.RIGHT)
			horizontalPad = 0;
		if (dir == CURSORDIRECTION.DOWN)
			verticalPad = 0;
		if (dir == CURSORDIRECTION.UP)
			verticalPad = 2;
	
		int board = bricks [y, x].GetComponent<BrickInteraction> ().internalBoard;

		int xb = getStartX (board)+horizontalPad;
		int yb=   getStartY (board) +verticalPad;
	
		return new Vector2 (xb, yb);
	}

	int GetBoardForBrick(int x, int y) {
		int b = bricks [y, x].GetComponent<BrickInteraction> ().internalBoard;
		int xb = getStartX (b) + 1;
		int yb=   getStartY (b) +1;
		/*if (yb != 4) {
			yb = yb == 7 ? 1 : 7;
		}*/
		int board = bricks [yb, xb].GetComponent<BrickInteraction> ().internalBoard;
		return board;
	}

	void MoveCursor(int x, int y)
	{
	/*	
		if (bricks [y, x].GetComponent<BrickInteraction> ().isTaken >= 10) {

			// this 3x3 is a big piece so get the board and get the center x and y and set the cursor to be at the center of the 3x3 board.


			bricks [keyIndexY, keyIndexX].GetComponent<Animator> ().enabled = true;
			iTween.MoveTo (cursor, bricks [y, x].transform.position, 0.5f);
			iTween.ScaleTo (cursor, new Vector3 (0.6f,0.6f, 1.0f), 0.5f);


		} else {

	*/
		cursorClickSound.Play ();
		bricks [keyIndexY, keyIndexX].GetComponent<Animator> ().enabled = true;
			iTween.MoveTo (cursor, bricks [keyIndexY, keyIndexX].transform.position, 0.25f);
		//	iTween.ScaleTo (cursor, new Vector3 (0.2f,0.2f, 1.0f), 0.5f);
	//	}

	}


	void  SetNewBoardInDirection(CURSORDIRECTION dir) {
		int x = keyIndexX;
		int y = keyIndexY;
		int currentBoard = GetBoardForBrick (x, y);
		int newBoard = 0;
		Debug.Log ("board" + currentBoard);
	
		switch (dir) {
		case CURSORDIRECTION.UP:
			newBoard = currentBoard + 3;
			y++;
			break;
		case CURSORDIRECTION.DOWN:
			newBoard = currentBoard - 3;
			y--;
			break;
		case CURSORDIRECTION.LEFT:
			newBoard = currentBoard - 1;
			x--;
			break;
		case CURSORDIRECTION.RIGHT:
			newBoard = currentBoard + 1;
			x++;
				break;
	
		}
		if (newBoard < 0 || newBoard > 8) {
			newBoard = currentBoard;
		}
		if (x < 0 || x > 8) {
			KissKing (keyIndexX);
			x = keyIndexX;
		}
		if (y < 0 || y > 8) {
			y = keyIndexY;
		}

		//	Debug.Log("just move in our empty square") ;
			keyIndexX = x;
			keyIndexY = y;
			selectedBrick = bricks [y, x].GetComponent<BrickInteraction>();
	
	//	Debug.Log ("direction board" + newBoard);
	
	}
	void KissKing(int index) {
		if (index < 0) {
			index = 0; // left king
		}
		if (index > 8) {
			index = 1; // right king.
		}
		GameManager.instance.KissKing (index);
	}

	void MoveCursorWithClamp(CURSORDIRECTION dir) {
		

	DoClampValue();

	
		bool isCurrentBoardBigWin = bricks [keyIndexY, keyIndexX].GetComponent<BrickInteraction> ().isTaken != -1;
		SetNewBoardInDirection (dir);
		if (isCurrentBoardBigWin) {
			//Vector2 newCoordinates = 
		}



		Vector2 coord = GetCoordinates (keyIndexX, keyIndexY,dir);
		int x =(int) coord.x;
		int y =(int) coord.y;
		//if (x != keyIndexX || y != keyIndexY) {
		MoveCursor (x,y);

	//	}
	}
	bool DoClampValue() {
		if (keyIndexX < 0) {
			keyIndexX = 0;
			return true;
		}
		if (keyIndexX > 8) {
			keyIndexX = 8;
			return true;
		}
		if (keyIndexY < 0) {
			keyIndexY = 0;
			return true;
		}
		if (keyIndexY > 8) {
			keyIndexY = 8;
			return true;
		}

		return false;
	}





	void OnMoveLeft() {
		
		//keyIndexX--;
		MoveCursorWithClamp (CURSORDIRECTION.LEFT);


	}
	void OnMoveRight() {
		
		//keyIndexX++;
		MoveCursorWithClamp (CURSORDIRECTION.RIGHT);

	}
	void OnMoveUp() {
		
	
			//keyIndexY++;



		MoveCursorWithClamp (CURSORDIRECTION.UP);
	
	}
	void OnMoveDown() {


			//keyIndexY--;



		MoveCursorWithClamp (CURSORDIRECTION.DOWN);
	
	}
	IEnumerator GrowCursor(BrickInteraction brick,int x, int y) {
		yield return new WaitForSeconds (0.8f);
		int brickID = brick.isTaken; 
		if (brickID >= 10) {
			brick.GetComponent<Animator> ().enabled = true;
			iTween.MoveTo (cursor, bricks [y, x].transform.position, 0.5f);
			iTween.ScaleTo (cursor, new Vector3 (0.6f, 0.6f, 1.0f), 0.5f);
		}
	}
	void OnSelectBrick() {
		if (CanBricksInteract ()) {
			selectedBrick = bricks [keyIndexY, keyIndexX].GetComponent<BrickInteraction> ();
			//bricks[keyIndexY][keyIndexX]	
			selectedBrick.GetComponent<Animator> ().enabled = true;
			selectedBrick.DoOnClick (false);
			Vector2 coord = GetCoordinates (keyIndexX, keyIndexY, CURSORDIRECTION.IDLE);
			int x = (int)coord.x;
			int y = (int)coord.y;
			int internalb = bricks [y, x].GetComponent<BrickInteraction> ().internalBoard;
			CheckForWin ();
			//StartCoroutine (GrowCursor (selectedBrick, x, y));
		}
	}



	void CheckForAnyWin() {
		if (Time.time >= nextTime) {

			//do something here every interval seconds


			CheckForWin ();
			nextTime += interval; 
			if (winner != -1 && bigWin == false) { 
				counterMove = null;
				AI = null;
				DoWin ();


				//delete.

			} else {
				if (counterMove != null) {
					int outBoard;
					int outPos;
					bool valid = true;

					if (AI != null) {
						valid = AI.MakeMove ((int)counterMove.move.x, (int)counterMove.move.y, out outBoard, out outPos, LevelStartDifficulty, LevelEndDifficulty, LevelDontFollow);


						Vector2 bindex = (Vector2)brickLookup ["b" + outBoard + "p" + outPos];
						//	Debug.Log (" internal array =  y "+(int)bindex.y + " " +(int)bindex.x+ " Ai board = "+ outBoard + " pos " +outPos +"\n");
					//	GameManager.instance.SetAI();
						bricks [(int)bindex.y, (int)bindex.x].GetComponent<BrickInteraction> ().DoOnClick (true);
						playInteractionEnabled = true;
						counterMove = null;
						 AIActive = false;
					}
				}
	
	}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (doTest) {
			doTest = false;
			Test ();
		}	//nextTime = 0;
			CheckForAnyWin();
		if (useKeyboard) {
			KeyCode pressedKey = GetKeyCode ();
			if (pressedKey == KeyCode.Menu) {
				GameManager.instance.ToggleMenu ();
			}
			//	if (playInteractionEnabled) {
//------- KEYBOARD INPUT
				
			//}
			if (GameManager.instance.IsMenuShowing () == false) {
				if (pressedKey == KeyCode.Space ) {
					OnSelectBrick ();
				}  
				if (pressedKey == KeyCode.LeftArrow) {
					OnMoveLeft ();
				} else if (pressedKey == KeyCode.RightArrow) {
					OnMoveRight ();
				} else if (pressedKey == KeyCode.UpArrow) {
					OnMoveUp ();
				} else if (pressedKey == KeyCode.DownArrow) {
					OnMoveDown ();
				}
			}
		}

}


}
