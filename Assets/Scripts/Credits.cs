using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour {
	int clicks = 0;
	public int index=0;

	// Use this for initialization
	void Start () {
		
	}
	public void RegisterClick() {
		clicks++;
		if (clicks == 5) {
			GameManager.instance.RegiserCredits (index);
			clicks = 0;
		}
	}
	void OnMouseDown() {
		RegisterClick ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
