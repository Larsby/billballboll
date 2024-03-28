using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickInteraction : MonoBehaviour
{

	public Sprite clear;
	public Sprite untaken;
	public Sprite x;
	public Sprite xpynt;
	public Sprite opynt;
	public Sprite o;
	public Sprite xb;
	//big x
	public Sprite ob;
	// big o
	public Sprite db;
	// big draw
	public Sprite xWIN;
	public Sprite oWIN;
	public Sprite xoWIN;

	public GameObject X;
	public GameObject O;

	public GameObject XBig;
	public GameObject OBig;
	public GameObject DBig;

	public TurnKeeper tk;
	public ScoreKeeper sk;
	public ParticleSystem particlesMark;
	public ParticleSystem particleWinArea;

	public AudioClip touchSound;

	private PlayfieldCreator parent;
	private GameObject playerObject = null;
	private Sprite playerSprite;
	private Sprite winnerSprite;
	private string playerString;
	private string winnerTrigger;
	public bool fade = false;
	public bool fadein = true;
	private bool init = false;
	public int isTaken = -1;
	private bool disableAndHide = false;
	private Animator animator;
	public bool canSing = true;
	public bool _isBig;
	private bool singEmpty;
	private bool singEmptyDown;
	private 	GameObject singSprite;
	private float fadeStep = 0.1f;
	private int board;
	private int xposOnBoard;
	private int yposOnBoard;
	public bool isCenter = false;
	public int internalBoard = -1;

	void Start ()
	{
		_isBig = false;
		GetComponent<SpriteRenderer> ().sprite = untaken;
		parent = GetComponentInParent<PlayfieldCreator> ();
		winnerTrigger = null;
		fade = false;
		fadein = true;
		Material m = GetComponent<Renderer> ().material;
		Color c = new Color (m.color.r, m.color.g, m.color.b, 0.0f);
		m.color = c;
		animator = GetComponent<Animator> ();
		canSing = true;
	 singSprite= transform.GetChild (0).gameObject;
	
		if (singSprite != null) {

			m = singSprite.GetComponent<Renderer> ().material;
			c = new Color (m.color.r, m.color.g, m.color.b, 0.0f);
			m.color = c;

		}
		singEmptyDown = false;
	}
	public int GetBoard() {
		return board;
	}
	public void SetBoardInfo(int board, int ypos, int xpos) {
		this.board = board;
		xposOnBoard = xpos;
		yposOnBoard = ypos;
	}
	public void KeyboardFocus() {
		if (isTaken > -1) {
			GetComponent<SpriteRenderer> ().sprite = untaken;
		}

	}
	public void KeyboardUnFocus() {

		if (isTaken > -1) {
			GetComponent<SpriteRenderer> ().sprite = clear;
		}
	}
	void Update ()
	{  
		
			if (singEmpty && singEmptyDown == false) {
				Material m = GetComponent<Renderer> ().material;
				Color c = new Color (m.color.r, m.color.g, m.color.b, m.color.a - fadeStep);
				m.color = c;

				m = singSprite.GetComponent<Renderer> ().material;
				c = new Color (m.color.r, m.color.g, m.color.b, m.color.a + fadeStep);
				m.color = c;
				if (c.a >= 1.0) {
					singEmptyDown = true;
				}
			}
			if (singEmptyDown && singEmpty) {
				Material m = GetComponent<Renderer> ().material;
				Color c = new Color (m.color.r, m.color.g, m.color.b, m.color.a + fadeStep);
				m.color = c;

				m = singSprite.GetComponent<Renderer> ().material;
				c = new Color (m.color.r, m.color.g, m.color.b, m.color.a - fadeStep);
				m.color = c;
				if (c.a <= 0.0) {

					singEmpty = false;
					//singEmptyDown = false;
				}
		
			}

		if (fadein) {
			Material m = GetComponent<Renderer> ().material;
			Color c = new Color (m.color.r, m.color.g, m.color.b, m.color.a + 0.05f);
			m.color = c;
			if (c.a > 1.0) {
				fadein = false;
			}
		}
		if (fade) {
			Material m = null;

			if (playerObject != null) {
				
				m = playerObject.GetComponent<Renderer> ().material;
				Color c = new Color (m.color.r, m.color.g, m.color.b, m.color.a - 0.05f);
				m.color = c;
				foreach (Transform child in playerObject.transform) {
					m = child.GetComponent<Renderer> ().material;
					c = new Color (m.color.r, m.color.g, m.color.b, m.color.a - 0.05f);
					m.color = c;

					foreach (Transform grandChild in child) {
						m = grandChild.GetComponent<SpriteRenderer> ().material;
						c = new Color (m.color.r, m.color.g, m.color.b, m.color.a - 0.05f);
						m.color = c;
					}

				}
				m = GetComponent<Renderer> ().material;
				c = new Color (m.color.r, m.color.g, m.color.b, m.color.a - 0.05f);
				m.color = c;
			} else {
				m = GetComponent<Renderer> ().material;
				Color c = new Color (m.color.r, m.color.g, m.color.b, m.color.a - 0.05f);
				m.color = c;
			}
			if (m.color.a <= 0.0f) {
				fade = false;
				if (disableAndHide) {
					DisableAndHide ();
				}
			}



		}
	}

	public void sing (AudioClip audioclipin)
	{
		GetComponent<AudioSource> ().clip = audioclipin;
		if (GameManager.instance.SFXEnabled ()) {
			if (GetComponent<AudioSource> ().enabled) {
				GetComponent<AudioSource> ().Play ();
				GetComponent<AudioSource> ().pitch = Random.Range (0.95f, 1.05f);
			}
			}
	}

	public void singlow (AudioClip audioclipin)
	{
		if (GameManager.instance.SFXEnabled ()) {
			if (GetComponent<AudioSource> ().enabled) {
				GetComponent<AudioSource> ().clip = audioclipin;
				GetComponent<AudioSource> ().Play ();
				GetComponent<AudioSource> ().pitch = Random.Range (0.5f, 0.55f);
			}
		}
	}

	private void Fade (float to, float time, float delay, Vector3 target)
	{
		
		iTween.FadeTo (gameObject
			, iTween.Hash ("amount", to, "delay", delay, "time", time));
		//SetPlayerObject ();
		if (playerObject != null) {
			playerObject.GetComponent<SpriteRenderer> ().sortingOrder = 0;
		//	iTween.MoveTo (playerObject, iTween.Hash ("position", target, "delay", delay, "time", time));
		}
	}

	public IEnumerator TrigAnim (string trigger)
	{
		
		yield return new WaitForSeconds (0.1f);
		animator.SetTrigger (trigger);


	}

	public void SetAnimTrigger (string trigger)
	{	
		//animator.SetTrigger ("goClear");
		//StartCoroutine (TrigAnim (trigger));
		animator.SetTrigger (trigger);


	}

	public void GrowBigAndFade (Vector3 target, bool fade)
	{
		
		//SetPlayerObject ();
		animator.enabled = false;
		//playerObject.GetComponent<SpriteRenderer> ().sortingOrder = 4;

		iTween.ScaleTo (playerObject, iTween.Hash ("scale", new Vector3 (0.8f, 0.8f, 1.0f), "time", 0.6f));
		iTween.ScaleTo (playerObject, iTween.Hash ("scale", new Vector3 (0.4f, 0.4f, 1.0f), "delay", 0.6f, "time", 0.6f));
		//GetComponent<SpriteRenderer> ().sortingOrder = 4;
		if (fade) {
			iTween.FadeTo (gameObject, iTween.Hash ("amount", 0.0f, "delay", 0.6f, "time", 0.8f));
			iTween.MoveTo (playerObject, iTween.Hash ("position", target, "delay", 0.6f, "time", 0.8f));
		//	iTween.ScaleTo (playerObject, iTween.Hash ("scale", new Vector3 (0.8f, 0.8f, 1.0f), "delay", 1.0f, "time", 0.8f));
		} 


	}

	public void EnableSing ()
	{
		canSing = true;
	}

	public void EnableAnimator ()
	{
		animator.enabled = true;

	}

	public void DisableAndHide ()
	{
		GetComponent<SpriteRenderer> ().sprite = clear;
		animator.enabled = false;
		gameObject.SetActive (false);
		canSing = false;
	}

	void SetSpriteLayer ()
	{
		//GetComponent<SpriteRenderer> ().sortingOrder = 5;
	}
	void SingEmptyFalse() {
		singEmpty = false;
		singEmptyDown = false;
		Material m = GetComponent<Renderer> ().material;
		Color c = new Color (m.color.r, m.color.g, m.color.b,1.0f);
		m.color = c;
		m = singSprite.GetComponent<Renderer> ().material;
		c = new Color (m.color.r, m.color.g, m.color.b, 0.0f);
		m.color = c;
	}

	public void TriggerEmptySing(float y, float duration) {
		singEmpty = true;
		/*
		float d = duration / 2.0f;
		GameObject singSprite = transform.GetChild (0).gameObject;

		iTween.FadeTo (gameObject, iTween.Hash ("amount",0.0f,  "time", d));
		iTween.FadeTo (gameObject, iTween.Hash ("amount",1.0f, "delay", d, "time", d));
		iTween.FadeTo (singSprite, iTween.Hash ("amount",1.0f,  "time", d));
		iTween.FadeTo (singSprite, iTween.Hash ("amount",0.0f, "delay", d, "time", d));
		*/
		Invoke ("SingEmptyFalse", duration);
	}

	public void GrowBigAndAnimate ()
	{
		canSing = false;
		GetComponent<SpriteRenderer> ().sprite = null;

		Invoke ("EnableAnimator", 1.5f);
		Invoke ("EnableSing", 3.0f);
		if (playerObject != null) {
			foreach (Transform child in playerObject.transform) {
				child.GetComponent<SpriteRenderer> ().enabled = false;


				foreach (Transform grandChild in child) {
					grandChild.GetComponent<SpriteRenderer> ().enabled = false;
				}

			}
		}
		SetPlayerObject ();
		EnablePlayerObjectChildrenSprite ();
		//GetComponent<SpriteRenderer> ().sprite = winnerSprite;
		//	Color c = GetComponent<SpriteRenderer> ().material.color;

		//GetComponent<SpriteRenderer> ().material.color = new Color (c.r, c.g, c.b, 0.0f);
		//GetComponent<SpriteRenderer> ().sprite = winnerSprite;
		//iTween.FadeTo (gameObject, iTween.Hash ("amount", 1.0f, "time", 0.4f));
		if (playerObject != null) {
			iTween.ScaleTo (playerObject, new Vector3 (1.2f, 1.2f, 1.0f), 0.8f);
		}
		//	iTween.FadeTo (gameObject, iTween.Hash ("amount", 0.2f, "time", 0.0));



		//	Invoke ("SetSpriteLayer", 0.3f);

		if (winnerTrigger != null) {
			animator.enabled = true;
			//	animator.Stop ();
			animator.Play (winnerTrigger);

			//	GetComponent<Animator> ().SetTrigger (winnerTrigger);
			//GetComponent<Animator> ().enabled = true;

			//GetComponent<Animator> ().CrossFade (winnerTrigger, 0.5f);
		}
		//	iTween.FadeTo (gameObject, iTween.Hash ("amount", 1.0f, "time", 0.8f));
		
		if (particleWinArea.isPlaying) {
			particleWinArea.Stop ();
		}

		particleWinArea.transform.position = transform.position;
		particleWinArea.Play ();

	}

	private void setWin (Sprite sp, bool isBig, bool win, int winner, string trigger, Vector3 target)
	{
		if (!isBig) {	
			if (disableAndHide) {
				fade = true;
				//DisableAndHide ();
				return;
			}

			if (win == false) {
				//DisableAndHide ();
				GetComponent<SpriteRenderer> ().sprite = clear;
				Fade (0, 0.6f,0.4f, target);
				//	if (win != 12) {
				//		return;
				//	}
			}
		}
		if (isBig) {
			_isBig = true;
			GetComponent<BoxCollider2D> ().size = new Vector2 (7.0f, 7.0f);
			GetComponent<Animator> ().enabled = false;
			winnerSprite = sp; 


			winnerTrigger = trigger;
			//GrowBigAndAnimate ();
			Invoke ("GrowBigAndAnimate", 1.0f);
			if (win || winner == 12) {
				GrowBigAndFade (Vector3.zero, false);
				//GetComponent<SpriteRenderer> ().sprite = clear;
				return;
			} else {
				//	DisableAndHide ();
				//Fade (0, 0.6f, 1.2f, target);
				//layerObject.GetComponent<SpriteRenderer> ().sortingOrder = 2;
				//SetPlayerObject ();
				if (playerObject != null) {			
					iTween.FadeTo (playerObject
					, iTween.Hash ("amount", 0, "delay", 0.6f, "time", 0.5f));
				}
			}


		} else {	
			if (win || winner == 12) {
				//DisableAndHide ();	
				GrowBigAndFade (target, true);
			} else {
				Fade (0.0f,0.6f,0.5f, target);
				//	GetComponent<SpriteRenderer> ().sprite = clear;
			}
			Invoke ("DisableAndHide",2.5f);



		}
		 
	}

	public void setWin (int num, bool isBig, Vector3 target)
	{
		int won = num == 10 ? 0 : 1;
		if (isTaken == -1) { 
			disableAndHide = true;
		}
		bool win = isTaken == won;
		isTaken = num;	

		if (num == 10) {
			setWin (oWIN, isBig, win, num, "winO", target);
		} else if (num == 11) {
			setWin (xWIN, isBig, win, num, "winX", target);
		} else {
			setWin (xoWIN, isBig, win, num, "winD", target);

	
			
		}
	}



	public void SetTaken (int winner)
	{
		isTaken = winner;
		//SetPlayerObject ();
		GetComponent<Animator> ().enabled = true;
		SetPlayerObject ();
		EnablePlayerObjectChildrenSprite ();



		//	GetComponentsInChildren<SpriteRenderer> ()[1].sprite = xpynt;
		GetComponent<SpriteRenderer> ().sprite = clear;
		GetComponent<Animator> ().Play ("mark" + playerString);

	}

	void EnablePlayerObjectChildrenSprite ()
	{
		foreach (Transform child in playerObject.transform) {
			child.GetComponent<SpriteRenderer> ().enabled = true;


			foreach (Transform grandChild in child) {
				grandChild.GetComponent<SpriteRenderer> ().enabled = true;
			}

		}
	}

	void SetPlayerObject ()
	{
		if (isTaken == 0) {
	
			playerObject = O;
			playerSprite = o;
			playerString = "O";
		} else if (isTaken == 1) {

			playerObject = X;
			playerSprite = x;
			playerString = "X";
		} else if (isTaken == 11) {
			playerObject = XBig;
			playerSprite = xb;
			playerString = "XBig";
		} else if (isTaken == 10) {
			playerObject = OBig;
			playerSprite = ob;
			playerString = "OBig";
		
		} else if (isTaken == 12) {
			playerObject = DBig;
			playerSprite = db;
			playerString = "DBig";
		} else {
			playerObject = null;
		}
	}

	public void DoOnClick (bool ai)
	{
	
	
			if (isTaken == -1) {
				if (particlesMark.isPlaying) {
					particlesMark.Stop ();

				}
			if (animator != null) {
				animator.enabled = true;
			} else {
				int breakMe = 1;
			}
				particlesMark.transform.position = transform.position;
				particlesMark.Play ();
				int prev = tk.getCurrentTurn ();
				sk.AddScore (1, prev);
				tk.SendBallToTarget (transform.position);
				int turn = tk.getAndIncreaseTurn ();
				if (prev == turn) {
					//	Debug.Log ("Wtf!");
				}
				isTaken = turn;
			if(ai == false) 
				parent.BrickPlayReport (board, yposOnBoard, xposOnBoard);

				SetPlayerObject ();
				EnablePlayerObjectChildrenSprite ();



				//	GetComponentsInChildren<SpriteRenderer> ()[1].sprite = xpynt;
				GetComponent<SpriteRenderer> ().sprite = clear;
				GetComponent<Animator> ().Play ("mark" + playerString);
			
				//	GetComponent<Animator> ().SetTrigger ("goMark" + playerString);

			} else {
				GetComponent<Animator> ().Play ("blink" + playerString);
				GetComponent<AudioSource> ().clip = touchSound;
				if (GameManager.instance.SFXEnabled ()) {
				if(GetComponent<AudioSource>().enabled == true) {
					GetComponent<AudioSource> ().Play ();
					GetComponent<AudioSource> ().pitch = Random.Range (0.85f, 1.25f);
				}
			}

			}

	}

	void OnMouseDown ()
	{
		bool interact = parent.CanBricksInteract ();
		if (interact) {
			DoOnClick (false);
		}
	}
}
