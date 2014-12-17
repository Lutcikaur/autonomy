﻿//------------------------------//
//  TerrainRaycaster.cs         //
//  Written by Alucard Jay      //
//  2014/9/5                    //
//------------------------------//


using UnityEngine;
using System.Collections;
using System;

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
		//check for UI COLLISIONS FIRST
		// option to set the current hexagon to a different texture index
		if(true){
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
					Debug.Log((int)point.x + " " + (int)point.y);
					//hexWorld.game.getNeighbor((int)point.x,(int)point.y);
					if(hexWorld.game.factorySelectionFlag){
						Debug.Log ("sending to fact");
						Building data = hexWorld.hexWorldData[(int)point.x,(int)point.y].building.GetComponent<Building>();
						if(data.name == "factory"){
							Debug.Log ("sending to fact");
							if(data.owner == hexWorld.game.me && data.capCurrent == data.capMax && data.currentlyBuilding == null){
								Debug.Log ("sending to fact");
								data.currentlyBuilding = hexWorld.game.factoryWaitingUnit;
								//hexWorld.game.handList[hexWorld.game.me][hexWorld.game.factoryWaitingUnitLoc];
								hexWorld.game.handList[hexWorld.game.me].RemoveAt(hexWorld.game.factoryWaitingUnitLoc);
								hexWorld.game.factorySelectionFlag = false;
								hexWorld.game.factoryWaitingUnit = null;
								hexWorld.game.factoryWaitingUnitLoc = -1;
								hexWorld.game.factorySelected = -Vector2.one;
							}
							//hexWorld.game.factorySelected = new Vector2(point.x,point.y);
						}
						hexWorld.game.factorySelectionFlag = false;
					}
					if(hexWorld.game.unitSpawnFlag){
						if(hexWorld.hexWorldData[(int)point.x,(int)point.y].unitObject == null){
							switch(hexWorld.game.me){
							default:
								break;
							case 0:
								Vector2[] yep = hexWorld.game.getNeighbor(88,0);
								Vector2[] yep2 = hexWorld.game.getNeighbor(90,0);
								if(Array.IndexOf(yep,point) != -1 || Array.IndexOf(yep2,point) != -1){
									hexWorld.game.spawnFromDepot(point);
								} else {
									hexWorld.game.unitSpawnFlag = false;
								}
								break;
							case 1:
								Vector2[] yyy = hexWorld.game.getNeighbor(89,25);
								Vector2[] yyy2 = hexWorld.game.getNeighbor(91,25);
								if(Array.IndexOf(yyy,point) != -1 || Array.IndexOf(yyy2,point) != -1){
									hexWorld.game.spawnFromDepot(point);
								} else {
									hexWorld.game.unitSpawnFlag = false;
								}
								break;
							}
						}
					}
					if(hexWorld.hexWorldData[(int)point.x,(int)point.y].unitObject != null || hexWorld.hexWorldData[(int)point.x,(int)point.y].building != null){
						selected = point;
					} else {
						selected = -Vector2.one;
					}
				} else if(b==3 && selected != -Vector2.one){
					if(hexWorld.hexWorldData[(int)point.x,(int)point.y].unitObject == null){
						hexWorld.game.moveUnit(selected,point);
						selected=point;
					} else {
						//targeting another unit! See if its an enemy or an ally. Or something?
						if(!hexWorld.game.playerObjects[hexWorld.game.me].Contains(hexWorld.hexWorldData[(int)point.x,(int)point.y].unitObject)){
							hexWorld.game.attackUnit(selected,point);
						}
					}
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
