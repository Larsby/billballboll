using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongScale : MonoBehaviour {

	// Use this for initialization
	void Start () {
		iTween.ScaleBy(gameObject,iTween.Hash("x",0.90f,"y",0.90f, "time",1f, "loopType","pingPong"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
