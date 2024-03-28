using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTextureFromCamera : MonoBehaviour
{
	private RenderTexture render;
	private Material mat;
	public Camera cam;
	// Use this for initialization
	void Start ()
	{
 
		if (cam.targetTexture != null)
		{
			cam.targetTexture.Release ();
		}
		cam.targetTexture = new RenderTexture (Screen.width, Screen.height, 24);
		render = cam.targetTexture;
		mat = GetComponent<Renderer> ().material;
		mat.mainTexture = render;
	}
	

}
