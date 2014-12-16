using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GUITexture))]
public class victory : MonoBehaviour
{
	public Texture2D[] slides = new Texture2D[0];
	public float changeTime = 10.0f;
	private int currentSlide = 0;
	public int x = 0; 
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), slides[currentSlide]);
		if (GUI.Button (new Rect (25, 350, 50, 25), "MENU")) {
			Application.LoadLevel("Networking"); 
		}
	}

}