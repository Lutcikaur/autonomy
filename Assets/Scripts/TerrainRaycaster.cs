//------------------------------//
//  TerrainRaycaster.cs         //
//  Written by Alucard Jay      //
//  2014/9/5                    //
//------------------------------//


using UnityEngine;
using System.Collections;

public class TerrainRaycaster : MonoBehaviour 
{
	public HexWorld hexWorld;
	public Transform cameraTx;

	public float indicatorTimerMax = 0.1f;

	private float indicatorTimer = 0f;

	public Vector2 selected = -Vector2.one;

	private Vector3 lastRayPos = Vector3.zero;
	private Vector3 thisRayPos = Vector3.zero;

	public Vector2 hexDataPos = Vector2.zero;
	
	
	//	-------------------------------------------------------  Persistent Functions
	
	
	void Awake() 
	{
		Initialize();
	}
	
	
	void Start() 
	{
		
	}
	
	
	void Update() 
	{
		CheckForMouseClick();

		ScrollCamera();

		UpdateIndicator();
	}
	
	
	//	-------------------------------------------------------  Other Functions
	
	
	void Initialize() 
	{
		if ( !hexWorld )
			hexWorld = GetComponent< HexWorld >();

		if ( !cameraTx )
			cameraTx = Camera.main.transform;
	}
	
	
	void CheckForMouseClick() 
	{
		// option to set the current hexagon to a different texture index
		if ( Input.GetMouseButtonDown(0) )
			// change hexagon to green
			RaycastTerrain( 2 );
		else if ( Input.GetMouseButtonDown(1) )
			// change hexagon to red
			RaycastTerrain( 3 );
		else if ( Input.GetMouseButtonDown(2) )
			// change hexagon to default
			RaycastTerrain( 0 );
	}
	
	
	void RaycastTerrain( int b ) 
	{
		indicatorTimer = 0f; // reset timer
		Vector2 point;
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;
		
		if ( Physics.Raycast( ray, out hit ) )
		{
			if ( hit.collider.gameObject.name == "Terrain" )
			{
				// store this position
				thisRayPos = hit.point;
				
				// change lastRayPos back to the last value stored in hexWorldData (send -1)
				if ( lastRayPos != Vector3.zero ){
					point = hexWorld.findHex(lastRayPos);
					hexDataPos = hexWorld.SetHexUVs( point, -1 );
					//selected = -Vector2.one;
				}
					
				
				// set the texture UV based on what button was clicked
				point = hexWorld.findHex(thisRayPos);
				//
				if(b==2){
					selected = point;
				} else if(b==3){
					hexWorld.hexWorldData[(int)point.x,(int)point.y].unitObject = hexWorld.hexWorldData[(int)selected.x,(int)selected.y].unitObject;
					hexWorld.hexWorldData[(int)point.x,(int)point.y].unit = hexWorld.hexWorldData[(int)selected.x,(int)selected.y].unit;
					Debug.Log (hexWorld.hexWorldData[(int)selected.x,(int)selected.y].unit);
					Debug.Log (hexWorld.hexWorldData[(int)point.x,(int)point.y].unit);
					hexWorld.hexWorldData[(int)point.x,(int)point.y].unitObject.transform.position = new Vector3 (hexWorld.hexWorldData[(int)point.x,(int)point.y].center.x, 1 , hexWorld.hexWorldData[(int)point.x,(int)point.y].center.y);
				} else {
					hexWorld.SetHexUVs( point, b );
				}
					
				// update lastRayPos
				lastRayPos = thisRayPos;
			}
			else
			{
				// change lastRayPos back to the last value stored in hexWorldData (send -1)
				if ( lastRayPos != Vector3.zero ){
					point = hexWorld.findHex(lastRayPos);
					hexWorld.SetHexUVs( point, -1 );
					//selected = -Vector2.one;
				}
					
				
				lastRayPos = Vector3.zero;
				hexDataPos = -Vector2.one;
			}
		}
		else
		{
			// change lastRayPos back to the last value stored in hexWorldData (send -1)
			if ( lastRayPos != Vector3.zero ){
				point = hexWorld.findHex(lastRayPos);
				hexWorld.SetHexUVs( point, -1 );
				//selected = -Vector2.one;
			}
			
			lastRayPos = Vector3.zero;
			hexDataPos = -Vector2.one;
		}
	}
	
	
	void ScrollCamera() 
	{
		Vector3 cameraPos = cameraTx.position;
		
		cameraPos.x += Input.GetAxis( "Horizontal" ) * 5f * cameraPos.y * Time.deltaTime;
		cameraPos.z += Input.GetAxis( "Vertical" ) * 5f * cameraPos.y * Time.deltaTime;
		
		cameraPos += Input.GetAxis( "Mouse ScrollWheel" ) * cameraTx.forward * 20f * cameraPos.y * Time.deltaTime;
		
		cameraTx.position = cameraPos;
	}
	
	
	void UpdateIndicator() 
	{
		indicatorTimer += Time.deltaTime;
		
		if ( indicatorTimer < indicatorTimerMax )
			return;

		// set UV to hover
		RaycastTerrain( 1 );
	}
}
