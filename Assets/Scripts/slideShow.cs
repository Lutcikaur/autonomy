using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GUITexture))]
public class slideShow : MonoBehaviour
{
	public Texture2D[] slides = new Texture2D[1];
	public float changeTime = 10.0f;
	private int currentSlide = 0;
	private float timeSinceLast = 1.0f;

	void Start()
	{
		guiTexture.texture = slides[currentSlide];
		//guiTexture.pixelInset = new Rect(10, 0, Screen.width, Screen.height);
		//GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), slides[currentSlide]); 
		currentSlide++;
	}
	
	void Update()
	{
		if(timeSinceLast > changeTime && currentSlide < slides.Length)
		{
			//GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), slides[currentSlide]);
			guiTexture.texture = slides[currentSlide];
			//guiTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
			timeSinceLast = 0.0f;
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
	}
}