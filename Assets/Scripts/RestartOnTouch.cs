using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartOnTouch : MonoBehaviour
{

	void OnMouseDown ()
	{
		GameManager.instance.Restart ();
	}
}
