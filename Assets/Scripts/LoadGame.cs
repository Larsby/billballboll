using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
	public enum Fade
	{
		In,
		Out}

	;

	float fadeTime = 1.0F;
	public AudioSource myAudio;
	public Canvas tehCaller;
	public AudioSource buttonsound;

	public float songMaxVolume = 0.385f;


	public IEnumerator FadeAudio (AudioSource source, float timer, Fade fadeType, bool destroySource)
	{
		float start = fadeType == Fade.In ? 0.0F : 1.0F;
		float end = fadeType == Fade.In ? 1.0F : 0.0F;
		float i = 0.0F;
		float step = 1.0F / timer;

		while (i <= 1.0F) {
			i += step * Time.deltaTime;
			if (source != null) {
				source.volume = Mathf.Lerp (start, end, i) * songMaxVolume;
			}
			yield return new WaitForSeconds (step * Time.deltaTime);
		}  
		if (destroySource) {
			Destroy (source);
			source = null;
		}
		//	Destroy (tehCaller);

		 
	}

	public IEnumerator LoadAndDim(float timer, int level) {
		return LoadAndDim (timer, level, false);
	}
	public IEnumerator  LoadAndDim (float timer, int level, bool singlePlayer)
	{
		
		Fader f = gameObject.GetComponent<Fader> ();
		if (f != null) {
			f.fade = true;
			f.dir = 1;
		}
		yield return new WaitForSeconds (timer);
		SceneManager.LoadScene (level);
		//Application.LoadLevel (level);

	}

	public void LoadMenu ()
	{
		if (buttonsound != null)
			buttonsound.Play ();
		StartCoroutine (LoadAndDim (fadeTime, 1));
	}

	public void SetSinglePlayer() {
		GameManager.singlePlayer = true;
	}
	public void SetMultiPlayer() {
		GameManager.singlePlayer = false;
	}

	public void LoadLevel ()
	{
		if (buttonsound != null)
			buttonsound.Play ();
			
		if (GameManager.instance != null) {
			GameManager.instance.SavePrefs ();
		}
		DontDestroyOnLoad (myAudio);
		DontDestroyOnLoad (tehCaller);
	
		StartCoroutine (FadeAudio (myAudio, fadeTime, Fade.Out, true));
		StartCoroutine (LoadAndDim (fadeTime, 2));

		//theButton.SetActive (false);
		tehCaller.enabled = false;
		
	}

	void OnLevelWasLoaded ()
	{
		Fader f = gameObject.GetComponent<Fader> ();
		if (f != null)
			f.dir = -1;
	}
}
