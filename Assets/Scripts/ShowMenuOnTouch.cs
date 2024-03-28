using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ShowMenuOnTouch : MonoBehaviour
{

	private bool toggle;
	public GameObject thePnl;
	public Button button;
	public Sprite show;
	public Sprite hide;
	public Button activeButton;
	public UnityEngine.EventSystems.EventSystem system;

	void Start ()
	{
		toggle = true;
	}

	public void ToggleMenu ()
	{
		
		thePnl.active = toggle;
		GameManager.instance.SetInteractionEnabled (!toggle);
		if (toggle) {
			GameManager.instance.SetMenuState ();
		}
		if (toggle) {
			button.image.sprite = hide;
		//	activeButton.gameObject.SetActive (true);
			system.SetSelectedGameObject (null);


		} else { 
			button.image.sprite = show;
		}
		toggle = !toggle;

	}
	public bool isMenuShowing() {
		return !toggle;
	}

	void OnMouseDown ()
	{
		ToggleMenu ();
	}


}
