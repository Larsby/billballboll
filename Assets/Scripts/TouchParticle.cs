using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchParticle : MonoBehaviour
{
	public ParticleSystem particlesTouch;
	public float z = 1.0f;
	public GameObject SoundObject;
	// Use this for initialization
	void Start ()
	{
		#if UNITY_TVOS
		UnityEngine.Apple.TV.Remote.touchesEnabled = true;
		//	UnityEngine.Apple.TV.Remote.reportAbsoluteDpadValues = true;
		UnityEngine.Apple.TV.Remote.allowExitToHome = false;
		#endif
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnMouseDown ()
	{
		if (particlesTouch.isPlaying) {
			particlesTouch.Stop ();
		}
		Vector2 curScreenPoint = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
		Vector2 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint);

		particlesTouch.transform.position = new Vector3 (curPosition.x, curPosition.y, z);
		particlesTouch.Play ();

		 
		SoundObject.GetComponent<PlayRandomSound> ().Play ();

	}
}
