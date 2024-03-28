using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnKeeper : MonoBehaviour
{

	int turn = 0;
	int tick = 0;
	public GameObject KingO;
	public GameObject KingX;
	public GameObject sourceSpriteO;
	public GameObject sourceSpriteX;
	private GameObject currrentKing;
	public float fadeTime = 0.4f;
	public float moveTime = 0.8f;
	public Sprite kingOSphere;
	public Sprite kingXSphere;
	public AudioSource sayBill;
	public AudioSource sayBoll;
	public GameObject trail;
	public GameObject SoundsObject;
	public GameObject SphereXGameObject;
	public GameObject SphereOGameObject;
	public GameObject landingSounds;
	public float sphereBallTimeToFade = 0.8f;
	public GameObject XRainbowParticleObject;
	public GameObject ORainbowParticleObject;

	public GameObject k_o_x;
	public GameObject k_o_o;

	private PlayRandomSound sound;
	private AnimState animState = AnimState.PLAY;

	private GameObject CK;
	private GameObject NK;

	enum AnimState
	{
		PLAY,
		VICTORY}

	;

	public class Ball
	{
		public Transform originalPosition;
		public GameObject gameObj;
		public Transform originalParent;
		public GameObject dubbelGanger;
		public Sprite sprite;
	
	};

	private Ball originalBall;

	void Start ()
	{
		currrentKing = KingX;
		//currrentKing.GetComponent<Animator> ().SetTrigger ("goKingActive");
		originalBall = null;
		animState = AnimState.PLAY;
		sound = SoundsObject.GetComponent<PlayRandomSound> ();
		XRainbowParticleObject.GetComponent<StartStopParticleSystem> ().SetEnabled (true);
	}

	void Init ()
	{
	}

	public void ActivateVictory (int winner)
	{
		GameObject king = (winner == 1 ? KingX : KingO);
		GameObject looser = (winner == 1 ? KingO : KingX);
		animState = AnimState.VICTORY;


		king.GetComponent<Animator> ().CrossFade ("KingVictory", 0.5f);
	
		looser.GetComponent<Animator> ().CrossFade ("KingSleep", 0.5f);
		/*	if (winner != turn) {
			turn = turn == 1 ? 0 : 1;
		}
		*/

	}

	public void Restart ()
	{
		currrentKing = KingX;
		KingO.GetComponent<Animator> ().SetTrigger ("goKingSleep");
		currrentKing.GetComponent<Animator> ().SetTrigger ("goKingActive");
		originalBall = null;
		animState = AnimState.PLAY;
		CK = null;
		NK = null;
		turn = 0;
	}

	public void TurnoffSprite ()
	{
		//	originalBall.gameObj.GetComponent<SpriteRenderer> ().sprite = null;
		landingSounds.GetComponent<PlayRandomSound> ().Play ();
	}

	public void SendBallToTarget (Vector3 target)
	{
		GameObject kingSphere = turn == 0 ? k_o_x : k_o_o;
		GameObject sphereBall = turn == 0 ? SphereOGameObject : SphereXGameObject;
		Color c = sphereBall.GetComponent<SpriteRenderer> ().material.color;
		sphereBall.GetComponent<SpriteRenderer> ().material.color = new Color (c.r, c.g, c.b, 0.0f);

		iTween.FadeTo (sphereBall, 1.0f, sphereBallTimeToFade);

		kingSphere.GetComponent<SphereParticleController> ().SetEnabledFast (false, 0.72f);
		if (GameManager.instance.SFXEnabled ()) {

			sound.Play (); //  detta är swoosh sound för aktuell kung.

		}

		foreach (Transform child in currrentKing.transform) {
			Transform dobbelGanger = null;
			foreach (Transform grandchild in child.transform) {
				if (grandchild.CompareTag ("KingBalloriginal")) {
					dobbelGanger = grandchild;

				}
				if (grandchild.CompareTag ("KingBall")) {
					trail.transform.position = grandchild.transform.position;
					trail.transform.GetChild (0).position = trail.transform.position;
					trail.GetComponent<StartStopParticleSystem> ().Ignite (moveTime);
					//grandchild.transform.position = target;
					if (originalBall != null) {
						originalBall.gameObj.transform.parent = originalBall.originalParent;
						originalBall.gameObj.transform.localPosition = turn == 1 ? SphereXGameObject.transform.localPosition : SphereOGameObject.transform.localPosition;
						originalBall.gameObj.GetComponent<SpriteRenderer> ().sprite = null;
						Vector3 scale = originalBall.dubbelGanger.transform.localScale;

					
						//iTween.FadeTo (originalBall.dubbelGanger, 1.0f, 0.0f);
						originalBall.dubbelGanger.GetComponent<SpriteRenderer> ().sprite = originalBall.sprite;
						//	iTween.FadeTo (originalBall.gameObj, 1f, 0.0f);
						//	originalBall.originalPosition.gameObject.GetComponent<Renderer> ().enabled = true;
						//	originalBall.dubbelGanger.GetComponent<SpriteRenderer> ().sprite = originalBall.sprite;
					}
					//iTween.FadeTo (grandchild.gameObject, 1.0f, 0.0f);
					originalBall = new Ball ();
					originalBall.gameObj = grandchild.gameObject;
					Sprite theSprite = kingXSphere;
					if (currrentKing == KingO) {
						theSprite = kingOSphere;
					}
				

					grandchild.gameObject.GetComponent<SpriteRenderer> ().sprite = theSprite;
					originalBall.sprite = theSprite;
					dobbelGanger.GetComponent<SpriteRenderer> ().sprite = null;
					originalBall.originalParent = grandchild.parent;
					originalBall.originalPosition = dobbelGanger;
					//	dobbelGanger.gameObject.GetComponent<Renderer> ().enabled = false;
					originalBall.dubbelGanger = dobbelGanger.gameObject;
					grandchild.gameObject.GetComponent<SpriteRenderer> ().material.color = new Color (c.r, c.g, c.b, 1.0f);
					iTween.MoveTo (grandchild.gameObject, target, moveTime);

					iTween.MoveTo (trail.transform.GetChild (0).gameObject, target, moveTime);
					iTween.FadeTo (grandchild.gameObject, 0f, fadeTime);
					//iTween.FadeTo (trail, 0f, fadeTime);
					Invoke ("TurnoffSprite", fadeTime - 0.1f);
					//	grandchild.position = target;
					grandchild.transform.parent = null;

					// animate 

				}
			}
		}
		GameObject nextKing = currrentKing == KingX ? KingO : KingX;
		kingSphere = turn == 0 ? k_o_o : k_o_x;
		kingSphere.GetComponent<SphereParticleController> ().SetEnabledFast (true, 0.24f);
		foreach (Transform child in nextKing.transform) {
			Transform dobbelGanger = null;
			foreach (Transform grandchild in child.transform) {

				if (grandchild.CompareTag ("KingBalloriginal")) {
					Sprite theSprite = kingXSphere;
					if (nextKing == KingO) {
						theSprite = kingOSphere;
					}


					grandchild.gameObject.GetComponent<SpriteRenderer> ().sprite = theSprite;
				}
			}
		}
	}
	public void SetPlayerTurn() {
		turn = 0;
	}
	private void TurnOffSourceBall ()
	{
		foreach (Transform child in currrentKing.transform) {

			foreach (Transform grandchild in child.transform) {

				if (grandchild.CompareTag ("KingBall")) {
					//grandchild.gameObject.GetComponent<Renderer> ().enabled = false;
				}
			}
		}

	}

	private void PlayAudio ()
	{
		if (turn == 0) {
			sayBill.Play ();
			sayBill.pitch = Random.Range (0.8f, 1.2f);
		} else {
			sayBoll.Play ();
			sayBoll.pitch = Random.Range (0.8f, 1.2f);
		}
	}

	private void ActivateKing ()
	{
		TurnOffSourceBall ();
		GameObject sprite = (turn == 0 ? sourceSpriteX : sourceSpriteO);
		//	currrentKing.GetComponent<Animator> ().SetTrigger ("goKingActive");
		PlayAudio ();
	}

	private void switchActiveKing ()
	{   
		

		animState = AnimState.PLAY;
		//currrentKing.GetComponent<Animator> ().SetTrigger ("goKingSleep");
		currrentKing = (turn == 0 ? KingO : KingX);
		ActivateKing ();

	}

	public void newGameTurn (int newTurn)
	{	
		GameObject king = (turn == 1 ? KingX : KingO);
		GameObject looser = (turn == 1 ? KingO : KingX);
		animState = AnimState.VICTORY;


		king.GetComponent<Animator> ().CrossFade ("KingSleep", 0.5f);

		looser.GetComponent<Animator> ().CrossFade ("KingActive", 0.5f);
		
		turn = newTurn;
		//currrentKing.GetComponent<Animator> ().SetTrigger ("goKingSleep");
		int newKing = newTurn == 0 ? 1 : 0;
		currrentKing = newKing == 1 ? KingX : KingO;
		ActivateKing ();
		animState = AnimState.PLAY;
	}

	public int getCurrentTurn ()
	{
		return turn;
	}

	public IEnumerator TurnOfClickedKingState (GameObject king, string action, float delay)
	{
		yield return new WaitForSeconds (delay);
		king.GetComponent<Animator> ().CrossFade (action, 0.1f);
	}

	public void KingClicked (GameObject king)
	{
		GameObject current = turn == 0 ? KingX : KingO;
		string action = "KingActive";

		if (current != king) {
			action = "KingSleep";
		}
		king.GetComponent<Animator> ().CrossFade ("KingClick", 0.1f);
		StartCoroutine (TurnOfClickedKingState (king, action, 1f));
	}
	public int GetTick() 
	{
		return tick;
	}
	public int IncreateseTurn() {
		turn = turn == 1 ? 0 : 1;
		tick = tick == 1 ? 0 : 1;
		return turn;

	}
	public void SetAI() {
		if (turn != 1) {
			getAndIncreaseTurn ();	
		}
	}
	public int getAndIncreaseTurn ()
	{
		switchActiveKing ();

		return IncreateseTurn();


	}

	void Update ()
	{
		if (animState != AnimState.VICTORY) {
			GameObject ck = turn == 0 ? KingX : KingO;
			GameObject nk = turn == 0 ? KingO : KingX;
			if (CK != ck || NK != nk) {
				ck.GetComponent<Animator> ().CrossFade ("KingActive", 0.5f);
				nk.GetComponent<Animator> ().CrossFade ("KingSleep", 0.5f);
				GameObject activeRainbow = turn == 0 ? XRainbowParticleObject : ORainbowParticleObject;
				GameObject inactiveRainbow = turn == 0 ? ORainbowParticleObject : XRainbowParticleObject;
				activeRainbow.GetComponent<StartStopParticleSystem> ().SetEnabled (true);
				inactiveRainbow.GetComponent<StartStopParticleSystem> ().SetEnabled (false);
				CK = ck;
				NK = nk; 
			}
		}
		
	}
}
