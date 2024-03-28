using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpriteToNoneOnStart : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		gameObject.GetComponent<SpriteRenderer> ().sprite = null;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
