using UnityEngine;
using System.Collections;
//using Facebook.Unity;

using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class Utilities : MonoBehaviour
{
	public static void Share (string game, string HighScore)
	{
		/*
		Facebook.Unity.FB.FeedShare ("",
			new Uri ("http://www.pastille.se/"),
			"Get "+game+
			",Join me and get "+game+
			", Can you beat my high score of " + HighScore + "?",
			null, null);
	*/
	}
	
	public static void Rate ()
	{

		UniRate r = GameObject.FindObjectOfType<UniRate> ();
		r.ShowPrompt ();
	}
	
	
}