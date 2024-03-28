using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour {

	// Use this for initialization
	Vector3 pos;
	void Start () {
		RectTransform rt = GetComponent<RectTransform> ();


		pos = rt.position;
	}
	
	// Update is called once per frame
	void Update () {
		bool show = false;
		if (GameManager.instance != null)
			show = GameManager.instance.ShowCredits ();
		
		if (show) {
			RectTransform rt = GetComponent<RectTransform> ();

			rt.position = new Vector3 (rt.position.x, rt.position.y + 0.5f, rt.position.z);	
		}
		if (!show) {
			RectTransform rt = GetComponent<RectTransform> ();

			rt.position = pos;
		}
	}


}
