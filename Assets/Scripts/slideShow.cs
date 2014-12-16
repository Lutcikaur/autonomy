using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GUITexture))]
public class slideShow : MonoBehaviour
{
	public Texture2D[] slides = new Texture2D[0];
	public float changeTime = 10.0f;
	private int currentSlide = 0;
	public int x = 0; 

	void OnGUI(){
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), slides[currentSlide]);
		if (GUI.Button (new Rect (25, 300, 50, 25), "BACK")) {
			Back(); 
		}
		if (GUI.Button (new Rect (25, 325, 50, 25), "NEXT")) {
				Start (); 
		}
		if (GUI.Button (new Rect (25, 350, 50, 25), "MENU")) {
			Application.LoadLevel("Networking"); 
		}
	}
	
	void Start()
	{
		//guiTexture.texture = slides[currentSlide];
		//guiTexture.pixelInset = new Rect(-slides[currentSlide].width, -slides[currentSlide].height, slides[currentSlide].width, slides[currentSlide].height);
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), slides[currentSlide]); 
		currentSlide++;
	}

	void Back()
	{
		//guiTexture.texture = slides[currentSlide];
		//guiTexture.pixelInset = new Rect(-slides[currentSlide].width, -slides[currentSlide].height, slides[currentSlide].width, slides[currentSlide].height);
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), slides[currentSlide]); 
		currentSlide--;
	}
	
	/*void Update()
	{
		if(timeSinceLast > changeTime && currentSlide < slides.Length)
		{
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), slides[currentSlide]);
			//guiTexture.texture = slides[currentSlide];
			//guiTexture.pixelInset = new Rect(-slides[currentSlide].width/2, -slides[currentSlide].height/2, slides[currentSlide].width, slides[currentSlide].height);
			//timeSinceLast = 0.0f;
			currentSlide++;
		}
		// comment out this section if you don't want the slide show to loop
		// -----------------------
		if(currentSlide == slides.Length)
		{
			currentSlide = 0;
		}
		// ------------------------
		timeSinceLast += Time.deltaTime;
	}*/
}