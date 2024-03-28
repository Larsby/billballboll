using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class VideoManager : MonoBehaviour {
	VideoPlayer videoPlayer;
	void EndReached (UnityEngine.Video.VideoPlayer vp)
	{	
		LoadMenu ();
	}
	/*
	void VideoPlayerFrameReady(UnityEngine.Video.VideoPlayer vp, long frameIndex) {
		if (frameIndex > 1) {
			//SceneBlocker.SetActive (false);
			videoPlayer.sendFrameReadyEvents = false;
		}
	}
*/
	void LoadMenu() {
		AsyncOperation op = SceneManager.LoadSceneAsync (1, LoadSceneMode.Single);
	}

	void Start() {
		PlayVideo ();
	}
	void PlayVideo ()
	{
		

		
		//videoPlane.SetActive (true);
		videoPlayer = GetComponent<VideoPlayer> ();
		//videoPlayer.sendFrameReadyEvents = true;
		videoPlayer.loopPointReached += EndReached;
		//videoPlayer.frameReady += VideoPlayerFrameReady;
		//	double duration = player.clip.length;
		videoPlayer.Play ();
		//StartCoroutine(StartGameWithDelay(duration));
	}

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			LoadMenu ();
		}
		#if UNITY_TVOS
		if (Input.GetKeyDown (KeyCode.JoystickButton14)) {
			LoadMenu();
		}
	
		#endif

	}

}
