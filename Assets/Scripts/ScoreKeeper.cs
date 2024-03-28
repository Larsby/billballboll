using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
	public RectTransform scoreX;
	public RectTransform scoreO;
	public Color dimColor;
	public Color normalColor;
	public int slowCount = 15;
	Text current;
	Text prev;
	Color currentColor;
	private int[] score = new int [2]{ 0, 0 };
	private int intScore;
	public GameObject CoinSoundObject;
	private PlayRandomSound coinSound;
	public Color[] colors;
	private int colorIndex;
	private int pulsateIndex = 0;
	bool pulsateScore = false;
	public GameObject scoreXParticle;
	public GameObject scoreOParticle;
	// Use this for initialization
	void Start ()
	{
		current = scoreX.GetComponent<Text> ();
		coinSound = CoinSoundObject.GetComponent<PlayRandomSound> ();
		//scoreXColor = new Color (scoreXColor.r / 255, scoreXColor.g / 255, scoreXColor.b / 255);
		//scoreOColor = new Color (scoreOColor.r / 255, scoreOColor.g / 255, scoreOColor.b / 255);
		normalColor = new Color (normalColor.r, normalColor.g, normalColor.b);
		pulsateScore = false;
	}

	public void SetColor (int index)
	{
		currentColor = normalColor;
		Text text = (index == 1 ? scoreO.GetComponent<Text> () : scoreX.GetComponent<Text> ());
		text.color = new Color (currentColor.r, currentColor.g, currentColor.b, 0.6f);
		Text text2 = text == scoreO.GetComponent<Text> () ? scoreX.GetComponent<Text> () : scoreO.GetComponent<Text> ();

		text2.color = normalColor;
	}

	public void Pulsate (int index, int cindex)
	{
		Text text = (index == 1 ? scoreO.GetComponent<Text> () : scoreX.GetComponent<Text> ());
		if (cindex >= colors.Length) {
			cindex = colors.Length - 1;
		}
		Color c = colors [cindex];
		text.color = new Color (c.r, c.g, c.b);
	}

	public void AddOneScore ()
	{ pulsateScore = true;
		prev.text = "" + intScore++;
		if (GameManager.instance.SFXEnabled ()) {
			coinSound.Play ();
		}
		Invoke ("SetColor", 0.1f);
		Invoke ("TurnOfPulsate", 0.3f);
	}

	public void SetColor ()
	{
		SetColor (colorIndex);

	}
	public void TurnOfPulsate ()
	{
		SetColor (colorIndex);
		
		pulsateScore = false;
	}
	public void AddScore (int value, int index)
	{ 
		pulsateScore = true;
		if (index == -1)
			return;
		intScore = score [index];
		score [index] += value;
		current = (index == 0 ? scoreX.GetComponent<Text> () : scoreO.GetComponent<Text> ());
		//StartStopParticleSystem sys = index == 0 ? scoreX.GetComponent<StartStopParticleSystem> () : scoreO.GetComponent<StartStopParticleSystem> ();
		//	sys.Ignite (1.0f);
			prev = (index == 1 ? scoreO.GetComponent<Text> () : scoreX.GetComponent<Text> ());
		GameObject particle = index == 0 ? scoreXParticle : scoreOParticle;
		//particle.transform.position = current.transform.position;

		if (value == 1) {
			current.text = "" + score [index];
			coinSound.Play ();
			Invoke ("TurnOfPulsate", 0.6f);
			return;
		}
		float time = 0;
		for (int i = 0; i <= value; i++) {
			float slowdown = 0;
			if (slowCount < value) {
				if (value - i < slowCount) {
					slowdown = Random.Range (0.5f, 1.2f);
				}
			}
			float randomTime = Random.Range (slowdown + 0.5f, slowdown + 1.0f);
			//Pulsate (index, i % 9);
			time += randomTime;
			Invoke ("AddOneScore", randomTime);
			pulsateScore = true;
		}
		colorIndex = index;



	}

	public void ResetScore ()
	{
		score [0] = 0;
		score [1] = 0;
		scoreX.GetComponent<Text> ().text = "" + score [0];
		scoreO.GetComponent<Text> ().text = "" + score [1];
	}

	public void RemoveScore (int value, int index)
	{
		score [index] -= value;
		if (score [index] < 0)
			score [index] = 0;
		current = (index == 0 ? scoreX.GetComponent<Text> () : scoreO.GetComponent<Text> ());
		current.text = "" + score [index];
	}

	void IncreaseePulsateIndex ()
	{
		pulsateIndex++;
	}
		
	// Update is called once per frame
	void Update ()
	{
		if (pulsateScore) {
			
			Pulsate (GameManager.instance.CurrentPlayer () == 0 ? 1 : 0, pulsateIndex);
			Invoke ("IncreaseePulsateIndex", 0.3f);

			if (pulsateIndex > 9)
				pulsateIndex = 0;
		}
		
	}
}
