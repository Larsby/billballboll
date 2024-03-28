using UnityEngine;
using System.Collections;

public class RandomMove : MonoBehaviour
{

	public float rotationSpeed;
	public float movementSpeed;
	public float rotationTime;
	public Camera boundsCamera;
	private Bounds bounds;


	void Start ()
	{
		Invoke ("ChangeRotation", rotationTime);

		float screenAspect = (float)Screen.width / (float)Screen.height;
		float cameraHeight = boundsCamera.orthographicSize * 2;
		bounds = new Bounds (
			new Vector3 (boundsCamera.transform.position.x, boundsCamera.transform.position.y, -5.08f),
			new Vector3 (cameraHeight * screenAspect, cameraHeight, 5.08f));
	
	}

	void ChangeRotation ()
	{
		if (Random.value > 0.5f)
		{
			rotationSpeed = -rotationSpeed;
		}
		Invoke ("ChangeRotation", rotationTime);
	}


	void Update ()
	{
		transform.Rotate (new Vector3 (0, 0, rotationSpeed * Time.deltaTime));
		transform.position += transform.up * movementSpeed * Time.deltaTime;

		if (!bounds.Contains (transform.position))
		{
			
			//transform.position = bounds.center;//new Vector3(transform.position.x,transform.position.y,transform.position.z)
			transform.Rotate (new Vector3 (0, 0, 180));
		}
 			

	}
}