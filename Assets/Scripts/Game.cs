using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	// Use this for initialization
	public HexWorld hexWorld;
	public TerrainRaycaster terrainCaster;
	public List<List<GameObject>> playerObjects = new List<List<GameObject>>{};
	//public GameObject Cylinder;
	public int me = -1;
	public List<List<string>> depotList = new List<List<string>>{};
	public static string server = null;
	public int turn = 0;

	//map bounds inclusive
	public int xLowerBound = 80;
	public int xUpperBound = 99;
	public int yLowerBound = 0;
	public int yUpperBound = 25;


	public int arbitraryTempOne=0;
	public bool arbitraryTempThree=false;

	void Start () {
		server = Menu.server;
		Debug.Log("Here");
		switch(Network.peerType){
		default:
		case NetworkPeerType.Disconnected:

			break;
		case NetworkPeerType.Client:
			break;
		case NetworkPeerType.Server:
			//We have to null then build up the connection list to clients first.
			networkView.RPC("Clear",RPCMode.OthersBuffered);
			for(int i = 0; i < Menu.connectionList.Count; i++){
				networkView.RPC("AddConnectionList",RPCMode.OthersBuffered,Menu.connectionList[i].guid,Menu.connectionList[i].username);
			}
			networkView.RPC("Initialize",RPCMode.OthersBuffered);
			//some test stuff.
			for(int i = 0; i < Menu.connectionList.Count; i++){
				playerObjects.Add(new List<GameObject>());
				depotList.Add (new List<string>());
				for(int j = 0; j<5; j++){
					string name = "Vtol";
					string guid = Menu.connectionList[i].guid;
					Vector3 location = new Vector3(i,1,j);
					networkView.RPC("SpawnObject",RPCMode.All,i,guid,name,location);
				}
			}
			break;
		}

	}

	public Vector2[] getNeighbor(int x, int y)
	{
		Vector2[] neighborList = new Vector2[6];
		int numberOfNeighbors = 0;
		bool odd = false;
		
		if ((y % 2) > 0) {
			odd = true;
		}
		
		//Debug.Log ("START: " + x + " " + y + " " + hexWorld.hexWorldData[x,y].height);
		//adds x neighbors
		if(x-1 >= xLowerBound && x-1 <= xUpperBound && y <= yUpperBound && y >= yLowerBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y].x,hexWorld.hexWorldData[x-1,y].y);
			//Debug.Log("A: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		if(x+1 <= xUpperBound && x+1 <= xUpperBound && y <= yUpperBound && y >= yLowerBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y].x,hexWorld.hexWorldData[x+1,y].y);
			//Debug.Log("B: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		//adds y neighbors
		if(y-1 >= yLowerBound && x <= xUpperBound && x <= xUpperBound && y-1 <= yUpperBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x,y-1].x,hexWorld.hexWorldData[x,y-1].y);
			Debug.Log("C: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		if(y+1 <= yUpperBound && x <= xUpperBound && x <= xUpperBound && y+1 >= yLowerBound){
			neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x,y+1].x,hexWorld.hexWorldData[x,y+1].y);
			//Debug.Log("D: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
			numberOfNeighbors++;
		}
		
		//gets y right neighbors for odd
		if(odd == true) {
			//Debug.Log ("If");
			if(x+1 <= xUpperBound && y-1 >= yLowerBound && x+1 >= xLowerBound && y-1 <= yUpperBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y-1].x,hexWorld.hexWorldData[x+1,y-1].y);
				//Debug.Log("E: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
			
			if(x+1 <= xUpperBound && y+1 <= yUpperBound && x+1 >= xLowerBound && y+1 >= yLowerBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x+1,y+1].x,hexWorld.hexWorldData[x+1,y+1].y);
				//Debug.Log("F: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
		} else {
			//Debug.Log ("Else");
			//gets y left neighbors for even
			if(x-1 <= xUpperBound && y+1 <= yUpperBound && x-1 >= xLowerBound && y+1 >= yLowerBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y+1].x,hexWorld.hexWorldData[x-1,y+1].y);
				//Debug.Log("G: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
			
			if(x-1 <= xUpperBound && y-1 >= yLowerBound && x-1 <= xUpperBound && y-1 <= yUpperBound){
				neighborList[numberOfNeighbors] = new Vector2(hexWorld.hexWorldData[x-1,y-1].x,hexWorld.hexWorldData[x-1,y-1].y);
				//Debug.Log("H: " + neighborList[numberOfNeighbors].x + " " + neighborList[numberOfNeighbors].y);
				numberOfNeighbors++;
			}
		}
		
		for(int i=0; i<numberOfNeighbors; i++)
		{
			hexWorld.SetHexUVs(neighborList[i],2);
		}
		return neighborList;
	}

	/*
	deck RandomizeDeck(deck d){
		int i,j;
		int iMin;
		var r=new Random();
		for (i=0; i<deckSize; i++){
			d[i].randomVal=r.Next(1000);
		}
		deck TempDeck=new deck;

		// a[0] to a[n-1] is the array to sort 

		
		// advance the position through the entire array 
		//   (could do j < n-1 because single element is also min element) 
		for (j = 0; j < deckSize; j++) {
			// find the min element in the unsorted a[j .. n-1] 
			
			// assume the min is the first element 
			iMin = j;
			// test against elements after j to find the smallest 
			for ( i = j+1; i <= deckSize; i++) {
				// if this element is less, then it is the new minimum   
				if (d[i].randomVal < d[iMin].randomVal) {
					// found new minimum; remember its index 
					iMin = i;
				}
			}
			
			if(iMin != j) {
				//swaps d[j]& d[iMin];
				TempDeck[0].index=d[j].index;
				TempDeck[0].randomVal=d[j].randomVal;
				d[j].index=d[iMin].index;
				d[j].randomVal=d[iMin].randomVal;
				d[iMin].index=TempDeck[0];
				d[iMin].randomVal=TempDeck[0];
			}
			
		}



		return(d);
	}

	*/
	void OnGUI() {
		int i = 0;
		float x=Screen.width;
		float y=Screen.height;
		switch(Network.peerType){
		default:
		case NetworkPeerType.Disconnected:
			GUI.Box (new Rect(0, y-(x*.2f), x*.2f, x*.2f), "Depot goes here?");
			GUI.Box (new Rect(x-(x*.3f), y-(y*.25f), x*.15f, y*.25f), "Unit Details & abilities goes here?");
			if(GUI.Button (new Rect(x*.2f, y-(y*.25f), x*.5f, y*.25f), "Hand of Cards Goes Here?"))
			{
				if(arbitraryTempOne ==0){
					arbitraryTempThree=true;
					arbitraryTempOne++;
					//arbitraryTempTwo++;
					Debug.Log("Is this happeneing 1");
				} else if (arbitraryTempOne==1){
					arbitraryTempThree=false;
					arbitraryTempOne--;
					//arbitraryTempTwo++;
					Debug.Log("Is this happeneing 3");
				}


			}
			if(arbitraryTempThree){
				GUI.Box (new Rect(x*.2f, y*.25f, x*.65f, y*.5f), "Enlarged Hand");
				Debug.Log("Is this happeneing 2");
				//arbitraryTempTwo--;
			}

			//GUI.Box (new Rect(x*.1f, y*.25f, x*.65f, y*.5f), "Enlarged Hand");

			break;
		case NetworkPeerType.Client:
			for(i=0;i<Menu.connectionList.Count;i++){
				int offset = i*25;
				GUI.Label(new Rect(10,(10+offset),100,25),Menu.connectionList[i].username);
				//button to kick
				//playerObjects[_i][playerObjects[_i].Count-1].transform.position = _location;
				//for(int j=0;j<depotList[i].Count;j++){
				//	GUI.Label(new Rect(100+(100*j),(10+offset),100,25),depotList[i][j]);
				//}
			}

			//GUI.Box (new Rect(50,50,100,90), "Loader Menu");
			GUI.Box (new Rect(0, y-(x*.2f), x*.2f, x*.2f), "Depot goes here?");
			GUI.Box (new Rect(x-(x*.3f), y-(y*.25f), x*.15f, y*.25f), "Unit Details & abilities goes here?");
			if(GUI.Button (new Rect(x*.2f, y-(y*.25f), x*.5f, y*.25f), "Hand of Cards Goes Here?"))
			{
				if(arbitraryTempOne ==0){
					arbitraryTempThree=true;
					arbitraryTempOne++;

					Debug.Log("Is this happeneing 1");
				} else if (arbitraryTempOne==1){
					arbitraryTempThree=false;
					arbitraryTempOne--;

					Debug.Log("Is this happeneing 3");
				}
				
				
			}
			if(arbitraryTempThree){
				GUI.Box (new Rect(x*.2f, y*.25f, x*.65f, y*.5f), "Enlarged Hand");
				Debug.Log("Is this happeneing 2");

			}
			if(GUI.Button(new Rect(x-100, y-40, 80, 20), "Pass Turn")) {
				//PUT PASS TURN CALL HERE
			}

			break;
		case NetworkPeerType.Server:
			GUI.Label(new Rect(100,100,100,25),"Server");
			GUI.Label(new Rect(100,125,100,25),"Connections: " + Network.connections.Length );
			for(i=0;i<Network.connections.Length;i++){
				int offset = i*50;
				GUI.Label(new Rect(100,(150+offset),150,25),Network.connections[i].guid);
				GUI.Label(new Rect(100,(150+offset+25),100,25),Network.connections[i].externalIP);
				GUI.Label(new Rect(200,(150+offset+25),100,25),Menu.connectionList[i].username);
				GUI.Label(new Rect(300,(150+offset+25),100,25),(Menu.connectionList[i].ready?"Ready":"NotReady"));
				
				//button to kick
			}
			if(GUI.Button(new Rect(100,150+(i*50),100,25),"Shutdown")){
				Application.LoadLevel(0);
				Network.Disconnect(250);	
			}
			break;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}

	void OnDisconnectedFromServer(NetworkDisconnection info){
		//called on client when client disconnects, and on server when connection has disconnected.
		Application.LoadLevel(0);
		Debug.Log("Disconnected from server: " + info);
	}

	public void moveUnit(Vector2 _selected, Vector2 _point){
		if(turn == me){
			Vector3 selected = new Vector3(_selected.x,1,_selected.y);
			Vector3 point = new Vector3(_point.x,1,_point.y);
			//theres no server checking here. dont send 'me' later.
			networkView.RPC("NetworkMove",RPCMode.All,me,Network.player.guid,selected,point);
		}
	}

	[RPC]
	void Clear(NetworkMessageInfo info) {
		if(Network.isServer){
			return;
		} else if(Network.isClient){
			if(info.sender.guid == server){
				Menu.connectionList.Clear();
			}
		}
	}

	[RPC]
	void Initialize(NetworkMessageInfo info) {
		if(Network.isServer){
			return;
		} else if(Network.isClient){
			if(info.sender.guid == server){
				me = Menu.connectionList.FindIndex(x => x.guid == Network.player.guid);
			}
		}
	}
	
	[RPC]
	void SpawnObject(int _i, string _guid, string _name, Vector3 _location, NetworkMessageInfo info){
		//if(info.sender.guid == server){
		//TODO : Fuck unity rpcs
		if(!(Menu.connectionList[_i].guid == _guid)){
			_i = Menu.connectionList.FindIndex(x => x.guid == _guid);
		}
		while(_i>=playerObjects.Count){
			playerObjects.Add(new List<GameObject>());
		}
		playerObjects[_i].Add((GameObject)Instantiate(Resources.Load(_name)));
		hexWorld.hexWorldData[(int)_location.x,(int)_location.z].unit = _name;
		hexWorld.hexWorldData[(int)_location.x,(int)_location.z].unitObject = playerObjects[_i][playerObjects[_i].Count-1];
		Vector2 center = hexWorld.hexWorldData[(int)_location.x,(int)_location.z].center;
		Vector3 finalloc = new Vector3(center.x,1,center.y);
		Debug.Log (center.x + " " + center.y + " " + finalloc.x + " " + finalloc.z);
		playerObjects[_i][playerObjects[_i].Count-1].transform.position = finalloc;
		//}
	}

	[RPC]
	void NetworkMove(int _i, string _guid, Vector3 _selected, Vector3 _point, NetworkMessageInfo info){
		//are you here to fix it sending 'me' aka _i? Have it hunt for sender.guid over all connected guids.
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unit = hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit;
		hexWorld.hexWorldData[(int)_point.x,(int)_point.z].unitObject.transform.position = new Vector3 (hexWorld.hexWorldData[(int)_point.x,(int)_point.z].center.x, 1 , hexWorld.hexWorldData[(int)_point.z,(int)_point.z].center.y);
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unitObject = null;
		hexWorld.hexWorldData[(int)_selected.x,(int)_selected.z].unit = null;
	}
	
	[RPC]
	void AddConnectionList(string _guid, string _username, NetworkMessageInfo info) {
		if(Network.isServer){
			return;
		} else if(Network.isClient){
			if(info.sender.guid == server){
				Menu.connectionList.Add(new Menu.NConn(_username,_guid));
			}
		}
	}

	[RPC]
	void RequestTurnSwitch(NetworkMessageInfo info){
		if(Network.isServer){
			int c = Menu.connectionList.Count;
			for(int i = 0; i<c; i++){
				if(info.sender.guid == Menu.connectionList[i].guid){
					if(turn == i){
						networkView.RPC("SwitchTurn",RPCMode.All,(turn+1>c?0:(turn+1)));
					}
				}
			}
		}
	}

	[RPC]
	void SwitchTurn(int _newTurn, NetworkMessageInfo info){
		if(info.sender.guid == server)
			turn = _newTurn;
	}
}
