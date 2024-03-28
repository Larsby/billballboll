using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustScreenSize : MonoBehaviour
{
	bool recalc = false;
	// Use this for initialization
	void Start ()
	{
		recalc = false;
	}

	void Update ()
	{
		Vector3 viewPos = Camera.main.WorldToViewportPoint (transform.position);

		if ((viewPos.x < 0.0f || viewPos.x > 0.995f) || (viewPos.y < 0.05f || viewPos.y > 0.995f)) {
			Camera.main.orthographicSize += 0.1f;
			recalc = true;


		} else {
			if (recalc) {
				recalc = false;

				//Camera.main.transform.parent = null;
			} 	
		} 

	}
}
