using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallTurnKeeperOnClick : MonoBehaviour
{

	public GameObject kissSounds;

	// Use this for initialization
	void Start ()
	{
		
	}
	public void Kiss() {
		kissSounds.GetComponent<PlayRandomSound> ().Play ();
		GameManager.instance.KingClicked (gameObject);
	}

	void OnMouseDown ()
	{
		Kiss ();
	}
	// Update is called once per frame
	void Update ()
	{
		
	}
}
