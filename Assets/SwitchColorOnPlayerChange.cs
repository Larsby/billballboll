using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchColorOnPlayerChange : MonoBehaviour {
	public Color yellowPlayer;
	public Color pinkPlayer;
	private Color fromColor;
	private Color toColor;
	private bool doLerp = false;
	int currentPlayer;
	private SpriteRenderer rend;
	// Use this for initialization
	void Start () {
		currentPlayer = -1;
		rend = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.instance != null) {
			int cp =  GameManager.instance.CurrentPlayer();
			if (cp != currentPlayer) {
				fromColor = cp == 0 ? pinkPlayer : yellowPlayer;
				toColor = cp == 0 ? yellowPlayer : pinkPlayer;
				doLerp = true;
			}
		}
		if (doLerp) {
			rend.color = Color.Lerp (fromColor, toColor, Time.time);
		}
	}
}
